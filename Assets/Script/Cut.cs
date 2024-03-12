using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
    void Start()
    {
        InputManager.Instance.inputActions.Actions.B.performed += ctx =>
        {
            Cuting();
        };
    }

    void Cuting()
    {
        var animator = GetComponent<Animator>();
        animator.SetTrigger("Cut");
    }
}
