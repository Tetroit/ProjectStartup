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
        int adjustedDamage = amount;
        enemy.IgnoreShield(redoEffect ? adjustedDamage * 2 : adjustedDamage);
        redoEffect = false;
        Debug.Log($"This did: {adjustedDamage} Poison Damage and {statusEffectAmount} dot dmg for {statusEffectDuration} turns");
    }
}
