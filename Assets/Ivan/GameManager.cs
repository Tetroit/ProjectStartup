
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The central element of the game. Manages scenes, game economy, key controls and game states
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton pattern
    /// </summary>
    static GameManager _instance;
    public static GameManager instance => _instance;

    void Awake()
    {
        if (instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R)) Restart();
    }
    /// <summary>
    /// Use it to set up the scene, not void Start()
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
    /// <summary>
    /// Restarts the game
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
