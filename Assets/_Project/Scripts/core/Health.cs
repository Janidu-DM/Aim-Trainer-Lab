using System;
using UnityEngine;

public class Health : MonoBehaviour , IDamageble
{

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private bool _debug = false;

    private float _currentHealth;

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
    public void TakeDamage(float damageAmount) 
    {
        if (_currentHealth <= 0f)
        {
            return; //already Dead
        }
        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Max(0f, _currentHealth);
        OnHealthChanged?.Invoke(_currentHealth);

        if (_debug) 
        {
            Debug.Log($"{gameObject.name} got {damageAmount} of damage. Remaining health : {_currentHealth}");
        }

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }
    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
