using System;
using System.Collections;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public enum FireMode
    {
        SemiAuto,
        Burst,
        FullAuto
    }
    [Header("Gun FireMode Settings")]
    [SerializeField] private bool _allowModeSwitching = false;
    [SerializeField] private FireMode _currentFireMode = FireMode.SemiAuto;
    [SerializeField] private int _burstCount = 3;
    [SerializeField] private float _burstDelay = 0.08f;
    private bool _isFiringBurst = false;

    public FireMode CurrentFireMode => _currentFireMode;



    [Header("Shooting Settings")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private float _fireRange = 100f;
    [SerializeField][Range(1 , 10000)] private float _impactForce = 10f;
    [SerializeField] private float _baseBulletSpreadFactor = 0.08f; // to display the mechanical inaccuracy of the gun 
 

    [Header("References")]
    [SerializeField] private Transform _shootingPointCameraMid;
    

    public event Action OnShoot;
    public event Action<RaycastHit> OnHit;
    public event Action OnMiss;
    public event Action<FireMode> OnFireModeChanged;
    private float _currentBulletSpreadFactor;
    private float _nextTimeToFire = 0f;
    public void Start()
    {
        _currentBulletSpreadFactor = _baseBulletSpreadFactor;
    }

    public void ToggleFiremode()
    {
        if (!_allowModeSwitching) return;

        int totalFireModes = Enum.GetValues(typeof(FireMode)).Length;
        _currentFireMode = (FireMode)(((int)_currentFireMode + 1)%totalFireModes); //cycling through firemodes ; modulo helps to have a boundary
        OnFireModeChanged?.Invoke(_currentFireMode);
        Debug.Log(CurrentFireMode);
    }
    public void TryShoot()
    {
        if (_isFiringBurst) return; // ignore mouse clicks if already burst fire is ongoing

        if (Time.time >= _nextTimeToFire)
        {
            _nextTimeToFire = Time.time + _fireRate ;
            
            if (_currentFireMode == FireMode.Burst)
            {
                StartCoroutine(BurstFireRoutine());
            }
            else
            {
                ExecuteShot(); //both auto and single fires one shot per request from controller
            }
        }
    }
    private IEnumerator BurstFireRoutine() //on request fires {burstcount} shots meantime locking the firing by ignoring clicks
    {
        _isFiringBurst = true; //locking trigger

        for(int i = 0;i < _burstCount; i++)
        {
            ExecuteShot();
            yield return new WaitForSeconds(_burstDelay);
        }
        _isFiringBurst = false; //unlocking
    }
    private void ExecuteShot()
    {
        OnShoot?.Invoke();

        RaycastHit hit;
        Ray ray = new Ray(_shootingPointCameraMid.position,CalculateShootingDirection());
        if (Physics.Raycast(ray, out hit, _fireRange))
        {
            OnHit?.Invoke(hit);


            if (hit.rigidbody != null) //AddingImpactForceto Rigidbody props to have fun
            {
                hit.rigidbody.AddForceAtPosition(-hit.normal * _impactForce, hit.point, ForceMode.Impulse); //more realistic
                //hit.rigidbody.AddForce(-hit.normal * _impactForce); //more cheap
            }


            IDamageble damageble = hit.collider.GetComponent<IDamageble>();
            if (damageble != null)
            {
                damageble.TakeDamage(_damage);
            }
            Debug.Log("Hit " + hit.transform.name);

        }
        else
        {
            OnMiss?.Invoke(); //probably to sky
        }
    }
    private Vector3 CalculateShootingDirection()
    {
        Vector3 shootingDirection;
        Vector3 spreadOffset;

        spreadOffset = UnityEngine.Random.insideUnitSphere * _currentBulletSpreadFactor * GameManager.Instance.BulletSpreadMultiplier ;
        shootingDirection = (_shootingPointCameraMid.forward + spreadOffset).normalized;
        return shootingDirection;
    }
    public void SetShootingSpreadMultiplier(float multiplier) //for future -- running and walking should have influence in shooting spread;
    {
        _currentBulletSpreadFactor = _baseBulletSpreadFactor * multiplier;
    }
    public void SetIsAdsHolding(bool adsHolding) 
    {
        _currentBulletSpreadFactor =  (adsHolding)? 0f : _baseBulletSpreadFactor;  
    }

}
