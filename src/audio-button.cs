using UnityEngine;
using UnityEngine.EventSystems;

public class AudioButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private AudioClip customSound;
    [SerializeField] private bool useCustomSound = false;
    [SerializeField] private float pitchVariation = 0.1f;
    [SerializeField] private bool usePitchVariation = false;
    
    // Visual feedback
    [SerializeField] private float scaleOnPress = 0.9f;
    private Vector3 originalScale;
    
    private void Start()
    {
        originalScale = transform.localScale;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        // Visual feedback - scale down
        transform.localScale = originalScale * scaleOnPress;
        
        // Play sound with variations if enabled
        if (AudioManager.Instance != null)
        {
            if (useCustomSound && customSound != null)
            {
                // Play custom sound
                AudioSource source = AudioManager.Instance.GetComponent<AudioSource>();
                if (usePitchVariation)
                {
                    // Save original pitch
                    float originalPitch = source.pitch;
                    
                    // Apply random pitch variation
                    source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
                    
                    // Play sound
                    source.PlayOneShot(customSound);
                    
                    // Restore original pitch
                    source.pitch = originalPitch;
                }
                else
                {
                    source.PlayOneShot(customSound);
                }
            }
            else
            {
                // Play default button click sound
                AudioManager.Instance.PlayButtonClickSound();
            }
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        // Return to original scale
        transform.localScale = originalScale;
    }
}
