using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    [Header("Aim Trainer Settings")]
    [SerializeField][Range(0.1f, 5f)] private float _sensitivityMultiplier = 1.0f;
    [SerializeField][Range(0f, 5f)] private float _recoilMultiplier = 1.0f;
    [SerializeField][Range(0f, 5f)] private float _bulletSpreadMultiplier = 1.0f;

    [Header("Round Settings")]
    [SerializeField] private float _roundDuration = 30f;
    [SerializeField] private float _roundPreWarmDuration = 1.0f;
    private bool _isRoundActive;
    private float _remainingTime;


    [Header("Game State")]
    [SerializeField] private int _maxCombo = 5;
    [SerializeField] private int _currentScore = 0;
    [SerializeField] private int _currentCombo = 1;
    [SerializeField] private int _targetHits = 0;
    private int _highScore;

    private bool _isGameActive = false;
    private bool _isRoundWarmPhase = false;
    //to UI and spawner listen
    public event Action OnRoundPreWarm;
    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action<int> OnNewHighScore;

    //Game Pause
    private bool _isPaused = true;
    public event Action<bool> OnPauseStateChanged;
    //Read-Only properties to Access private values (exactly like getters but its the better practice and ofcourse properties like a spcialize wrapper for getter/setters)
    public float SensetivityMultiplier => _sensitivityMultiplier;
    public float RecoilMultiplier => _recoilMultiplier;
    public float BulletSpreadMultiplier => _bulletSpreadMultiplier;
    public int CurrentScore => _currentScore;
    public int HighScore => _highScore;
    public int CurrentCombo => _currentCombo;
    public bool IsRoundActive => _isRoundActive;
    public float RemainingTime => _remainingTime;
    public float RoundDuration => _roundDuration;
    public int TargetHits => _targetHits;
    public bool IsPaused => _isPaused;
    public bool IsGameActive => _isGameActive;
    public bool IsRoundPreWarm => _isRoundWarmPhase;

    private const string PlayerHighScoreKey = "PlayerHighScore"; //making this won't accidently mistype with strings 
    private const string PlayerSensitivityKey = "PlayerSenseMultiplier"; //making this won't accidently mistype with strings 
    private const string PlayerBulletSpreadKey = "PlayerBulletSpreadMultiplier"; //making this won't accidently mistype with strings 
    private const string PlayerRecoilKey = "PlayerRecoilMultiplier"; //making this won't accidently mistype with strings 
   
    private void Awake()
    {
        _isPaused = true;
        Time.timeScale = 0.0f;
        _isGameActive = false;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); //stay without destroying when loading levels
        

        _highScore = PlayerPrefs.GetInt(PlayerHighScoreKey, 0);
        _sensitivityMultiplier = PlayerPrefs.GetFloat(PlayerSensitivityKey, 1);
        _recoilMultiplier = PlayerPrefs.GetFloat(PlayerRecoilKey, 0);
        _bulletSpreadMultiplier = PlayerPrefs.GetFloat(PlayerBulletSpreadKey, 0);
    }

    private void Update()
    {
        if (_isRoundActive)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0f)
            {
                EndRound();
            }
        }
    }
    private void EndRound()
    {
        _isRoundActive = false;
        _remainingTime = 0f;
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            PlayerPrefs.SetInt(PlayerHighScoreKey, _highScore);
            PlayerPrefs.Save();
            OnNewHighScore.Invoke(_highScore);
            
        }
        OnRoundEnd?.Invoke();
        Debug.Log($"Round Ended! final score: {_currentScore}");

    }

    public void TryStartRound()
    {
        if (!_isRoundWarmPhase && !_isRoundActive)
        {
            StartCoroutine( StartRound() );
        }
    }
    private IEnumerator StartRound()
    {
        _isRoundWarmPhase = true;
        OnRoundPreWarm?.Invoke();

        yield return new WaitForSeconds(_roundPreWarmDuration);

        

        _remainingTime = _roundDuration;
        _isRoundActive = true;
        _currentCombo = 1;
        _currentScore = 0;
        _targetHits = 0;
        _isRoundWarmPhase = false;
        OnRoundStart?.Invoke();
        Debug.Log("Round Started go go go");
        
    }

    //from UI Sliders to change
    public void SetSensitivityMultiplier(float multiplier) => _sensitivityMultiplier = multiplier;
    public void SetRecoilMultiplier(float multiplier) => _recoilMultiplier = multiplier;
    public void SetSpreadMultiplier(float multiplier) => _bulletSpreadMultiplier = multiplier;


    //from target system to 
    public void AddScore(int baseScores)
    {
        if (!_isRoundActive) return;

        _currentScore += _currentCombo * baseScores;
        _targetHits++;
    }
    public void ResetCombo()
    {
        _currentCombo = 1;
    }
    public void ResetPlayerPref()
    {
        _bulletSpreadMultiplier = 0f;
        _sensitivityMultiplier = 1f;
        _recoilMultiplier = 0f;

        SavePlayerPref();
    }
    public void SavePlayerPref()
    {
        PlayerPrefs.SetFloat(PlayerRecoilKey, _recoilMultiplier);
        PlayerPrefs.SetFloat(PlayerBulletSpreadKey, _bulletSpreadMultiplier);
        PlayerPrefs.SetFloat(PlayerSensitivityKey, _sensitivityMultiplier);
        PlayerPrefs.Save();
    }
    public void IncrementCombo()
    {
        if (_currentCombo < _maxCombo)
        {
            _currentCombo++;
        }
    }
    public void ToggleGameState()
    {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;
        OnPauseStateChanged?.Invoke(_isPaused);
        
    }
    public void SetIsGameActive(bool state)
    {
        _isGameActive = state;
    }
}
