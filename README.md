<h1 align="center">
  Audio VR Shooting Game
</h1>
<h6 align="center"> 
  Xinyu Bai, with Jonathan Tang & Daniel Wang
</h6>
<h6 align="center"> 
  Dyson School of Design Engineering | Imperial College London
</h6>

***

The installation is an **interactive audio-based shooting game** running on the Oculus Quest VR platform using Unity. The main mechanism of the game is to shoot at incoming audio targets using the audio weapon to achieve a higher score. The player has to make sounds to shoot projectiles from the weapon and destroy incoming audio targets which are generated from the music track playing in the background. The installation was exhibited at the Dyson School of Design Engineering on 22nd March 2022. The following video shows a recording of the exhibition demo.

https://user-images.githubusercontent.com/53417086/159577907-92d8c4ff-b591-4317-ab4e-4892538281db.mp4

## Game Features Overview
The game consists of three major components: Audio weapon control, Audio target generation and Spatial Sound Effect. All of them are controlled/generated based on **real-time continuous signal processing**. Player must shoot at the audio targets to destroy them and increase points. Missed targets (within 1.5m of the player) will destroy themselves and add up to number of missed. A detailed flow diagram of the game mechanism is shown below.

<img width="2564" alt="image" src="https://user-images.githubusercontent.com/53417086/159635456-601f9063-32ba-4327-8d4b-f6e89a029499.png">
<p align = "center"><em>Figure 1: Game Flow System Diagram</em></p>

### Part 1) Audio Weapon Control
The player is able to control the weapon projectile using real-time audio input through a microphone. The higher the amplitude of the sound made by the player, the larger the projectile velocity and the greater the haptics feedback intensity from the VR controller. Similarly, the higher the pitch of the sound made by the player, the smaller the projectile size and the greater the haptics intensity. Presence of local frequency maxima can initiate a change in colour of the projectile.

### Part 2) Audio Target Generation
The audio targets are generated based on real-time beat detection algorithm implemented on the music track. They are generated at random positions when a beat is detected but within a pre-defined sphere constraint. The size of the target is changing with the amplitude of music track. The colour of the target is changing with the dominating frequency band of the track.

Other features such as room lighting is changing with the shifting of the spectral centroid of the signal. The yellow bars are a visual representation of the first 160 frequency bins after performing Fast Fourier Transform on the music track.

### Part 3) Spatial Audio Effect
Each audio target is attached to a spatial sound effect. They each has a spawning sound effect and a destroy sound effect. 

> **Remarks:** My main contribution to the team is on **Part 2**. However, we collectively worked on audio signal processing and VR game development including testing and debugging. As a result, most of our work are heavily shared in the end.

## Audio Theories Behind the Game

### Real-time Signal Processing
Notably, all the incoming audio signals from either the microphone **(Part 1)** or the music track **(Part 2)** are processed in real-time continuously using Fast Fourier Transform (FFT). FFT converts the amplitude over time domain into the frequency domain, returning relative amplitude at each frequency bin. This is handled by Unity’s inbuilt function `GetSpectrumData()`. 

        public static float[] _samples = new float[512];
        void GetSpectrumAudioSource(){
           _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
        }
     
Incoming audio signals are sampled at the rate of 44100 Hz using Unity. 512 frequency bins are created. However, based on **Nyquist Theorem**, frequencies above Nyquist frequency (22050 Hz – 44100 Hz) are discarded as they are mirroring the first half of the range (0 Hz – 22050 Hz) due to aliasing effect. As such the resulting frequency split is 22050/512 = 43 Hz. Frequencies can be accessed using index of the data array such as _samples[2] = 86 – 129Hz. Blackman window is used to handle spectral leakage, producing more defined frequency responses. We created 512 scalable cube objects in Unity to visualise this result in real time.

FFT at Timestamp A             |  FFT at Timestamp B
:-------------------------:|:-------------------------:
![](https://user-images.githubusercontent.com/53417086/159595350-4aa00e42-f6a2-49fd-8da2-5a0fbafabcf4.png)  |  ![](https://user-images.githubusercontent.com/53417086/159595516-44ea83db-b703-4540-843a-f6daef30b2c5.png)

### A-weighting
Since human perceives sounds differently, we applied an A-weighting algorithm in **Part 1** to the incoming microphone signal. This helps to adjust the incoming signal to the **relative loudness perceived by our ear**, so that the velocity of the projectile will match with perceived loudness better.
   
        void ApplyAWeighting(_sampleFreq, _sampleAmp) {
          float _sampleWeighting = (Mathf.Pow(12194, 2f) * Mathf.Pow(_sampleFreq, 4f)) / (((Mathf.Pow(_sampleFreq, 2f) + (Mathf.Pow(20.6f, 2f)))) * (Mathf.Sqrt((Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(107.7f, 2f)) * (Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(737.9f, 2f)))) * (Mathf.Pow(_sampleFreq, 2f) + Mathf.Pow(12194f, 2f)));
          _samplesWeighted = _sampleWeighting * _sampleAmp;
          return _samplesWeighted;
        }
        
### Amplitude Analysis
Both projectile velocity **(Part 1)** and the size of the generated audio target **(Part 2)** are controlled by amplitude input. Real-time total amplitude `totalAmplitude` is obtained by adding up amplitudes of each frequency bin using a “for loop”. Since the obtained amplitude is a **relative value**, we created a variable called `highestAmplitude` to keep track of the highest amplitude recorded so far. We then used their **ratio** to control the game features instead. For audio targets, their scale refers to `scaleFactor = amplitudeRatio*1.5 + 0.75`. This prevents over scaling of the feature value by sounds which are too loud or too quiet. 

        for (int i=0; i < _samples.Length; i++){
            totalAmplitude += _samples[i];
        }
        if (totalAmplitude > highestAmplitude){
            highestAmplitude = totalAmplitude;
        }
        amplitudeRatio = totalAmplitude/highestAmplitude;


### Frequency Analysis
#### Frequency Band Analysis
The colour of the audio target is controlled by the dominating frequency band **(Part 2)**. 9 different frequency bands are created, and they correspond to 9 different colour choices. The decision to split the frequencies into 9 different bands is tricky. Firstly, a **cut-off frequency** of 8600Hz is applied, which equates to the frequency range of the electric guitar track analysed. It is further noted that there is a high concentration of low frequencies, giving the FFT graph a skewed shape. Therefore, a **high-pass filter** is applied to discount the higher amplitude of lower frequencies. Note that frequency range and weighting are not linear. They were adjusted and tuned based on the actual signal.

Frequency Spectrum of Music Track (bgm)| Audio Target Changing Color
:-------------------------:|:-------------------------:
<img width="1200" alt="frequency_analysis" src="https://user-images.githubusercontent.com/53417086/159619285-31ec686c-9215-4e43-a390-77666e17c8a4.png"> | ![changeColor](https://user-images.githubusercontent.com/53417086/159620756-60f2d373-1bdd-401d-894d-983cd2dd9869.gif)

<em>Table 1: Frequency Bands</em>

Band Number | Index Range (i)            |Frequency Range            | Weighting| Colour
:-------------------------:|:-------------------------:|:-------------------------:|:-------------------------:|:-------------------------:
0|0 - 14 | 0Hz - 645Hz | 0.02*i | red
1|15 - 29 | 645Hz - 1290Hz | 0.02*i | orange
2|30 - 44 | 1290Hz - 1935Hz | 0.02*i | yellow
3| 45 - 59 | 1935Hz - 2580Hz | 0.01*i | green
4|60 - 74 | 2580Hz - 3225Hz | 0.01*i | cyan
5|75 - 84 | 3225Hz - 3655Hz | 0.01*i | blue
6|85 - 99 | 3655Hz - 4300Hz | 1 | purple
7|100 - 124 | 4300Hz - 5375Hz | 1 | pink
8|125 - 200 | 5375Hz - 8600Hz | 1-0.005*i | white

#### Spectral Centroid Analysis
During frequency analysis, we realised that sometimes there is a rapid change in notes (e.g.  pull off technique) in the electric guitar track. This indicates a **shift in the overall frequency**. To capture this “shift”, spectral centroid is used to identify the **centre of mass** of the spectrum, which perceptually connects with the impression of brightness of a sound. To get this centre of mass, a “for loop” is implemented which returns the frequency bin that split the total amplitude into two halves (left total frequency = right total frequency). We then applied this value to change the lighting effect of the room **(Part 2)**. A higher COM results in greater lighting intensity: `Intensity = min(COM Index/25, 2)`;

        void GetCOM(){
            float currentMass = 0;
            for (int i=0; i < _samples.Length; i++){
                currentMass += _samples[i];
                if (currentMass > 0.5* totalAmplitude){
                    COMIndex = i;
                    break;
                }
            }       
        }
 
Spectral Centroid Visualisation | Change in Lighting Intensity based on COM
:-------------------------:|:-------------------------:
<img width="1000" src="https://user-images.githubusercontent.com/53417086/159616498-3a4209f7-b301-4058-b1a5-a327c35a8397.png"> |![lighting](https://user-images.githubusercontent.com/53417086/159616235-d6634250-3d3f-4556-883f-02017fe8a3d0.gif)

### Beat Detection
Beat detection algorithm is applied to the music track, and the audio targets are generated every time when a beat is detected **(Part 2)**. The core idea behind this algorithm is the change in **sound energy**. The average energy of a couple of seconds of the sound before the current playback is calculated. This value is then compared to the current energy of the sound. If the **threshold** is passed, then a beat is detected. The algorithm is applied to the snare drum soundtrack, which sets its frequency range to roughly from 300 Hz – 1000 Hz. To achieve this idea, we implemented a **history buffer** to store the previous energies with a size of 43. The buffer is kept updated and new average energies are added to it. The threshold is also adjusted based on variance. This is because noisy music like hard rock will make the beat detection doggy and we need to decrease threshold for higher variance values. Mathematical reference of this application can be found <a href="https://www.parallelcube.com/2018/03/30/beat-detection-algorithm/">here</a>.

Beat Detection Theory Visualisation | Music Track in Time Domain
:-------------------------:|:-------------------------:
<img width="500" src="https://user-images.githubusercontent.com/53417086/159624622-e5369e6a-bb89-494c-87c6-8bab52bcc588.gif">| <img width="500" alt="beats" src="https://user-images.githubusercontent.com/53417086/159625329-61e7793d-438e-405f-a0bb-c3652503c9d9.png">

        void detectBeat(){
                Variance = VarianceAdder(historyBuffer) / historyBuffer.Length;  
                Constant = (float)((-0.0025714 * Variance) + 1.5142857);
                
                // History buffer
                float[] shiftingHistoryBuffer = new float[historyBuffer.Length]; // make a new array and copy all the values to it
                for (int i = 0; i < (historyBuffer.Length - 1); i++) { // shift the array one slot to the right
                    shiftingHistoryBuffer[i+1] = historyBuffer[i]; //fill the empty slot with the new instant sound energy
                }
                shiftingHistoryBuffer [0] = InstantSpec;
                for (int i = 0; i < historyBuffer.Length; i++) {
                    historyBuffer[i] = shiftingHistoryBuffer[i]; //then we return the values to the original array
                }
                
                // Detect beat
                if (InstantSpec > (Constant * AverageSpec)) { // now we check if we have a beat
                        if(!Beated) {
                            Debug.Log("Beat");
                            Beated = true;  
                        }
                } 
                else {
                    if(Beated) {
                        Beated = false;
                    }
                } 
        }
        
> **Note:** Since the music track we chose is quite noisy, the beat detection algorithm can correctly pick up 80% of the beats only.
        
### Spatial Sound Effect
Spatial Sound Effect:
Audio targets are spawn in the VR space with sound effects attached to each of them. To achieve 3D spatial sound, we implemented the idea of reverberation curve. Reverberation refers to the decay of sound as it decreases to a certain loudness. We tuned both the reverberation curve and volume curve for the sound effect as a function of distance between the audio target and the player.

## Game Mechanism: Application of Audio Theories to the Game
The above-mentioned audio theories are integrated into the game mechanism in various ways. Below is a full demo of the game from the player’s perspective (VR camera).

https://user-images.githubusercontent.com/53417086/159628254-f38ccb79-a515-43a0-8175-f57bba0f0e52.mp4

### Weapon Projectile Shooting
Weapon projectiles are scaled based on frequency and amplitude of the sound made. Audio signal data are accessed through a `public static` type in the Weapon script. Below is a quick demo of how is the sound signal data used:

        _shootingForce = Mathf.Pow(AudioPeer._mainFreqAmp * 1000, 3f) / 10000 + 1; // multiply amplitude by 1000 so it's guaranteed to be above 1, cube for making louder things louder, divide by 10000 to bring it back to a usable number.
        base.shootingForce = _shootingForce;
        projectileSize = 100000000 / Mathf.Pow(AudioPeer._mainFreq, 3f);
        ParticleSystem.MainModule _projectileProperties = _aProjectile.main;
        _projectileProperties.startSize = projectileSize; // set projectile size
        _aProjectileTrail.startWidth = projectileSize;  // set trail width to match projectile size
        _aProjectileTrail.endWidth = 0; // trail taper off to nothing
        _aProjectileCollider.radius = projectileSize * 0.8f / 2; // hitbox radius

`_mainFreqAmp` is accessed from the signal processing script `AudioPeer.cs`.

### Audio Target Generating
Much of the work was done on my part to generate audio targets. Firstly, I implemented a distance range to limit the spawning point of the targets. The targets are generated within +/- 10m of both x and y direction, but are at least 4m away from the player. If the condition is not met, the distance will be scaled as shown below.

        float upperRange = 10.0f;
        float x = Random.Range(-upperRange, upperRange) + origin.transform.position.x;
        float y = Random.Range(1.5f, 5.0f) + origin.transform.position.y;
        float z = Random.Range(-upperRange, upperRange) + origin.transform.position.z;
        float distance = (x-origin.transform.position.x)*(x-origin.transform.position.x) + (z-origin.transform.position.z)*(z-origin.transform.position.z);
        distance = Mathf.Pow(distance,0.5f);
        if (distance < 4.0f){
            x = 4.0f/distance * x;
            z = 4.0f/distance * z;
        }
        beatTarget.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
        Instantiate(beatTarget, new Vector3 (x,y,z), Quaternion.identity);

I later managed to add various sound components to the targets and also made the targets rotating while travelling towards the player. I also implemented the destroy animation when the target is hit.


## Build & Development
### Setup & Platforms
**Hardware:** 
- Oculus Quest 2 VR headset & controllers
- Sony Headphone
- AntLion ModMic

**Software**
- Unity

**Soundtrack**
- Painted Skies (Melodic Prog Metal) from https://www.cambridge-mt.com/ms/mtk/

### Development Process
#### Stage 1: Ideation (W6)
The original concept of the game came from Jonathan Tang. The basic idea is to create a shooting game but with sound control. Player can make interesting sounds such as "Biu" or "Oiii" to activate different weapons and projectiles depending on their sound loudness, pitch and duration.
#### Stage 2: Research (W7-8)
We researched different methods to process the sounds in real life, including trying out Python and MaxMSP. The initial plan was to use MaxMSP to analyse the signals outside Unity and use Open Sound Control to import audio data back into Unity to activate game functions. We eventually figured out that it is possible to conduct FFT and sound signal analysis in Unity and we decided to build our entire game, including the audio component, into a holistic Unity package.
#### Stage 3: Finalise Concept (W9)
The finalised concept of the game is similar to the current prototype. We split the main goal into 3 different parts as mentioned previously.  The biggest obstacle we faced is the lack of game development experience. We also researched into similar games to get inspiration.
#### Stage 4: Unity Exploration (W10-W11)
With only 2 weeks left and zero background knowledge in Unity, this stage of the development largely involves Unity Exploration and online tutorial “Speed run”. We extensively watched lots of Unity VR game development videos and managed to successfully setup a basic shooting game structure (with many struggles). We then proceeded to study audio and signal processing in Unity. Special Thanks to <a href="https://www.youtube.com/channel/UCBkub2TsbCFIfdhuxRr2Lrw">Peer Play</a> and his excellent video resources in Unity Audio Processing.
#### Stage 5: Game Development (W10-W11)
While we were breathing with Unity day and night during these two weeks, we managed to quickly iterate through the game prototypes and integrate various features into the game as we getting more experienced with Unity. With a week of development time, we successfully produced the final prototype the night before the installation day.

## Conclusion & Future Steps
The overall installation was a **success** with great user engagement and satisfaction during the demo day. Moreover, users seem all like the game mechanism and actively interacted with the game by making interesting sounds. We were even **approached by tutors** who are interested to collaborate with us in applying VR to an industry setting. Other successes included:
- [x] Largely accurate audio signal capturing and processing during the demo;
- [x] A playable game mechanism (audio targets) with reasonable success & failure rate among tested users;
- [x] Audio weapon remains sensitive in picking up sounds from different people.

Limitations of the current game include **a lack of game UI or panel** for users to easily start and pause the game. Moreover, we realised that most users **fail to recognise that audio targets are generated by beats**. Since the soundtrack we chose is really loud and fast-paced, it is likely to make users feel overwhelmed. The music track we used is also currently a mono track, making it different for us to add 3D spatial effect with the music. 

Future steps are therefore developed based on existing plan and limitations:
- [ ] Improve beat detector accuracy;
- [ ] Add in game UIs such as selectin panel and menus;
- [ ] Incorporate stereo music track into the game to create more intensive 3D audio effect;
- [ ] Implement harmonics and fundamental frequencies to the audio weapon.
- [ ] Explore the impact of resizing the width of frequency bins.



