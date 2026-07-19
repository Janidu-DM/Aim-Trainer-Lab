using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;

public class WeaponEffect : MonoBehaviour
{
    [Header("Weapon Effect References")]
    [SerializeField] private ParticleSystem _muzzleFlashParticleSys;
    [SerializeField] private ImpactFX _impactParticleSys;
    [SerializeField][Range(5,100)] private int _impactParticlePoolSize = 30;

    [Header("Bullet Tracer References")]
    [SerializeField] private BulletTracer _bulletTracerPrefab;
    [SerializeField][Range(5,100)] private int _bulletTracerPoolSize = 30;
    [SerializeField] private Transform _shootingPoint;

    [Header("Aim Donw Sight Settings")]
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private Transform _weaponMeshContainer;
    [SerializeField] private Vector3 _hipFirePosition;
    [SerializeField] private Vector3 _adsPosition;
    [SerializeField] private float _adsSpeed = 12f;

    [Header("Camera Zoom Settings")]
    [SerializeField] private float _hipFireFOV = 60f;
    [SerializeField] private float _adsFOV = 45f;
    [SerializeField] private Camera _playerCamera;

    private RaycastWeapon _weaponLogic;
    private ComponentPool<ImpactFX> _impactParticleSystemPool;
    private ComponentPool<BulletTracer> _bulletTracerPrefabPool;

    public event Action<bool> OnADS;
    private void Awake()
    {
        _weaponLogic = GetComponent<RaycastWeapon>();

        if (_impactParticleSys != null) 
        {
            _impactParticleSystemPool = new ComponentPool<ImpactFX>(_impactParticleSys , _impactParticlePoolSize );
        }

        if (_bulletTracerPrefab != null) 
        {
            _bulletTracerPrefabPool = new ComponentPool<BulletTracer>(_bulletTracerPrefab, _bulletTracerPoolSize);
        }
    }

    public void TryADS(bool Aiming)
    {
        if (_adsPosition != null && _weaponMeshContainer != null) 
        {
            ExecuteADS(Aiming);
        }
        
    }
    private void ExecuteADS(bool Aiming)
    {
        OnADS?.Invoke(Aiming);
        Vector3 targetPosition = Aiming ? _adsPosition : _hipFirePosition;
        float targetFOV = Aiming ? _adsFOV : _hipFireFOV;
        _crosshair.gameObject.SetActive(!Aiming);

        _weaponMeshContainer.localPosition = Vector3.Lerp(_weaponMeshContainer.localPosition, targetPosition, _adsSpeed * Time.deltaTime);
        _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, targetFOV, _adsSpeed * Time.deltaTime);
        
    }
    private void OnEnable()
    {
        if (_weaponLogic != null)
        {
            _weaponLogic.OnShoot += PlayMuzzleFlash;
            _weaponLogic.OnHit += PlayBulletTracer;
            _weaponLogic.OnHit += PlayImpactFx;
            
        }
    }

    private void OnDisable()
    {
        if (_weaponLogic != null)
        {
            _weaponLogic.OnShoot -= PlayMuzzleFlash;
            _weaponLogic.OnHit -= PlayBulletTracer;
            _weaponLogic.OnHit -= PlayImpactFx;
        }
    }


    private void PlayMuzzleFlash()
    {
        if (_muzzleFlashParticleSys != null)
        {
            _muzzleFlashParticleSys.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            _muzzleFlashParticleSys.Play();
        }
    }
    private void PlayBulletTracer(RaycastHit hit)
    {
        if (_bulletTracerPrefabPool == null && _shootingPoint == null)
        {
            return; 
        }
        BulletTracer bulletTracerInstance = _bulletTracerPrefabPool.GetInstanceFromPool(_shootingPoint.position, Quaternion.identity);
        bulletTracerInstance.InitiateFlight(_shootingPoint.position,hit.point,(invokedTracer) =>
                                                                                                _bulletTracerPrefabPool.ReturnInstanceToPool(invokedTracer));


    }
    private void PlayImpactFx(RaycastHit hit)
    {
        if (_impactParticleSystemPool != null)
        {
            ImpactFX impactParticleSystemInstance = _impactParticleSystemPool.GetInstanceFromPool(hit.point,Quaternion.identity);
            impactParticleSystemInstance.InitilizeImpact(hit,(invokedImpact) => 
                                                                                _impactParticleSystemPool.ReturnInstanceToPool(invokedImpact) );
        }
    }

}
