using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : CardEffect
{
    public override void ApplyEffect(Enemy enemy, int amount)
    {
        enemy.GetDamage(amount);
        Debug.Log("Fire Damage");
    }
}
