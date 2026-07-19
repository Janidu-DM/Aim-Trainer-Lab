using System;
using UnityEngine;

public class AimTarget : MonoBehaviour , IDamageble
{
    [Header("Aim Target Settings")]
    [SerializeField] private float _maxHealth = 10f;
    
    private Action<AimTarget> _OnDeathCallBack;
    private float _currentHealth;

    public void Initialize(Action<AimTarget> OnDeath)
    {
        _currentHealth = _maxHealth;
        _OnDeathCallBack = OnDeath;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0f)
        {
            Die();
        }

    }
    private void Die() 
    {
        _OnDeathCallBack?.Invoke(this);
    }
}
