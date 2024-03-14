using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.SceneManagement;

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

    [SerializeField] private AudioClip prepareCuttingSound;
    [SerializeField] private AudioClip impactSound;

    [SerializeField] private bool titleScreen = false;
    [SerializeField] private bool canCut = true;

    private int leftMass;
    private int rightMass;

    void Start()
    {
        if (!titleScreen)
        {
            Reset();
        }
        FindAnyObjectByType<InputManager>().inputActions.Actions.B.performed += ctx =>
        {
            if (FindAnyObjectByType<InputManager>().currentState != InputManager.States.Cutting && canCut)
            {
                GetComponent<Animator>().SetBool("Cutting", true);
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = prepareCuttingSound;
                audioSource.Play();
            }
        };
        FindAnyObjectByType<InputManager>().inputActions.Actions.B.canceled += ctx =>
        {
            if (FindAnyObjectByType<InputManager>().currentState != InputManager.States.Cutting)
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
        GetComponent<Animator>().SetBool("Cutting", false);
    }

    void CutWhenAnimationEnds()
    {
        GameObject cuttableObject = GameObject.FindWithTag("Cuttable");

        var spriteRenderer = cuttableObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer.bounds.Contains(cuttingPoint.position))
        {
            FindAnyObjectByType<InputManager>().currentState = InputManager.States.Cutting;
            int[] mass = cuttableObject.GetComponent<Cuttable>().Cut(cuttingPoint.position, partLeft, partRight);
            leftMass = mass[0];
            rightMass = mass[1];

            if (!titleScreen)
            {
                FindAnyObjectByType<GameManager>().InstantiateSandwichPart(partLeft.sprite, partRight.sprite);
            }

            StartCoroutine(StartAnimation());
        }
    }

    IEnumerator StartAnimation()
    {
        directorScore.Play();

        yield return new WaitForSeconds((float)directorScore.duration + 0.1f);

        if (titleScreen)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            yield break;
        }

        for (int i = 10; i > 0; i--)
        {
            balance1.weightValue = leftMass + i;
            balance2.weightValue = rightMass + i;

            float waitTime = Mathf.Pow(2, 10 - i) * 0.0012f;
            yield return new WaitForSeconds(waitTime);

        }

        balance1.weightValue = leftMass;
        balance2.weightValue = rightMass;

        int percentLeftValue = leftMass * 100 / (leftMass + rightMass);
        int percentRightValue = 100 - percentLeftValue;

        percentLeft.hideValue = false;
        percentRight.hideValue = false;
        percentLeft.numberValue = percentLeftValue;
        percentRight.numberValue = percentRightValue;
        percentLeft.StartAnimation();
        percentRight.StartAnimation();

        yield return new WaitForSeconds(2.5f);

        FindAnyObjectByType<GameManager>().CalculateScore(percentLeftValue, percentRightValue, leftMass, rightMass);

        //Reset animation
        var animator = GetComponent<Animator>();
        animator.SetBool("Cutting", false);
    }

    void ShakeScreen()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = impactSound;
        audioSource.Play();
        CameraShake.Instance.BigShake();
    }

}
