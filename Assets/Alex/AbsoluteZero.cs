using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsoluteZero : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(Enemy enemy, int amount)
    {
        enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.AttackReduction, statusEffectDuration, statusEffectAmount));
        Debug.Log($"AbsoluteZero added:  brittle stacks to all enemies");
    }
}
