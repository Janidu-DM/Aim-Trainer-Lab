using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _highScoretext;
    [SerializeField] private TMP_Text _comboText;
    [SerializeField] private TMP_Text _timeLeftText;
    [SerializeField] private TMP_Text _targetHitText;
    [SerializeField] private TMP_Text _fireModeText;
    [SerializeField] private TMP_Text _playerInteractorText;

    [Header("Pause Menu Elements")]
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private Button _startResumeButton;
    [SerializeField] private Button _resetPlayerButton;
    [SerializeField] private Button _savePlayerButton;
    [SerializeField] private Button _quitFromPauseButton;
    [SerializeField] private Slider _bulletSpreadSlider;
    [SerializeField] private Slider _sensetivitySlider;
    [SerializeField] private Slider _recoilMultiplierSlider;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;

    private PlayerController _playerController;
    private RaycastWeapon _weapon;
    private PlayerInteractor _playerInteractor;

    private void Start()
    {
        GameManager.Instance.OnPauseStateChanged += HandleOnPauseStateChange; // since when OnEnable which runs with awawk , Singleton is not prepared
        GameManager.Instance.OnNewHighScore += HandleNewHighScore;
        HandleNewHighScore(GameManager.Instance.HighScore); // run only start

        _bulletSpreadSlider.value = GameManager.Instance.BulletSpreadMultiplier;
        _sensetivitySlider.value = GameManager.Instance.SensetivityMultiplier;
        _recoilMultiplierSlider.value = GameManager.Instance.RecoilMultiplier;

        _startResumeButton.onClick.AddListener(() => GameManager.Instance.ToggleGameState());
        _quitFromPauseButton.onClick.AddListener(() => QuitGameFromMenu());
        _resetPlayerButton.onClick.AddListener(() => ResetPlayerPref());
        _savePlayerButton.onClick.AddListener(() => GameManager.Instance.SavePlayerPref());
        _bulletSpreadSlider.onValueChanged.AddListener(OnBulletSpreadValueChanged);
        _sensetivitySlider.onValueChanged.AddListener(OnSensetivityValueChanged);
        _recoilMultiplierSlider.onValueChanged.AddListener(OnRecoilMultiplierValueChanged);

        //MainMenu
        _startButton.onClick.AddListener(StartGameFromMenu);
        _quitButton.onClick.AddListener(QuitGameFromMenu);

        _mainMenuPanel.SetActive(true);
        _pauseMenuPanel.SetActive(false);
        _hudPanel.SetActive(false); 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;





        _playerController = FindAnyObjectByType<PlayerController>();
        if (_playerController != null)
        {

            _weapon = _playerController.GetActiveWeapon();
            if (_weapon != null)
            {
                _weapon.OnFireModeChanged += _weapon_OnFireModeChanged;
                _weapon_OnFireModeChanged(_weapon.CurrentFireMode); // only in the start we force to, otherwise firemode initial wil be inacurate 
            }

            _playerInteractor = _playerController.GetPlayerInteractor();
            if (_playerInteractor != null)
            {
                _playerInteractor.OnInteractableHovered += Handle_OnInteractableHovered;
            }
        }
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnPauseStateChanged -= HandleOnPauseStateChange;
        _weapon.OnFireModeChanged -= _weapon_OnFireModeChanged;
        GameManager.Instance.OnNewHighScore -= HandleNewHighScore;
        _playerInteractor.OnInteractableHovered -= Handle_OnInteractableHovered;
    }
    private void Handle_OnInteractableHovered(string interactorText)
    {
        if (string.IsNullOrEmpty(interactorText))
        {
            _playerInteractorText.gameObject.SetActive(false);
        }
        else
        {
            _playerInteractorText.gameObject.SetActive(true);
            _playerInteractorText.text = $"[E] {interactorText}";
        }
    }


    private void HandleOnPauseStateChange(bool isPaused) 
    {
       // if (_mainMenuPanel.activeSelf) return; //in main menu either panels are not active

        SetUIVisibility(isPaused);
        if (isPaused)
        {
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void HandleNewHighScore(int newHighScore)
    {
        _highScoretext.text = $"High Score : {GameManager.Instance.HighScore}";
    }

  

    private void _weapon_OnFireModeChanged(RaycastWeapon.FireMode newMode)
    {
        _fireModeText.text = newMode.ToString().ToUpper();
    }

    private void Update()
    {
        if (GameManager.Instance.IsRoundActive) 
        {
            UpdateHUD();
        }
    }

    private void SetUIVisibility(bool _isPaused) 
    {
        _hudPanel.gameObject.SetActive(!_isPaused);
        _pauseMenuPanel.gameObject.SetActive(_isPaused);
    }
    private void UpdateHUD()
    {
        _comboText.text = $"combo : {GameManager.Instance.CurrentCombo}"; 
        _scoreText.text = $"score : {GameManager.Instance.CurrentScore}";
        _timeLeftText.text = $"T- : {Mathf.FloorToInt(GameManager.Instance.RemainingTime)}";
        _targetHitText.text = $"Hits : {GameManager.Instance.TargetHits}";
        


    }

    private void OnBulletSpreadValueChanged(float newValue) 
    {
        GameManager.Instance.SetSpreadMultiplier(newValue);
    }
    private void OnSensetivityValueChanged(float newValue) 
    {
        GameManager.Instance.SetSensitivityMultiplier(newValue);
    }
    private void OnRecoilMultiplierValueChanged(float newValue) 
    { 
        GameManager.Instance.SetRecoilMultiplier(newValue); 
    }
    private void ResetPlayerPref()
    {
        GameManager.Instance.ResetPlayerPref();

        _bulletSpreadSlider.value = GameManager.Instance.BulletSpreadMultiplier;
        _sensetivitySlider.value = GameManager.Instance.SensetivityMultiplier;
        _recoilMultiplierSlider.value = GameManager.Instance.RecoilMultiplier;

        GameManager.Instance.ResetPlayerPref();
    }

    private void StartGameFromMenu()
    {
        Debug.Log("ClickedOntheStart");
        _mainMenuPanel.SetActive(false);
        GameManager.Instance.SetIsGameActive(true);
        GameManager.Instance.ToggleGameState();
    }

    private void QuitGameFromMenu()
    {
        Debug.Log("Quitting From Aim Trainer");
        Application.Quit();

    }
}
