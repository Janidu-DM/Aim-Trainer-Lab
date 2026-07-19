using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IPlayerInput _input;
    private PlayerMovementPhysics _movementPhysics;
    private PlayerLook _playerLook;
    private PlayerInteractor _playerInteractor;

    [Header("The weapon container")]
    [SerializeField] private RaycastWeapon _weaponInstance;
    [SerializeField] private WeaponEffect _weaponEffect;
    private void Awake()
    {
        _input = GetComponent<IPlayerInput>();
        _movementPhysics = GetComponent<PlayerMovementPhysics>();
        _playerLook = GetComponent<PlayerLook>();
        _playerInteractor = GetComponent<PlayerInteractor>();
    }
    private void Update()
    {
        if (_input.GetPauseInputDown())
        {
            if (GameManager.Instance.IsGameActive)
            {
                GameManager.Instance.ToggleGameState();
            }
        }
        if (GameManager.Instance.IsPaused || !GameManager.Instance.IsGameActive) return; //returning since pausing should not involve player controll furthermore


        Vector2 movementInput = _input.GetMovementInput();

        Vector2 lookInput = _input.GetLookInput();

        _movementPhysics.Move(movementInput);
        _playerLook.Look(lookInput);

        if (_input.GetJumpInputDown())
        {
            _movementPhysics.Jump();
        }
        if (_input.GetInteractInputDown())
        {
            _playerInteractor.TryInteract();
        }
        if (_input.GetShootInputToggle())
        {
            _weaponInstance.ToggleFiremode();
        }

        bool shouldFire = false;

        if (_weaponInstance.CurrentFireMode == RaycastWeapon.FireMode.FullAuto)
        {
            shouldFire = _input.GetShootInputHold();
        }
        else if(_weaponInstance.CurrentFireMode == RaycastWeapon.FireMode.Burst || _weaponInstance.CurrentFireMode == RaycastWeapon.FireMode.SemiAuto)
        {
            shouldFire |= _input.GetShootInputDown();
        }
        if (shouldFire && _weaponInstance != null)
        {
            _weaponInstance.TryShoot();
        }
        if (_weaponEffect != null)
        {
            _weaponEffect.TryADS(_input.GetADSInputDown());
            _weaponInstance.SetIsAdsHolding(_input.GetADSInputDown());
        }


    }
    public RaycastWeapon GetActiveWeapon()
    {
        return _weaponInstance == null ? null : _weaponInstance;
    }
    public PlayerInteractor GetPlayerInteractor()
    {
        return _playerInteractor == null ? null : _playerInteractor;
    }
}
