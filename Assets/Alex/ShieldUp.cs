using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUp : CardEffect
{
    public override void ApplyEffect(Enemy enemy, int amount)
    {
        int adjustedDamage = Mathf.CeilToInt(amount);
        // use a method that gives the enemy extra hitpoints
        Debug.Log($"This gives: {adjustedDamage} Shield Absorbtion");
    }
}
