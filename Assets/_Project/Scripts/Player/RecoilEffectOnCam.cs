using UnityEngine;
public class RecoilEffectOnCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaycastWeapon _weaponLogic;
    [SerializeField] private Transform _camRecoilContainer;
    [SerializeField] private PlayerLook _playerLook;

    [Header("Recoil Settings")]
    [SerializeField] private Vector3 _recoilRotation = new Vector3(-2f,2f,0);
    [SerializeField] private float _snappiness = 15f;
    [SerializeField] private float _recoverSpeed = 10f;
    [SerializeField] private float _permenantRecoilClimb = 1.5f;
    

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

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
    private  void TriggerRecoil()
    {
        float globalRecoilMultiplier = GameManager.Instance.RecoilMultiplier;
        _targetRotation += new Vector3(
                                        _recoilRotation.x ,
                                        Random.Range(-_recoilRotation.y,_recoilRotation.y) ,
                                        Random.Range(-_recoilRotation.z, _recoilRotation.z)
                                       ) * globalRecoilMultiplier;
        if (_playerLook != null)
        {
            _playerLook.InjectRecoil(_permenantRecoilClimb * globalRecoilMultiplier, 0);
        }
    }
    private void Update()
    {
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _recoverSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation,_targetRotation,_snappiness * Time.deltaTime);
        _camRecoilContainer.localRotation = Quaternion.Euler(_currentRotation);
    }

}