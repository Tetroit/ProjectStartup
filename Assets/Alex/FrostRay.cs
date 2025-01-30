using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostRay : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(IDamageable enemy, int amount)
    {
        int adjustedDamage = amount;
        enemy.GetDamage(redoEffect ? adjustedDamage * 2 : adjustedDamage);
        enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.AttackReduction, redoEffect ? statusEffectDuration * 2 : statusEffectDuration
            , redoEffect ? statusEffectAmount * 2 : statusEffectAmount));
        redoEffect = false;
        Debug.Log($"This did: {adjustedDamage} FrostRay Damage and {statusEffectAmount} brittle stacks for {statusEffectDuration} turns");
    }
}
