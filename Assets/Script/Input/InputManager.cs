
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Controls inputActions;

    public enum States
    {
        WaitingForCut,
        Cutting,
        WaitingInputToLoadNextLevel,
        WaitingAnimationNextLevel,
        WaitingInputToRestart
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