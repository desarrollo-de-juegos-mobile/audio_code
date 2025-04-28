using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SimpleProceduralAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private int sampleRate = 44100;
    
    [Header("Tone Generator")]
    [SerializeField] private bool useToneGenerator = true;
    [SerializeField] [Range(50f, 5000f)] private float baseFrequency = 440f; // A4 note
    [SerializeField] [Range(0f, 1f)] private float amplitude = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float fadeInTime = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float fadeOutTime = 0.2f;
    [SerializeField] private WaveformType waveformType = WaveformType.Sine;
    
    [Header("Random SFX Generator")]
    [SerializeField] private bool useRandomSFX = false;
    [SerializeField] [Range(0.1f, 3f)] private float maxDuration = 0.5f;
    [SerializeField] [Range(0, 10)] private int randomSeed = 0;
    [SerializeField] [Range(2, 10)] private int randomness = 5;
    
    private AudioSource audioSource;
    private bool isPlaying = false;
    private float currentAmplitude = 0f;
    private float phase = 0f;
    private System.Random randomGenerator;
    
    public enum WaveformType
    {
        Sine,
        Square,
        Triangle,
        Sawtooth,
        Noise
    }
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        
        randomGenerator = new System.Random(randomSeed);
    }
    
    private void Start()
    {
        if (playOnStart)
        {
            PlaySound();
        }
    }
    
    private void OnDestroy()
    {
        StopSound();
    }
    
    // Generate audio data procedurally
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPlaying) return;
        
        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = 0f;
            
            if (useToneGenerator)
            {
                // Generate tone based on selected waveform
                sample = GenerateWaveform(waveformType);
                
                // Increment phase
                phase += 2 * Mathf.PI * baseFrequency / sampleRate;
                if (phase > 2 * Mathf.PI) phase -= 2 * Mathf.PI;
            }
            else if (useRandomSFX)
            {
                // Generate random noise with some structure
                sample = GenerateRandomSFX();
            }
            
            // Apply the sample to all channels
            for (int j = 0; j < channels; j++)
            {
                data[i + j] = sample * currentAmplitude;
            }
        }
    }
    
    private float GenerateWaveform(WaveformType type)
    {
        switch (type)
        {
            case WaveformType.Sine:
                return Mathf.Sin(phase);
                
            case WaveformType.Square:
                return Mathf.Sin(phase) >= 0 ? 0.6f : -0.6f; // Square wave with slight volume adjustment
                
            case WaveformType.Triangle:
                return 1f - 4f * Mathf.Abs(Mathf.Round(phase / Mathf.PI - 0.25f) - (phase / Mathf.PI - 0.25f));
                
            case WaveformType.Sawtooth:
                return (2f * (phase / (2f * Mathf.PI) - Mathf.Floor(0.5f + phase / (2f * Mathf.PI))));
                
            case WaveformType.Noise:
                return (Random.value * 2f - 1f);
                
            default:
                return 0f;
        }
    }
    
    private float GenerateRandomSFX()
    {
        // Create semi-random sound effects with some structure
        float time = Time.time * 10f;
        float noise = (float)randomGenerator.NextDouble() * 2f - 1f;
        
        // Add some structure based on randomness level
        float structure = 0;
        for (int i = 1; i <= randomness; i++)
        {
            structure += Mathf.Sin(time / i + noise * i) / i;
        }
        
        structure /= randomness * 0.5f;
        return structure * 0.7f + noise * 0.3f; // Mix structure and pure noise
    }
    
    // Public methods to control the sound
    public void PlaySound()
    {
        if (isPlaying) return;
        
        // Reset phase
        phase = 0f;
        
        // Start with zero amplitude and fade in
        currentAmplitude = 0f;
        audioSource.Play();
        isPlaying = true;
        
        // Start fade in
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }
    
    public void StopSound()
    {
        if (!isPlaying) return;
        
        // Fade out before stopping
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }
    
    // Change frequency in real-time (useful for UI interactions)
    public void SetFrequency(float frequency)
    {
        baseFrequency = Mathf.Clamp(frequency, 50f, 5000f);
    }
    
    // Change waveform in real-time
    public void SetWaveform(int waveformIndex)
    {
        waveformType = (WaveformType)waveformIndex;
    }
    
    // Create smooth transitions
    private IEnumerator FadeIn()
    {
        float timer = 0f;
        
        while (timer < fadeInTime)
        {
            currentAmplitude = Mathf.Lerp(0f, amplitude, timer / fadeInTime);
            timer += Time.deltaTime;
            yield return null;
        }
        
        currentAmplitude = amplitude;
    }
    
    private IEnumerator FadeOut()
    {
        float timer = 0f;
        float startAmplitude = currentAmplitude;
        
        while (timer < fadeOutTime)
        {
            currentAmplitude = Mathf.Lerp(startAmplitude, 0f, timer / fadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }
        
        currentAmplitude = 0f;
        audioSource.Stop();
        isPlaying = false;
    }
    
    // Simple method to play a random tone (can be called from buttons)
    public void PlayRandomTone()
    {
        baseFrequency = Random.Range(200f, 800f);
        PlaySound();
        
        // Auto-stop after a short time
        StartCoroutine(AutoStop());
    }
    
    private IEnumerator AutoStop()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, maxDuration));
        StopSound();
    }
}
