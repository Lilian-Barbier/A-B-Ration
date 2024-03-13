using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private bool isMoving = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InputManager.Instance.inputActions.Actions.A.performed += ctx =>
        {
            isMoving = true;
            audioSource.Play();
        };
        InputManager.Instance.inputActions.Actions.A.canceled += ctx =>
        {
            isMoving = false;
            audioSource.Stop();
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
