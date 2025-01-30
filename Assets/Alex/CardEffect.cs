using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEffect : MonoBehaviour
{
    public int targetCount;
    public bool targetAll = false;
    public bool redoEffect = false;
    public abstract void ApplyEffect(Enemy enemy, int ammount);
}