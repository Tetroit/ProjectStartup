using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUp : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 0;
    public override void ApplyEffect(Enemy enemy, int amount)
    {
        int adjustedAmount = Mathf.CeilToInt(amount * 0.5f);
        enemy.ApplyShield(redoEffect ? adjustedAmount * 2 : adjustedAmount);
        enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.Shield,redoEffect ? statusEffectDuration * 2 : statusEffectDuration
            , statusEffectAmount));
        redoEffect = false;
        Debug.Log($"This gives: {adjustedAmount} Shield Absorbtion for {statusEffectDuration}");
    }
}
