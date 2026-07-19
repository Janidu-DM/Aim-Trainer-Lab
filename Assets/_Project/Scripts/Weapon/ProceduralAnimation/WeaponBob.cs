using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _weaponBobContainer;
    [SerializeField] private PlayerMovementPhysics _movementPhysics;

    [Header("Weapon Bob Settings")]
    [SerializeField] private float _horizontalAmplitude = 0.05f;
    [SerializeField] private float _verticalAmplitude = 0.05f;
    [SerializeField] private float _bobFrequency = 15f;

    [Tooltip("Bob Smoothing")]
    [SerializeField] private float _returnSpeed = 10f;

    [Header("Bob Dynamic Scaling")]
    [Tooltip("The expected normal walking speed of the player. Used to normalize bob speed.")]
    [SerializeField] private float _baseWalkSpeed = 5f;
    [SerializeField] private float _minBobMultiplier = 1f;
    [SerializeField] private float _maxBobMultiplier = 2f;

    
    private Vector3 _defaultPosition;
    private float _bobTimer;

    private void Start()
    {
        _defaultPosition = transform.localPosition;

        if (_movementPhysics == null)
        {
            _movementPhysics = GetComponentInParent<PlayerMovementPhysics>();
        }
    }
    private void Update()
    {
        if (GameManager.Instance.IsPaused || _movementPhysics == null)
        {
            
            return;
        }

        bool isActuallyMoving = _movementPhysics.CurrentSpeed > 0.1f;
        

        if (isActuallyMoving)
        {
            
            float speedMultipler = Mathf.Clamp(_movementPhysics.CurrentSpeed / _baseWalkSpeed, _minBobMultiplier, _maxBobMultiplier); // ususal walking is 5=basewalkspeed usually in unity.so we divide the speed by that value to normalize.so the bob happens 1s per 5 meters , when running it increase to 2 at max

            _bobTimer += Time.deltaTime * speedMultipler * _bobFrequency;

            float horizontalOffset = Mathf.Cos(_bobTimer) * _horizontalAmplitude;
            float verticalOffset = Mathf.Sin(_bobTimer * 2f) * _verticalAmplitude; //here we multiply by two to mimic the natural movement.where one horizontal move takes two verticle move time

            Vector3 targetPosition = _defaultPosition + new Vector3(horizontalOffset, verticalOffset, 0f);
            _weaponBobContainer.localPosition = Vector3.Lerp(_weaponBobContainer.localPosition, targetPosition, Time.deltaTime * _returnSpeed);
        }
        else
        {
            
            _bobTimer = 0f;
            _weaponBobContainer.localPosition = Vector3.Lerp(_weaponBobContainer.localPosition, _defaultPosition, Time.deltaTime * _returnSpeed);
        }
    }
}
