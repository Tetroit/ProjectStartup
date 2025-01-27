using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState", order = 1)]
public class GameState : ScriptableObject
{
    public bool allowChipInteraction;
    public bool allowCardInteraction;
    public bool allwoEnemyInteraction;
}
