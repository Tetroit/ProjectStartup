using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostRay : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(Enemy enemy, int amount)
    {
        int adjustedDamage = Mathf.CeilToInt(amount);
        enemy.GetDamage(adjustedDamage);
        enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.AttackReduction, statusEffectDuration, statusEffectAmount));
        Debug.Log($"This did: {adjustedDamage} FrostRay Damage");
    }
}
