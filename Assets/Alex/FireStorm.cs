using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(Enemy enemy, int amount)
    {
        int adjustedDamage = Mathf.CeilToInt(amount * 0.5f);
        enemy.GetDamage(redoEffect ? adjustedDamage * 2 : adjustedDamage);
        enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.DamageOverTime,redoEffect ? statusEffectDuration * 2 : statusEffectDuration
            ,redoEffect ? statusEffectAmount * 2 : statusEffectAmount));
        redoEffect = false;
        Debug.Log($"This did: {adjustedDamage} FireStorm Damage and {statusEffectAmount} dot damage for {statusEffectDuration} turns");
    }
}
