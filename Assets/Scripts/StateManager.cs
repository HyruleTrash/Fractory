using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateManager : MonoBehaviour {
    public string defaultState;
    private List<string> currentStates = new List<string>();
    private string[] states = new string[] { };
    public delegate void StateChangeListener(List<string> currentStates);
    private List<StateChangeListener> stateChangeListeners = new List<StateChangeListener>();

    private void Start() {
        if (defaultState != null && defaultState != "")
        {
            AddPossibleState(defaultState);
            AddState(defaultState);
        }
    }

    /// <summary>
    /// Adds a new state to the current states list
    /// </summary>
    /// <param name="newState"></param>
    public void AddState(string newState) {
        if (currentStates.IndexOf(newState) != -1) return;
        if (System.Array.IndexOf(states, newState) == -1) return;

        currentStates.Add(newState);
        CallStateChangeListeners();
    }

    /// <summary>
    /// Removes a state from the current states list
    /// </summary>
    /// <param name="state"></param>
    public void RemoveState(string state) {
        if (currentStates.IndexOf(state) == -1) return;

        currentStates.Remove(state);
        CallStateChangeListeners();
    }

    /// <summary>
    /// Checks if the current states list contains a certain state
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool HasState(string state) {
        return currentStates.IndexOf(state) != -1;
    }

    /// <summary>
    /// Adds a list of possible states to the possible states list
    /// </summary>
    /// <param name="states"></param>
    public void AddPossibleState(string[] states) {
        string[] newStates = new string[this.states.Length + states.Length];
        this.states.CopyTo(newStates, 0);
        states.CopyTo(newStates, this.states.Length);
        this.states = newStates;
    }

    /// <summary>
    /// Adds a possible state to the possible states list
    /// </summary>
    /// <param name="state"></param>
    public void AddPossibleState(string state) {
        AddPossibleState(new string[] { state });
    }

    /// <summary>
    /// Adds a listener function that will be called when the state changes
    /// </summary>
    /// <param name="listener"></param>
    public void AddStateChangeListener(StateChangeListener listener) {
        stateChangeListeners.Add(listener);
    }

    /// <summary>
    /// Removes a listener function from the state change listeners list
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveStateChangeListener(StateChangeListener listener) {
        stateChangeListeners.Remove(listener);
    }

    /// <summary>
    /// Calls all the state change listeners
    /// </summary>
    public void CallStateChangeListeners() {
        foreach (StateChangeListener listener in stateChangeListeners) {
            listener(currentStates);
        }
    }
}