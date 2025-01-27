using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDart : CardEffect
{
    public override void ApplyEffect(Enemy enemy, int amount)
    {
        enemy.GetDamage(amount);
        Debug.Log("Poison Damage");
    }
}
