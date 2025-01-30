using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField]
    int m_health;
    [SerializeField]
    int m_maxHealth;
    [SerializeField]
    int m_shield;

    [SerializeField]
    public UnityEvent<int> OnShieldChanged;

    public int health => m_health;
    public int maxHealth => m_maxHealth;

    public int shield => m_shield;

    private void OnEnable()
    {
        UpdateAll();
    }

    public void UpdateAll()
    {
        UpdateShield();
    }

    public void ApplyShield(int amount)
    {
        m_shield = Mathf.Max(0, m_shield + amount);
        UpdateShield();
    }

    public void UpdateShield()
    {
        OnShieldChanged?.Invoke(m_shield);
    }
}
