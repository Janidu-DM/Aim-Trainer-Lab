using UnityEngine;

public class ScoringHandler : MonoBehaviour
{
    private RaycastWeapon _weapon;
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();

        if (_playerController != null)
        {
            _weapon = _playerController.GetActiveWeapon();

            if (_weapon != null) {
                _weapon.OnHit += HandleWeaponHit;
                _weapon.OnMiss += HandleWeaponMiss;
            }
        }
    }
    private void OnDestroy()
    {
        if (_weapon != null)
        {
            _weapon.OnHit -= HandleWeaponHit;
            _weapon.OnMiss -= HandleWeaponMiss;
        }
    }
    private void HandleWeaponHit(RaycastHit hit)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRoundActive) return;
        
        AimTarget target = hit.collider.GetComponent<AimTarget>();

        if (target != null) //mean we have hit a target
        {
            GameManager.Instance.AddScore(5);
            GameManager.Instance.IncrementCombo();
        }
        else
        {
            HandleWeaponMiss(); //shot hit not in a target so combo resets
        }
        
    }
    private void HandleWeaponMiss()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRoundActive) return;

        GameManager.Instance.ResetCombo();
    }


}
