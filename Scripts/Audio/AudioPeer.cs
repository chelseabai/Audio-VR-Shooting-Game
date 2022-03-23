using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Linq;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;

    //Microphone Input
    public AudioClip _audioClip;
    public bool _useMicrophone;
    public string _selectedDevice;
    public int _microphoneNo;
    public AudioMixerGroup _mixerGroupMicrophone, _mixerGroupMaster;

    public static float[] _samples = new float[512];
    public static float[] _samplesWeighted = new float[512];
    public static float[] _freqBand = new float[8];
    public static float[] _bufferBand = new float[8];
    float[] _bufferDecrease = new float[8];

    float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    public static float _Amplitude, _AmplitudeBuffer, _AmplitudeNoRatio;
    float _AmplitudeHighest;
    public float _audioProfile;
    public static float _mainFreq, _mainFreqAmp;
    public int _mainFreqIndex;
    public static float _1Freq, _1FreqAmp;
    public int _1FreqIndex;
    public static float _2Freq, _2FreqAmp;
    public int _2FreqIndex;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);
        if (_useMicrophone){
            if (Microphone.devices.Length > 0){
                _audioSource.outputAudioMixerGroup = _mixerGroupMicrophone;
                _selectedDevice = Microphone.devices[_microphoneNo].ToString();
                _audioSource.clip = Microphone.Start(_selectedDevice, true, 600, AudioSettings.outputSampleRate);
                }else{
                    _useMicrophone = false;
                }
        }
        if (!_useMicrophone){
            _audioSource.outputAudioMixerGroup = _mixerGroupMaster;
            _audioSource.clip = _audioClip;
        }

        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        ApplyAWeighting();
        GetMainFreq();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
    }

    void AudioProfile(float audioProfile){
        for (int i = 0; i < 8; i++){
            _freqBandHighest[i] = audioProfile;
        }
    }

    void GetAmplitude(){
        float _CurrentAmplitude = 0;
        float _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++){
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if (_CurrentAmplitude > _AmplitudeHighest){
            _AmplitudeHighest = _CurrentAmplitude;
        }
        _Amplitude = _CurrentAmplitude/ _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
        _AmplitudeNoRatio = _CurrentAmplitude;

    }

    void CreateAudioBands(){
        for (int i = 0; i < 8; i++){
            if (_freqBand[i] > _freqBandHighest[i]){
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bufferBand[i]/_freqBandHighest[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);

        // for (int i = 1; i < _samples.Length - 1; i++)
        // {
        //     Debug.DrawLine(new Vector3(i - 1, _samples[i] + 10, 0), new Vector3(i, _samples[i + 1] + 10, 0), Color.red);
        //     Debug.DrawLine(new Vector3(i - 1, Mathf.Log(_samples[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(_samples[i]) + 10, 2), Color.cyan);
        //     Debug.DrawLine(new Vector3(Mathf.Log(i - 1), _samples[i - 1] - 10, 1), new Vector3(Mathf.Log(i), _samples[i] - 10, 1), Color.green);
        //     Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(_samples[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(_samples[i]), 3), Color.blue);
        // }
    }

    void ApplyAWeighting() {
        // _samplesWeighted = _samples.Clone();
        _samplesWeighted = new float[512];
        _samples.CopyTo(_samplesWeighted, 0);
        // Debug.Log(_samplesWeighted.Length);
        int count = 1;
        foreach (float _sampleAmp in _samplesWeighted) {
            float _sampleFreq = count * ((AudioSettings.outputSampleRate/2)/512);
            float _sampleWeighting = (Mathf.Pow(12194, 2f) * Mathf.Pow(_sampleFreq, 4f)) / (((Mathf.Pow(_sampleFreq, 2f) + (Mathf.Pow(20.6f, 2f)))) * (Mathf.Sqrt((Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(107.7f, 2f)) * (Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(737.9f, 2f)))) * (Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(12194f, 2f)));
            // _samplesWeighted[count-1] = 20 * Mathf.Log(_sampleWeighting * _sampleAmp) - 20 * Mathf.Log(_sampleWeighting * _sampleAmp * 1000);
            _samplesWeighted[count-1] = _sampleWeighting * _sampleAmp;
            count++;
        }
        // Debug.Log("weighted");
    }

    void GetMainFreq(){
        List<float> _samplesWeightedPeaks = new List<float>();
        int n = 512;
        if (_samplesWeighted[0] > _samplesWeighted[1]) {
            _samplesWeightedPeaks.Add(_samplesWeighted[0]);
        }
        for (int i =1; i < n - 1; i++) {
            if ((_samplesWeighted[i-1] < _samplesWeighted[i]) && (_samplesWeighted[i] > _samplesWeighted[i+1])) {
                _samplesWeightedPeaks.Add(_samplesWeighted[i]);
            }
        }
        if (_samplesWeighted[n - 1] > _samplesWeighted[n - 2]) {
            _samplesWeightedPeaks.Add(_samplesWeighted[n - 1]);
        }
        _samplesWeightedPeaks.Sort();
        _samplesWeightedPeaks.Reverse();
        _mainFreqAmp = _samplesWeightedPeaks[0];
        _1FreqAmp = _samplesWeightedPeaks[1];
        _2FreqAmp = _samplesWeightedPeaks[2];

        // _mainFreqAmp = _samplesWeighted.Max();
        _mainFreqIndex = _samplesWeighted.ToList().IndexOf(_mainFreqAmp);
        _mainFreq = (_mainFreqIndex + 1) * ((AudioSettings.outputSampleRate/2)/512);

        
        _1FreqIndex = _samplesWeighted.ToList().IndexOf(_1FreqAmp);
        _1Freq = (_1FreqIndex + 1) * ((AudioSettings.outputSampleRate/2)/512);
        _2FreqIndex = _samplesWeighted.ToList().IndexOf(_2FreqAmp);
        _2Freq = (_2FreqIndex + 1) * ((AudioSettings.outputSampleRate/2)/512);
        // Debug.Log(_mainFreqAmp);
        // Debug.Log(_mainFreqIndex);
        // Debug.Log(_mainFreq);
    }

    void BandBuffer(){
        for (int g= 0; g<8; g++){
            if (_freqBand[g] > _bufferBand[g]){
                _bufferBand[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }
            if (_freqBand[g] < _bufferBand[g]){
                _bufferBand[g] -= _freqBand[g];
                _bufferDecrease[g] *= 1.2f;
            }

        }
    }

    void MakeFrequencyBands(){
        /*
            22050 / 512 = 43hertz per sample

            20-60 hertz      2 0-86          
            60-250 hertz     4 87-258
            250-500 hertz    8 259-602
            500-2000 hertz   16 603-1290
            2000-4000 hertz  32 1291-2666
            4000-6000 hertz  64 2667-5418
            6000-20000 hertz 128 5419-10922
                             256 10922-21930
        
        */
        int count = 0;
        for (int i = 0; i < 8; i++){

            float average = 0;
            int sampleCount = (int)Mathf.Pow(2,i) * 2;
            if (i == 7) {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++){
                average += _samples[count] * (count + 1);
                count++;
            }
            average /= count;
            _freqBand[i] = average * 10;
        }
    }
}
