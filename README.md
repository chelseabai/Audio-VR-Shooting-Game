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

The installation is an **interactive audio-based shooting game** running on the Oculus Quest VR platform using Unity. The main mechanism of the game is to shoot at incoming audio cubes using the audio weapon to achieve a higher score. The player has to make sounds to shoot from the weapon and destroy incoming audio cubes which are generated from the music track playing in the background. The installation was exhibited at the Dyson School of Design Engineering on 22nd March 2022. The following video shows a recording of the exhibition demo.

https://user-images.githubusercontent.com/53417086/159577907-92d8c4ff-b591-4317-ab4e-4892538281db.mp4

## Game Features Overview
The game consists of two major components: audio weapon control and audio target generation. Both are controlled/generated based on **real-time continuous signal processing**. Player must shoot at the audio cubes to destroy them and increase points. Missed cubes (within 1.5m of the player) will destroy themselves and add up to number of missed. A detailed flow diagram of the game mechanism is shown below.

![SystemDiagram](https://user-images.githubusercontent.com/53417086/159578853-41d1c046-01ff-4962-963f-0a73cdd8b531.svg)
<p align = "center"><em>Figure 1: Game Flow System Diagram</em></p>

### Audio Weapon Control
The player is able to control the weapon projectile using real-time audio input through a microphone. The higher the amplitude of the sound made by the player, the larger the projectile velocity and the greater the haptics feedback intensity from the VR controller. Similarly, the higher the frequency of the sound made by the player, the smaller the projectile size and the greater the haptics intensity. Presence of local frequency maxima can initiate a change in colour of the projectile.

### Audio Target Generation
The audio targets are generated based on real-time beat detection algorithm implemented on the music track. They are generated at random positions when a beat is detected but within a pre-defined sphere constraints. The size of the target is changing with the amplitude of music track. The colour of the target is changing with the dominating frequency band of the track. 

### Spatial Audio Effect
Each audio cube is attached to a spatial sound effect. They each has a spawning sound effect and a destroy sound effect. 

### Other Audio Features
Other features such as room lighting is changing with the shifting of the spectral centroid of the signal. The yellow bars are a visual representation of the first 160 frequency bins after performing Fast Fourier Transform on the music track.


