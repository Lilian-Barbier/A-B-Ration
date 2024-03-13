using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private bool isMoving = false;

    void Start()
    {
        InputManager.Instance.inputActions.Actions.A.performed += ctx => isMoving = true;
        InputManager.Instance.inputActions.Actions.A.canceled += ctx =>
        {
            isMoving = false;
            speed *= -1;
        };
    }

    void Update()
    {
        if (isMoving && InputManager.Instance.currentState == InputManager.States.WaitingForCut)
        {
            transform.position += speed * Time.deltaTime * Vector3.right;
        }
    }

}
