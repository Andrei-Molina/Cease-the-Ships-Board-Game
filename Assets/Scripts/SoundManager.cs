using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundManager : MonoBehaviour
{
    [Header("Main Menu UI Elements")]
    [SerializeField] Image soundIconMenu;
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;

    [Header("Settings UI Elements")]
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Button musicOffButton;
    [SerializeField] private Image musicOnImage;
    [SerializeField] private Image musicOffImage;
    [SerializeField] private TextMeshProUGUI musicOnText;
    [SerializeField] private TextMeshProUGUI musicOffText;
    [SerializeField] private Sprite musicOnActiveSprite;
    [SerializeField] private Sprite musicOnInactiveSprite;
    [SerializeField] private Sprite musicOffActiveSprite;
    [SerializeField] private Sprite musicOffInactiveSprite;

    [Header("Volume UI Elements")]
    [SerializeField] private Button volumeUpButton;  // Button to increase volume
    [SerializeField] private Button volumeDownButton; // Button to decrease volume
    [SerializeField] private GameObject[] volumeBars; // Array of 10 volume bars

    [Header("Sound Effects UI Elements")]
    [SerializeField] private Button soundEffectsOnButton;
    [SerializeField] private Button soundEffectsOffButton;
    [SerializeField] private Image soundEffectsOnImage;
    [SerializeField] private Image soundEffectsOffImage;
    [SerializeField] private TextMeshProUGUI soundEffectsOnText;
    [SerializeField] private TextMeshProUGUI soundEffectsOffText;
    [SerializeField] private Sprite soundEffectsOnActiveSprite;
    [SerializeField] private Sprite soundEffectsOnInactiveSprite;
    [SerializeField] private Sprite soundEffectsOffActiveSprite;
    [SerializeField] private Sprite soundEffectsOffInactiveSprite;

    [Header("Scripts Reference")]
    [SerializeField] private AudioManager audioManager;

    private bool muted = false;
    private bool soundEffectsMuted = false;

    private float bgmVolume = 1.0f; // BGM volume between 0.0f and 1.0f
    private const float volumeStep = 0.1f;
    private const int maxVolumeBars = 10;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager = AudioManager.Instance;

        if (!PlayerPrefs.HasKey("muted"))
        {
            PlayerPrefs.SetInt("muted", 0);
            PlayerPrefs.SetInt("soundEffectsMuted", 0);
            PlayerPrefs.SetFloat("bgmVolume", 1.0f); // Default full volume
            Load();
        }
        else
        {
            Load();
        }
        if (soundIconMenu != null)
            UpdateButtonIconMenu();
        ApplyAudioSettings();
        UpdateSettingsUI();
        UpdateVolumeBars();

        // Attach button listeners for volume controls
        volumeUpButton.onClick.AddListener(IncreaseVolume);
        volumeDownButton.onClick.AddListener(DecreaseVolume);
    }

    public void OnButtonPress()
    {
        muted = !muted;
        AudioListener.volume = muted ? 0f : bgmVolume; // Toggle mute and unmute
        Save();
        UpdateButtonIconMenu();
        UpdateSettingsUI();
    }

    private void UpdateButtonIconMenu()
    {
        soundIconMenu.sprite = muted ? soundOffSprite : soundOnSprite;
    }

    private void Load()
    {
        muted = PlayerPrefs.GetInt("muted") == 1;
        soundEffectsMuted = PlayerPrefs.GetInt("soundEffectsMuted") == 1;
        bgmVolume = PlayerPrefs.GetFloat("bgmVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("muted", muted ? 1 : 0);
        PlayerPrefs.SetInt("soundEffectsMuted", soundEffectsMuted ? 1 : 0);
        PlayerPrefs.SetFloat("bgmVolume", bgmVolume);
        PlayerPrefs.Save();
    }

    private void ApplyAudioSettings()
    {
        audioManager.BgSource.volume = muted ? 0f : bgmVolume;
    }

    public void OnSoundOnClick()
    {
        muted = false;
        audioManager.BgSource.volume = bgmVolume;
        Save();
        UpdateSettingsUI();
    }

    public void OnSoundOffClick()
    {
        muted = true;
        audioManager.BgSource.volume = 0f;
        Save();
        UpdateSettingsUI();
    }

    private void UpdateSettingsUI()
    {
        if (muted)
        {
            musicOnImage.sprite = musicOnInactiveSprite;
            musicOffImage.sprite = musicOffActiveSprite;
            SetTextAlpha(musicOnText, 128);  // Inactive
            SetTextAlpha(musicOffText, 255); // Active
        }
        else
        {
            musicOnImage.sprite = musicOnActiveSprite;
            musicOffImage.sprite = musicOffInactiveSprite;
            SetTextAlpha(musicOnText, 255);  // Active
            SetTextAlpha(musicOffText, 128); // Inactive
        }

        // Update sound effects UI
        if (soundEffectsMuted)
        {
            soundEffectsOnImage.sprite = soundEffectsOnInactiveSprite;
            soundEffectsOffImage.sprite = soundEffectsOffActiveSprite;
            SetTextAlpha(soundEffectsOnText, 128);  // Inactive
            SetTextAlpha(soundEffectsOffText, 255); // Active
        }
        else
        {
            soundEffectsOnImage.sprite = soundEffectsOnActiveSprite;
            soundEffectsOffImage.sprite = soundEffectsOffInactiveSprite;
            SetTextAlpha(soundEffectsOnText, 255);  // Active
            SetTextAlpha(soundEffectsOffText, 128); // Inactive
        }
    }

    private void SetTextAlpha(TextMeshProUGUI textComponent, int alpha)
    {
        Color color = textComponent.color;
        color.a = alpha / 255f;
        textComponent.color = color;
    }

    public void OnSoundEffectsOnClick()
    {
        soundEffectsMuted = false;
        Save();
        UpdateSettingsUI();
    }

    public void OnSoundEffectsOffClick()
    {
        soundEffectsMuted = true;
        Save();
        UpdateSettingsUI();
    }

    // Volume controls for BGM

    public void IncreaseVolume()
    {
        if (bgmVolume < 1.0f)
        {
            bgmVolume = Mathf.Min(bgmVolume + volumeStep, 1.0f);
            if (!muted) audioManager.BgSource.volume = bgmVolume; // Only adjust if not muted
            Save();
            UpdateVolumeBars();
        }
    }

    public void DecreaseVolume()
    {
        if (bgmVolume > 0.0f)
        {
            bgmVolume = Mathf.Max(bgmVolume - volumeStep, 0.0f);
            if (!muted) audioManager.BgSource.volume = bgmVolume; // Only adjust if not muted
            Save();
            UpdateVolumeBars();
        }
    }

    private void UpdateVolumeBars()
    {
        int activeBars = Mathf.RoundToInt(bgmVolume * maxVolumeBars); // Calculate active bars
        for (int i = 0; i < volumeBars.Length; i++)
        {
            volumeBars[i].SetActive(i < activeBars);
        }
    }
}
