
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
        Debug.Log("Quit Game");
        GameManager.instance.Exit();
    }
}
