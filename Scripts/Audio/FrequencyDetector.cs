using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Linq;

[RequireComponent (typeof (AudioSource))]
public class FrequencyDetector : MonoBehaviour
{
    AudioSource _audioSource;

    public static float[] _samples = new float[512];
    public static float[] _freqBand = new float[9];
    public float maxFreqAmplitude;
    public static int maxFreqIndex;
    public float totalAmplitude;
    public static float amplitudeRatio;
    public static int COMIndex = 0;

    float highestAmplitude = 0;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        ProcessSignal();
        GetCOM();
    }

    void ProcessSignal(){
        totalAmplitude = 0;
        maxFreqAmplitude = 0f;
        maxFreqIndex = -1;
        for (int i=0; i < _freqBand.Length; i++){
            totalAmplitude += _freqBand[i];
            if (_freqBand[i] > maxFreqAmplitude){
                maxFreqAmplitude = _freqBand[i];
                maxFreqIndex = i;
                // Debug.Log(maxFreqAmplitude.ToString());
            }
        }
        if (totalAmplitude > highestAmplitude){
            highestAmplitude = totalAmplitude;
        }
        amplitudeRatio = totalAmplitude/highestAmplitude;
        // Debug.Log(amplitudeRatio);

    }

    void GetCOM(){
        float currentMass = 0;
        Debug.Log(currentMass/totalAmplitude);
        for (int i=0; i < _samples.Length; i++){
            currentMass += _samples[i];
            if (currentMass > 0.5* totalAmplitude){
                COMIndex = i;
                break;
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
        //band 0
        _freqBand[0] = 0;
        _freqBand[1] = 0;
        _freqBand[2] = 0;
        _freqBand[3] = 0;
        _freqBand[4] = 0;
        _freqBand[5] = 0;
        _freqBand[6] = 0;
        _freqBand[7] = 0;
        _freqBand[8] = 0;

        //band 0
        for (int i=0; i < 15; i++){
             _freqBand[0] += _samples[i] * (0.02f*i);
        }
        //band 1
        for (int i=15; i < 30; i++){
             _freqBand[1] += _samples[i] * (0.02f*i);
        }
        //band 2
        for (int i=30; i < 45; i++){
             _freqBand[2] += _samples[i] * (0.01f*i);
        }
        //band 3
        for (int i=45; i < 60; i++){
             _freqBand[3] += _samples[i] * (0.01f*i);
        }
        //band 4
        for (int i=60; i < 75; i++){
             _freqBand[4] += _samples[i] * (0.01f*i);
        }
        // //band 5
        for (int i=75; i < 85; i++){
             _freqBand[5] += _samples[i] * (0.01f*i);
        }
        // //band 6
        for (int i=85; i < 100; i++){
             _freqBand[6] += _samples[i] ;
        }
        // //band 7
        for (int i=100; i < 125; i++){
             _freqBand[7] += _samples[i];
        }
        // //band 8
        for (int i=125; i < 200; i++){
             _freqBand[8] += _samples[i] * (1-i*0.005f);
        }
        
    }


    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }
}