
using System;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent<GameState> OnGameStateChange => _stateMachine.OnGameStateChange;

    [SerializeField]
    StateMachine _stateMachine;
    public StateMachine StateMachine { get; private set; }

    public GameState currentState => _stateMachine.currentState;

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
        _stateMachine.SwitchState(_stateMachine.defaultState);
    }

    public delegate (bool, string) Validation();
    public event Validation OnValidateTurn;

    public Action OnTurnPassed;
    public static void PassTurn()
    {
        if (instance.OnValidateTurn!=null)
        {
            foreach (Validation listener in instance.OnValidateTurn.GetInvocationList())
            {
                var info = listener();
                if (!info.Item1)
                {
                    Debug.Log("Couldnt pass turn " + info.Item2);
                    return;
                }
            }
        }
        instance.OnTurnPassed?.Invoke();
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
    public void SwitchScene(string sceneName, GameState gameState = null)
    {
        SceneManager.LoadScene(sceneName);
        if (gameState != null)
            _stateMachine.SwitchState(gameState);
        else
            _stateMachine.SwitchState(_stateMachine.defaultState);
    }
}
