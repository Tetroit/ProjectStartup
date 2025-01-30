using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUI, NewRunMenuUI;

    //[SerializeField]
    //MainMenuLayout layout;
    [SerializeField]
    public string gameScene = "Game";
    public void StartGame()
    {
        Debug.Log("Start Game");
        GameManager.instance.SwitchScene(gameScene);
    }
    public void ExitGame()
    {
        Debug.Log("Quit Game");
        GameManager.instance.Exit();
    }

    public void OpenNewRunUI()
    {
        MainMenuUI.SetActive(false);
        NewRunMenuUI.SetActive(true);
    }

    public void OpenMainMenuUI()
    {
        MainMenuUI.SetActive(true);
        NewRunMenuUI.SetActive(false);
    }
}
