using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : CardEffect
{
    public override void ApplyEffect(IDamageable enemy, int amount)
    {
        int adjustedDamage = amount * 2;
        enemy.GetDamage(redoEffect ? adjustedDamage * 2 : adjustedDamage);
        redoEffect = false;
        Debug.Log($"This did: {adjustedDamage} SlingShot Damage");
    }
}
