using GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class Enemy : MonoBehaviour
{

    [SerializeField]
    int m_health;
    [SerializeField]
    int m_maxHealth;
    [SerializeField]
    int m_damage;
    [SerializeField]
    int m_shield;


    public int health => m_health;
    public int maxHealth => m_maxHealth;
    public int damage => m_damage;
    public int shield => m_shield;

    [SerializeField]
    public UnityEvent<int> OnHealthChanged;
    [SerializeField]
    public UnityEvent<int> OnMaxHealthChanged;
    [SerializeField]
    public UnityEvent<int> OnShieldChanged;
    [SerializeField]
    public UnityEvent<int> OnDamageChanged;

    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    private void OnEnable()
    {
        UpdateAll();
    }

    public void UpdateAll()
    {
        UpdateMaxHealth();
        UpdateHealth();
        UpdateShield();
        UpdateDamage();
    }

    public void GetDamage(int value)
    {
        if (m_shield > 0)
        {
            m_shield -= Math.Min(value, m_shield);
            value -= Math.Min(value, m_shield);
        }
        if (value == 0)
            return;
        m_health -= value;
        UpdateShield();
        UpdateHealth();
    }

    public void ModifyDamage(int amount)
    {
        m_damage = Mathf.Max(0, m_damage + amount);
        UpdateDamage();
    }

    public void ProcessEffects()
    {
        foreach(var effect in activeEffects.ToList())
        {
            effect.ApplyEffect(this);
            effect.duration--;
        }

        // Remove expired effects
        activeEffects.RemoveAll(e => e.duration <= 0);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        activeEffects.Add(effect);
    }

    public void UpdateShield()
    {
        OnShieldChanged?.Invoke(m_shield);
    }
    public void UpdateMaxHealth()
    {
        OnMaxHealthChanged?.Invoke(m_maxHealth);
    }
    public void UpdateHealth()
    {
        OnHealthChanged?.Invoke(m_health);
    }
    public void UpdateDamage()
    {
        OnDamageChanged?.Invoke(m_damage);
    }
}
