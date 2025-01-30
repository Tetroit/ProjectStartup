using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Redo : CardEffect
{
    public override void ApplyEffect(Enemy enemy, int ammount)
    {
        redoEffect = true;
    }
}
