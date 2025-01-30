using CustomEvents;
using GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class Enemy : MonoBehaviour, IDamageable
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
        m_maxHealth = UnityEngine.Random.Range(20, 1000);
        m_health = m_maxHealth;
        m_damage = UnityEngine.Random.Range(2, 10);
        m_shield = UnityEngine.Random.Range(1, 10);
        UpdateAll();
    }

    public void UpdateAll()
    {
        UpdateMaxHealth();
        UpdateHealth();
        UpdateShield();
        UpdateDamage();
    }

    void PlayHitAnimation()
    {

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("trigger");
        }
    }
    void PlayDeathAnimation()
    {
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("deathtrigger");
        }
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

    public void IgnoreShield(int value)
    {
        m_health -= value;

        UpdateHealth();
    }

    public void ApplyShield(int amount)
    {
        m_shield = Mathf.Max(0, m_shield + amount);
        UpdateShield();
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
        if (m_health <= 0)
            PlayDeathAnimation();
        else
            PlayHitAnimation();


        if (m_health <= 0)
        {
            EventBus<EnemyDestroyed>.Publish(new EnemyDestroyed(this));
        }
        OnHealthChanged?.Invoke(m_health);
    }
    public void Despawn()
    {
        Debug.Log("L{F}IP{EGWHLRIJEOKF");
        Destroy(gameObject);
    }
    public void UpdateDamage()
    {
        OnDamageChanged?.Invoke(m_damage);
    }

    public void Select()
    {
        throw new NotImplementedException();
    }
}
