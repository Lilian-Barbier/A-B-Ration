using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    public bool canMove = true;
    private bool isMoving = false;
    private AudioSource audioSource;
    Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        FindAnyObjectByType<InputManager>().inputActions.Actions.A.performed += ctx =>
        {
            if (canMove)
            {
                isMoving = true;
                audioSource.Play();
            }
        };
        FindAnyObjectByType<InputManager>().inputActions.Actions.A.canceled += ctx =>
        {
            if (canMove)
            {
                isMoving = false;
                audioSource.Stop();
                speed *= -1;
            }
        };

        FindAnyObjectByType<InputManager>().inputActions.Actions.B.performed += ctx =>
        {
            if (FindAnyObjectByType<InputManager>().currentState != InputManager.States.Cutting && canMove)
            {
                animator.SetBool("Cutting", true);
            }
        };

        FindAnyObjectByType<InputManager>().inputActions.Actions.B.canceled += ctx =>
        {
            if (canMove)
            {
                animator.SetBool("Cutting", false);
            }
        };
    }

    void Update()
    {
        if (isMoving && FindAnyObjectByType<InputManager>().currentState == InputManager.States.WaitingForCut)
        {
            transform.position += speed * Time.deltaTime * Vector3.right;
        }
    }

}
