
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public Controls inputActions;

    public enum States
    {
        WaitingForCut,
        Cutting,
        WaitingInputToLoadNextLevel,
        WaitingAnimationNextLevel
    }

    public States currentState = States.WaitingForCut;

    void Awake()
    {
        inputActions = new Controls();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}