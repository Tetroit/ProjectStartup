using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using System;

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
    [SerializeField]
    public UnityEvent<int> OnHealthChanged;

    private List<StatusEffect> activeEffects = new List<StatusEffect>();

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
        UpdateHealth();
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
    public void UpdateHealth()
    {
        OnShieldChanged?.Invoke(m_shield);
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

    public void ProcessEffects()
    {
        foreach (var effect in activeEffects.ToList())
        {
            effect.ApplyEffect(this);
            effect.duration--;
        }

        activeEffects.RemoveAll(e => e.duration <= 0);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        activeEffects.Add(effect);
    }

    public void Select()
    {
        throw new System.NotImplementedException();
    }

    public void ModifyDamage(int amount)
    {
        throw new Exception("u gay");
    }

    public void IgnoreShield(int amount)
    {
        m_health -= amount;
        UpdateHealth();
    }
}
