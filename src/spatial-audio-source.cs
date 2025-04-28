using UnityEngine;

public class SpatialAudioSource : MonoBehaviour
{
    [Header("Audio Configuration")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool loop = false;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    
    [Header("3D Sound Settings")]
    [SerializeField] [Range(0f, 1f)] private float spatialBlend = 1f; // 0 = 2D, 1 = 3D
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private AnimationCurve rolloffCurve;
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    
    // Optional movement to demonstrate audio positioning
    [Header("Movement (Optional)")]
    [SerializeField] private bool moveObject = false;
    [SerializeField] private Vector3 movementDirection = new Vector3(1, 0, 0);
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float movementRange = 5f;
    
    private AudioSource audioSource;
    private Vector3 startPosition;
    
    private void Awake()
    {
        // Create or get audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Configure audio source
        audioSource.clip = audioClip;
        audioSource.playOnAwake = false; // We'll handle this manually
        audioSource.loop = loop;
        audioSource.volume = volume;
        
        // Configure 3D settings
        audioSource.spatialBlend = spatialBlend;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.rolloffMode = rolloffMode;
        
        // Set custom rolloff curve if provided
        if (rolloffCurve != null && rolloffCurve.keys.Length > 0)
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, rolloffCurve);
        
        // Save starting position for movement
        startPosition = transform.position;
    }
    
    private void Start()
    {
        if (playOnAwake && audioClip != null)
            audioSource.Play();
    }
    
    private void Update()
    {
        if (moveObject)
        {
            // Simple ping-pong movement to demonstrate spatial audio
            Vector3 movement = movementDirection.normalized * Mathf.Sin(Time.time * movementSpeed) * movementRange;
            transform.position = startPosition + movement;
        }
    }
    
    // Public methods to control the sound
    public void PlaySound()
    {
        if (audioClip != null && audioSource != null)
            audioSource.Play();
    }
    
    public void StopSound()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
    
    public void PauseSound()
    {
        if (audioSource != null)
            audioSource.Pause();
    }
    
    // Method to visualize the audio range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f); // Orange, semi-transparent
        Gizmos.DrawWireSphere(transform.position, minDistance);
        
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.1f); // Blue, more transparent
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
