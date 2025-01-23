using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MainMenuLayout", menuName = "GameUI/MainMenuLayout")]
public class MainMenuLayout : ScriptableObject
{
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
}
