using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    [Header("Value Text (Optional)")]
    [SerializeField] private TextMeshProUGUI masterValueText;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI sfxValueText;
    
    [Header("Audio Preview Buttons")]
    [SerializeField] private Button musicPreviewButton;
    [SerializeField] private Button sfxPreviewButton;
    
    [Header("Panel Controls")]
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button closeSettingsButton;
    
    private void Start()
    {
        // Setup initial slider values
        if (AudioManager.Instance != null)
        {
            // Get initial values from AudioManager (optional)
            // Or set default values
            if (masterVolumeSlider) masterVolumeSlider.value = 1f;
            if (musicVolumeSlider) musicVolumeSlider.value = 0.7f;
            if (sfxVolumeSlider) sfxVolumeSlider.value = 1f;
        }
        
        // Add listeners to sliders
        SetupSliderListeners();
        
        // Add listeners to buttons
        SetupButtonListeners();
        
        // Initial UI update
        UpdateValueText();
        
        // Initially hide the settings panel if available
        if (audioSettingsPanel)
            audioSettingsPanel.SetActive(false);
    }
    
    private void SetupSliderListeners()
    {
        if (masterVolumeSlider)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        
        if (musicVolumeSlider)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        if (sfxVolumeSlider)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    private void SetupButtonListeners()
    {
        if (musicPreviewButton)
            musicPreviewButton.onClick.AddListener(OnMusicPreviewClicked);
        
        if (sfxPreviewButton)
            sfxPreviewButton.onClick.AddListener(OnSFXPreviewClicked);
        
        if (openSettingsButton)
            openSettingsButton.onClick.AddListener(OnOpenSettingsClicked);
        
        if (closeSettingsButton)
            closeSettingsButton.onClick.AddListener(OnCloseSettingsClicked);
    }
    
    // Slider event handlers
    public void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);
        
        UpdateValueText();
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
        
        UpdateValueText();
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
        
        UpdateValueText();
    }
    
    // Button event handlers
    public void OnMusicPreviewClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackgroundMusic();
    }
    
    public void OnSFXPreviewClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClickSound();
    }
    
    public void OnOpenSettingsClicked()
    {
        if (audioSettingsPanel)
            audioSettingsPanel.SetActive(true);
    }
    
    public void OnCloseSettingsClicked()
    {
        if (audioSettingsPanel)
            audioSettingsPanel.SetActive(false);
    }
    
    // Update UI text elements
    private void UpdateValueText()
    {
        if (masterValueText && masterVolumeSlider)
            masterValueText.text = Mathf.RoundToInt(masterVolumeSlider.value * 100).ToString() + "%";
        
        if (musicValueText && musicVolumeSlider)
            musicValueText.text = Mathf.RoundToInt(musicVolumeSlider.value * 100).ToString() + "%";
        
        if (sfxValueText && sfxVolumeSlider)
            sfxValueText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100).ToString() + "%";
    }
}
