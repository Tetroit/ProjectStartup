using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(IDamageable enemy, int amount)
    {
        // rounds a floating point number up to the nearest whole number.
        int adjustedDamage = (int)(amount * 1.5f);
        enemy.GetDamage(redoEffect ? adjustedDamage * 2 : adjustedDamage);
        redoEffect = false;
        Debug.Log($"This did: {adjustedDamage} FireBall Damage and {statusEffectAmount} dot damage for {statusEffectDuration} turns");
    }
}
