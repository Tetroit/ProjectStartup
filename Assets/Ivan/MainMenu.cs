using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    MainMenuLayout layout;
    [SerializeField]
    public string gameScene = "Game";
    public void StartGame()
    {
        Debug.Log("Start Game");
        GameManager.instance.SwitchScene(gameScene);
    }
    public void ExitGame()
    {
        Debug.Log("Start Game");
        GameManager.instance.Exit();
    }
}
