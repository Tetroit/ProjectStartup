using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDart : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(Enemy enemy, int amount)
    {
        int adjustedDamage = Mathf.CeilToInt(amount);
        enemy.GetDamage(adjustedDamage);
        // Apply status effect for X amount of turns
        enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.DamageOverTime, statusEffectDuration, statusEffectAmount));
        Debug.Log($"This did: {adjustedDamage} SlingShot Damage");
    }
}
