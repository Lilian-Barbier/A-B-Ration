using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class Cut : MonoBehaviour
{
    [SerializeField] Transform cuttingPoint;
    [SerializeField] private float timeWaitingAnimation = 0.5f;
    [SerializeField] private SpriteRenderer partLeft;
    [SerializeField] private SpriteRenderer partRight;
    [SerializeField] private WeightValueControls balance1;
    [SerializeField] private WeightValueControls balance2;

    [SerializeField] private NumberValueAnimationUI percentLeft;
    [SerializeField] private NumberValueAnimationUI percentRight;

    [SerializeField] private PlayableDirector directorScore;

    private int leftMass;
    private int rightMass;

    void Start()
    {
        Reset();
        InputManager.Instance.inputActions.Actions.B.performed += ctx =>
        {
            if (InputManager.Instance.currentState != InputManager.States.Cutting)
            {
                var animator = GetComponent<Animator>();
                animator.SetTrigger("PrepareCutting");
            }
        };
        InputManager.Instance.inputActions.Actions.B.canceled += ctx =>
        {
            if (InputManager.Instance.currentState != InputManager.States.Cutting)
            {
                Cuting();
            }
        };
    }

    public void Reset()
    {
        percentLeft.hideValue = true;
        percentRight.hideValue = true;
        partLeft.enabled = false;
        partRight.enabled = false;
    }

    void Cuting()
    {
        var animator = GetComponent<Animator>();
        animator.SetTrigger("Cutting");
    }

    void CutWhenAnimationEnds()
    {
        GameObject cuttableObject = GameObject.FindWithTag("Cuttable");

        var spriteRenderer = cuttableObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer.bounds.Contains(cuttingPoint.position))
        {
            InputManager.Instance.currentState = InputManager.States.Cutting;
            int[] mass = cuttableObject.GetComponent<Cuttable>().Cut(cuttingPoint.position, partLeft, partRight);
            leftMass = mass[0];
            rightMass = mass[1];
            StartCoroutine(StartAnimation());
        }
    }

    IEnumerator StartAnimation()
    {
        directorScore.Play();

        yield return new WaitForSeconds((float)directorScore.duration + 0.1f);

        for (int i = 10; i > 0; i--)
        {
            balance1.weightValue = leftMass + i;
            balance2.weightValue = rightMass + i;

            float waitTime = Mathf.Pow(2, 10 - i) * 0.0012f;
            yield return new WaitForSeconds(waitTime);

        }

        balance1.weightValue = leftMass;
        balance2.weightValue = rightMass;

        yield return new WaitForSeconds(1f);

        int percentLeftValue = leftMass * 100 / (leftMass + rightMass);
        int percentRightValue = rightMass * 100 / (leftMass + rightMass);

        percentLeft.hideValue = false;
        percentRight.hideValue = false;
        percentLeft.numberValue = percentLeftValue;
        percentRight.numberValue = percentRightValue;
        percentLeft.StartAnimation();
        percentRight.StartAnimation();

        FindAnyObjectByType<GameManager>().CalculateScore(percentLeftValue, percentRightValue, leftMass, rightMass);

        InputManager.Instance.currentState = InputManager.States.WaitingInputToLoadNextLevel;

        //Reset animation
        var animator = GetComponent<Animator>();
        animator.SetBool("Cutting", false);
    }

    void ShakeScreen(){
        CameraShake.Instance.BigShake();
    }

}
