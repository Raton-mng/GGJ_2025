using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private GameObject _healthPoint;
    [SerializeField] private int _maxHealth;
    private GameObject[] _health;
    private int _currentHealth;
    [SerializeField] private float _invulnerabiltyTime;
    private float _lastDamage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _health = new GameObject[_maxHealth];
        _currentHealth = 1;
        _lastDamage = 0;
        for(int i = 0; i < _maxHealth; i++)
        {
            _health[i] = Instantiate(_healthPoint, transform);
            _health[i].SetActive(false);
        }
        _health[_currentHealth - 1].SetActive(true);
    }

    private void Update()
    {
        _lastDamage += Time.deltaTime;
    }

    public bool TakeDamage()
    {
        if (_lastDamage > _invulnerabiltyTime)
        {
            _lastDamage = 0;
            _currentHealth--;
            _health[_currentHealth].SetActive(false);
            if(_currentHealth == 0)
            {
                return true;
            }
        }
        return false;
    }

    public void AddHealth()
    {
        if(_currentHealth < _maxHealth)
        {
            _health[_currentHealth].SetActive(true);
            _currentHealth++;
        }
    }
}
