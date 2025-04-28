using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambient3DSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip collectibleSound;
    [SerializeField] private AudioClip ambientSound;

    [Header("Audio Control")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize audio sources if not set in the inspector
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        if (ambient3DSource == null)
        {
            ambient3DSource = gameObject.AddComponent<AudioSource>();
            ambient3DSource.spatialBlend = 1f; // Make it fully 3D spatial
            ambient3DSource.rolloffMode = AudioRolloffMode.Linear;
            ambient3DSource.minDistance = 1f;
            ambient3DSource.maxDistance = 10f;
        }

        // Set initial configuration
        musicSource.loop = true;
        UpdateVolumeLevels();
    }

    private void Start()
    {
        // Start playing background music
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Background music clip not assigned!");
        }
    }

    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }

    public void PlayCollectibleSound()
    {
        PlaySFX(collectibleSound);
    }

    public void PlayAmbientSoundAt(Vector3 position)
    {
        if (ambientSound != null)
        {
            ambient3DSource.transform.position = position;
            ambient3DSource.clip = ambientSound;
            ambient3DSource.Play();
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void UpdateVolumeLevels()
    {
        // Update mixer settings
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        
        // Update source volumes directly as backup
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        ambient3DSource.volume = sfxVolume;
    }

    // Volume control methods for UI sliders
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateVolumeLevels();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateVolumeLevels();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateVolumeLevels();
    }

    // Mobile-specific optimizations
    private void OnApplicationPause(bool pause)
    {
        // When app is paused (in background), pause audio
        if (pause)
        {
            musicSource.Pause();
            ambient3DSource.Pause();
        }
        else
        {
            musicSource.UnPause();
            ambient3DSource.UnPause();
        }
    }
}
