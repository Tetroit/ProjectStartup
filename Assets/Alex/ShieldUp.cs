using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUp : CardEffect
{
    public override void ApplyEffect(Enemy enemy, int amount)
    {
        Debug.Log("Shield");
    }
}
