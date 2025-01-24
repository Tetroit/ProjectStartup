using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : CardEffect
{
    public override void ApplyEffect(Enemy enemy, int ammount)
    {
        enemy.health -= ammount;
        Debug.Log("Fire Damage");
    }
}
