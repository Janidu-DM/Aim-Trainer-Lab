using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ScoreBoardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _timerBarFillImage;
    [SerializeField] private TextMeshPro _liveScoreTextmesh;
    [SerializeField] private TextMeshPro _allTimeHighTextmesh;
    [SerializeField] private TextMeshPro _lastRoundScoreTextmesh;


    [Header("audio clips references")]
    [SerializeField] private AudioClip _roundStartClip;
    [SerializeField] private AudioClip _roundEndClip;
    [SerializeField][Range(0.1f, 1f)] private float _effectVolume = 0.5f;

    private AudioSource _audioSource;
    private bool _isTimerStarted;
    private float _roundDuration;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnRoundPreWarm += Handle_OnRoundPreWarm;
        GameManager.Instance.OnRoundStart += Handle_OnRoundStart;
        GameManager.Instance.OnRoundEnd += Handle_OnRoundEnd;
        GameManager.Instance.OnNewHighScore += Handle_OnNewHighScore;

        //intial 
        if (_allTimeHighTextmesh != null) 
        {
            _allTimeHighTextmesh.text = $"All Time High Score : {GameManager.Instance.HighScore.ToString("0000")}";
        }

        _roundDuration = GameManager.Instance.RoundDuration;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsRoundActive && _isTimerStarted)
        {
            _liveScoreTextmesh.text = $"{GameManager.Instance.CurrentScore.ToString("0000")}";

            RunTimerBarEffect();

        }
    }
    private void Handle_OnNewHighScore(int obj)
    {
        if (_allTimeHighTextmesh != null)
        {
            _allTimeHighTextmesh.text = $"All Time High Score : {GameManager.Instance.HighScore.ToString("0000")}";
        }
    }

    private void Handle_OnRoundEnd()
    {
        if (_roundEndClip != null) 
        {
            _audioSource.PlayOneShot(_roundEndClip,_effectVolume);
        }
        _isTimerStarted = false;
        _liveScoreTextmesh.text = "START";
        _lastRoundScoreTextmesh.text = $"Last Round Score : {GameManager.Instance.CurrentScore.ToString("0000")}";
    }

    private void Handle_OnRoundStart()
    {

        _isTimerStarted = true;
        _liveScoreTextmesh.text = "";
        
    }

    private void Handle_OnRoundPreWarm()
    {
        if (_roundStartClip != null)
        {
            _audioSource.PlayOneShot(_roundStartClip, _effectVolume);
        }
        _liveScoreTextmesh.text = "FIRE";
        _roundDuration = GameManager.Instance.RoundDuration;
        
    }

    private void RunTimerBarEffect()
    {
        if (_timerBarFillImage != null)
        {
            float remainingTimeToFillAmount = GameManager.Instance.RemainingTime / _roundDuration;
            _timerBarFillImage.fillAmount = remainingTimeToFillAmount;
            _timerBarFillImage.color = Color.Lerp(Color.red, Color.green, remainingTimeToFillAmount);
        }
    }
}
