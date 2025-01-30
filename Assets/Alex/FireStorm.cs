using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(IDamageable enemy, int amount)
    {
        int adjustedDamage = (int)(amount * 0.5f);
        enemy.GetDamage(redoEffect ? adjustedDamage * 2 : adjustedDamage);
        redoEffect = false;
        Debug.Log($"This did: {adjustedDamage} FireStorm Damage and {statusEffectAmount} dot damage for {statusEffectDuration} turns");
    }
}
