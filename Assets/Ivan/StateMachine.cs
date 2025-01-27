using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateMachine : MonoBehaviour
{
    [SerializeField]
    int _currentStateIndex = 0;
    [SerializeField]
    int _defaultStateIndex = 0;
    public GameState currentState => _states[_currentStateIndex];
    public GameState defaultState => _states[_defaultStateIndex];

    [SerializeField]
    List<GameState> _states;

    public UnityEvent<GameState> OnGameStateChange;

    public void SwitchState(int newStateID)
    {
        _currentStateIndex = newStateID;
        OnGameStateChange.Invoke(currentState);
    }
    public void SwitchState(GameState newState)
    {
        if (!_states.Contains(newState))
        {
            Debug.LogWarning("State " + newState.name + " is not in the list of states, it was added to the list");
            _states.Add(newState);
            SwitchState(_states.Count - 1);
        }
        else
            SwitchState(_states.IndexOf(newState));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) 
            SwitchState(0);
        if (Input.GetKeyDown(KeyCode.P))
            SwitchState(1);
    }
}
