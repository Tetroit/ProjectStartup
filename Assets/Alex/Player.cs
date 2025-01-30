using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IDamageable
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

    public void Select()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateHealth()
    {
        throw new System.NotImplementedException();
    }

    public void GetDamage(int amount)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyDamage(int amount)
    {
        throw new System.NotImplementedException();
    }

    public void IgnoreShield(int amount)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        throw new System.NotImplementedException();
    }
}
