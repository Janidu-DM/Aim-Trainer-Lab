using UnityEngine;

public class RecoilEffectOnGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaycastWeapon _weaponLogic;
    [SerializeField] private Transform _gunRecoilContainer;

    [Header("Recoil Settings")]
    [SerializeField] private Vector3 _kickBackAmount = new Vector3(0, 0, 0.1f);
    [SerializeField] private Vector3 _recoilRotation = new Vector3(-5f,2f,0f);
    [SerializeField] private float _recoverSpeed = 20f;
    [SerializeField] private float _snappiness = 10f;

    private Vector3 _targetPosition;
    private Vector3 _currentPosition;
    private Vector3 _targetRotation;
    private Vector3 _currentRotation;

    private void OnEnable()
    {
        if (_weaponLogic != null)
        {
            _weaponLogic.OnShoot += TriggerRecoil;
        } 
    }
    private void OnDisable()
    {
        if (_weaponLogic != null)
        {
            _weaponLogic.OnShoot -= TriggerRecoil;
        }
    }
    private void Update()
    {
        if (_gunRecoilContainer == null) return;

        //Returning Target to Vector3 gradually
        _targetPosition = Vector3.Lerp(_targetPosition, Vector3.zero, _recoverSpeed * Time.deltaTime);
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _recoverSpeed * Time.deltaTime);

        //making current position to target position snappily
        _currentPosition = Vector3.Slerp(_currentPosition, _targetPosition, _snappiness * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, _snappiness * Time.deltaTime);

        //making the container's local position and rotation to the current values
        _gunRecoilContainer.localPosition = _currentPosition;
        _gunRecoilContainer.localRotation = Quaternion.Euler(_currentRotation);

    }
    private void TriggerRecoil()
    {
        _targetPosition += new Vector3(
            Random.Range(_kickBackAmount.x , -_kickBackAmount.x),Random.Range(_kickBackAmount.y , -_kickBackAmount.y),_kickBackAmount.z);

        _targetRotation += new Vector3(
            _recoilRotation.x, Random.Range(-_recoilRotation.y, _recoilRotation.y), Random.Range(_recoilRotation.z, -_recoilRotation.z));
    }
}