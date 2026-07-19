using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAudio : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlayClickAudio);
    }
    private void OnDestroy()
    {
        if (_button == null) return;
        _button.onClick.RemoveListener(PlayClickAudio);
    }

    private void PlayClickAudio()
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.PlayUIClick();
    }
}
