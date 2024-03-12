
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public Controls inputActions;

    public bool alreadyCut = false;

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