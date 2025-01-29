using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : CardEffect
{
    public override void ApplyEffect(Enemy enemy, int amount)
    {
        int adjustedDamage = Mathf.CeilToInt(amount * 2);
        enemy.GetDamage(adjustedDamage);
        Debug.Log($"This did: {adjustedDamage} SlingShot Damage");
    }
}
