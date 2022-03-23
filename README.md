<h1 align="center">
  Audio VR Shooting Game
</h1>
<h6 align="center"> 
  Xinyu Bai, Jonathan Tang, Daniel Wang
</h6>
<h6 align="center"> 
  Dyson School of Design Engineering | Imperial College London
</h6>

***

The installation is an **interactive audio-based shooting game** running on the Oculus Quest VR platform using Unity. The main mechanism of the game is to shoot at incoming audio cubes using the audio weapon to achieve a higher score. The player has to make sounds to shoot projectiles from the weapon and destroy incoming audio cubes which are generated from the music track playing in the background. The installation was exhibited at the Dyson School of Design Engineering on 22nd March 2022. The following video shows a recording of the exhibition demo.

https://user-images.githubusercontent.com/53417086/159577907-92d8c4ff-b591-4317-ab4e-4892538281db.mp4

## Game Features Overview
The game consists of three major components: Audio weapon control, Audio target generation and Spatial Sound Effect. All of them are controlled/generated based on **real-time continuous signal processing**. Player must shoot at the audio cubes to destroy them and increase points. Missed cubes (within 1.5m of the player) will destroy themselves and add up to number of missed. A detailed flow diagram of the game mechanism is shown below.

![SystemDiagram](https://user-images.githubusercontent.com/53417086/159578853-41d1c046-01ff-4962-963f-0a73cdd8b531.svg)
<p align = "center"><em>Figure 1: Game Flow System Diagram</em></p>

### Part 1) Audio Weapon Control
The player is able to control the weapon projectile using real-time audio input through a microphone. The higher the amplitude of the sound made by the player, the larger the projectile velocity and the greater the haptics feedback intensity from the VR controller. Similarly, the higher the frequency of the sound made by the player, the smaller the projectile size and the greater the haptics intensity. Presence of local frequency maxima can initiate a change in colour of the projectile.

### Part 2) Audio Target Generation
The audio targets are generated based on real-time beat detection algorithm implemented on the music track. They are generated at random positions when a beat is detected but within a pre-defined sphere constraint. The size of the target is changing with the amplitude of music track. The colour of the target is changing with the dominating frequency band of the track.

Other features such as room lighting is changing with the shifting of the spectral centroid of the signal. The yellow bars are a visual representation of the first 160 frequency bins after performing Fast Fourier Transform on the music track.

### Part 3) Spatial Audio Effect
Each audio cube is attached to a spatial sound effect. They each has a spawning sound effect and a destroy sound effect. 

> **Remarks:** My main contribution to the team is on **Part 2**. However, we collectively worked on audio signal processing and VR game development including testing and debugging. As a result, most of our work are heavily shared in the end.

## Audio Theories Behind the Game

### Real-time Signal Processing
Notably, all the incoming audio signals from either the microphone **(Part 1)** or the music track **(Part 2)** are processed in real-time continuously using Fast Fourier Transform (FFT). FFT converts the amplitude over time domain into the frequency domain, returning relative amplitude at each frequency bin. This is handled by Unity’s inbuilt function `GetSpectrumData()`. 

        public static float[] _samples = new float[512];
        void GetSpectrumAudioSource(){
           _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
        }<img width="631" alt="512frequency_cubes1" src="https://user-images.githubusercontent.com/53417086/159595478-7a2e9318-96a9-46cf-8649-e74db41646f8.png">

     
Incoming audio signals are sampled at the rate of 44100 Hz using Unity. 512 frequency bins are created. However, based on Nyquist Theorem, frequencies above Nyquist frequency (22050 Hz – 44100 Hz) are discarded as they are mirroring the first half of the range (0 Hz – 22050 Hz) due to aliasing effect. As such the resulting frequency split is 22050/512 = 43 Hz. Frequencies can be accessed using index of the data array such as _samples[2] = 86 – 129Hz. Blackman window is used to handle spectral leakage, producing more defined frequency responses. We created 512 scalable cube objects in Unity to visualise this result in real time.
FFT at Timestamp A             |  FFT at Timestamp B
:-------------------------:|:-------------------------:
![](https://user-images.githubusercontent.com/53417086/159595350-4aa00e42-f6a2-49fd-8da2-5a0fbafabcf4.png)  |  ![](https://user-images.githubusercontent.com/53417086/159595516-44ea83db-b703-4540-843a-f6daef30b2c5.png)

### Amplitude Analysis
### Frequency Analysis
#### Frequency Band Analysis
#### Spectral Centroid Analysis
### Beat Detection
### A-weighting
Since human perceives sounds differently, we applied an A-weighting algorithm in **Part 1** to the incoming microphone signal. This 
        void ApplyAWeighting(_sampleFreq, _sampleAmp) {
          float _sampleWeighting = (Mathf.Pow(12194, 2f) * Mathf.Pow(_sampleFreq, 4f)) / (((Mathf.Pow(_sampleFreq, 2f) + (Mathf.Pow(20.6f, 2f)))) * (Mathf.Sqrt((Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(107.7f, 2f)) * (Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(737.9f, 2f)))) * (Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(12194f, 2f)));
          _samplesWeighted = _sampleWeighting * _sampleAmp;
          return _samplesWeighted;
        }
### Spatial Sound Effect

## Build & Development
### Setup & Platforms
### Development Process

## Interesting Game Mechanisms


