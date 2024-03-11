using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private bool isMoving = false;

    void Start()
    {
        InputManager.Instance.inputActions.Actions.A.performed += ctx => isMoving = true;
        InputManager.Instance.inputActions.Actions.A.cancelled += ctx => isMoving = false;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position += speed * Time.deltaTime * Vector3.right;
        }
    }
}
