# AudioMixer Configuration for Mobile Audio Demo

## Group Structure

1. **Master**
   * Main group that controls all audio
   * Exposed parameters:
      * `MasterVolume` (logarithmic range: -80dB to 0dB)

2. **Music**
   * Subgroup for background music
   * Exposed parameters:
      * `MusicVolume` (logarithmic range: -80dB to 0dB)
   * Effects:
      * Light compressor (ratio 2:1)
      * Limiter (-2dB)

3. **SFX**
   * Subgroup for sound effects
   * Exposed parameters:
      * `SFXVolume` (logarithmic range: -80dB to 0dB)
   * Effects:
      * Limiter (-1dB)

4. **Ambient** (optional)
   * Subgroup for ambient sounds
   * Independent volume control

## Steps to Create the AudioMixer

1. In Unity, go to **Window > Audio > Audio Mixer**
2. Click the "+" button to create a new AudioMixer
3. Name the main mixer "MobileAudioMixer"
4. In the group view, right-click on "Master" and choose "Create Child Group"
5. Create the groups "Music", "SFX" and optionally "Ambient"
6. In the Inspector view, for each group:
   * Go to the "Volume" section
   * Right-click and select "Expose Parameter"
   * Name the parameters as indicated above

## Effects Configuration

### For the Music group:
1. Click "Add Effect" in the Inspector
2. Select "Compressor"
3. Configure:
   * Threshold: -15 dB
   * Ratio: 2:1
   * Attack: 50 ms
   * Release: 200 ms
4. Add another "Limiter" effect
5. Configure:
   * Threshold: -2 dB
   * Release: 50 ms

### For the SFX group:
1. Click "Add Effect" in the Inspector
2. Select "Limiter"
3. Configure:
   * Threshold: -1 dB
   * Release: 50 ms

## Snapshots (Preset Configurations)

1. Create "Default" snapshot
   * Normal volumes for all tracks
2. Create "Paused" snapshot
   * Reduce Music to -10dB
3. Create "Muted" snapshot
   * Reduce everything to -80dB

## Connection with Scripts

```csharp
// In AudioManager.cs
public void UpdateVolumeLevels()
{
    // Use logarithmic method to control volume
    float masterVolumeDB = masterVolume > 0.001f ? Mathf.Log10(masterVolume) * 20 : -80;
    float musicVolumeDB = musicVolume > 0.001f ? Mathf.Log10(musicVolume) * 20 : -80;
    float sfxVolumeDB = sfxVolume > 0.001f ? Mathf.Log10(sfxVolume) * 20 : -80;
    
    audioMixer.SetFloat("MasterVolume", masterVolumeDB);
    audioMixer.SetFloat("MusicVolume", musicVolumeDB);
    audioMixer.SetFloat("SFXVolume", sfxVolumeDB);
}
```

## Considerations for Mobile

* Audio effects consume CPU, use them sparingly
* For low-end devices, create a "LowPerformance" snapshot that removes effects
* Assign AudioSources to the appropriate groups:

```csharp
audioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Music")[0];
```

## AudioMixer Preview

```
MobileAudioMixer (Master)
├── Music
│   ├── [Compressor]
│   └── [Limiter]
├── SFX
│   └── [Limiter]
└── Ambient
```
