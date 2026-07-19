using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ParticleSystem))]
public class TargetExplosionFX : MonoBehaviour
{
    [Header("Target Explosion audio clip")]
    [SerializeField] private AudioClip _ballHitSFXclip;
    [Tooltip("Pitch randomness helps to ear fatigue of the player")]
    [SerializeField] private float _pitchRandomness = 0.1f;
    [SerializeField] private float _audioVolume = 0.5f;

    private AudioSource _audioSource;
    private ParticleSystem _targetExplosionParticleSystem;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _targetExplosionParticleSystem = GetComponent<ParticleSystem>();
    }

    public void InitializeExplosion( Action<TargetExplosionFX> OnComplete)
    {
        PlayHitSound();
        _targetExplosionParticleSystem.Clear(true);
        StartCoroutine(PlayExplosionFX(OnComplete));
    } 
    private IEnumerator PlayExplosionFX(Action<TargetExplosionFX> OnComplete)
    {
        _targetExplosionParticleSystem.Play();
        yield return new WaitForSecondsRealtime(_targetExplosionParticleSystem.main.duration);
        OnComplete?.Invoke(this);
    }
    private void PlayHitSound()
    {
        if (_audioSource == null || _ballHitSFXclip == null) return;

        _audioSource.pitch = 1f + Random.Range(-_pitchRandomness, _pitchRandomness);
        _audioSource.PlayOneShot(_ballHitSFXclip, _audioVolume);
    }
}
