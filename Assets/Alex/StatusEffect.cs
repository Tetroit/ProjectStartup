using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    DamageOverTime,
    AttackReduction
}

public class StatusEffect
{
    public StatusEffectType type;
    public int duration; 
    public int amount;

    public StatusEffect(StatusEffectType type, int duration, int amount)
    {
        this.type = type;
        this.duration = duration;
        this.amount = amount;
    }

    public void ApplyEffect(Enemy enemy)
    {
        switch(type)
        {
            case StatusEffectType.DamageOverTime:
                enemy.GetDamage(amount);
                break;
            case StatusEffectType.AttackReduction:
                enemy.ModifyDamage(amount);
                break;
        }
    }
}