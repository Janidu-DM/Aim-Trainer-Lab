using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClip _bgmClip;
    [SerializeField] private AudioClip _uiClickClip;

    [Header("Audio Settings")]
    [SerializeField][Range(0.1f, 1f)] private float _bgmVolume;
    [SerializeField][Range(0.1f, 1f)] private float _sfxVolume;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (_bgmAudioSource == null) return;
        if (_sfxAudioSource == null) return;

        _bgmAudioSource.volume = _bgmVolume;
        _sfxAudioSource.volume = _sfxVolume;

        PlayBgmClip();
    }

    private void PlayBgmClip()
    {
        if (_bgmClip == null) return;
        if (_bgmAudioSource == null) return;
        {
            _bgmAudioSource.clip = _bgmClip;
            _bgmAudioSource.loop = true;
            _bgmAudioSource.Play();
        }
    }
    public void PlayUIClick()
    {
        if (_sfxAudioSource == null) return;
        if (_uiClickClip == null) return;

        _sfxAudioSource.volume = _sfxVolume;
        _sfxAudioSource.PlayOneShot(_uiClickClip);
    }
}
