using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayableDirector directorBetweenLevels;
    [SerializeField] PlayableDirector directorEndLevel;
    [SerializeField] GameObject parentOfCuttableObjects;
    private List<GameObject> cuttableObject;
    private GameObject lastBread;
    private GameObject firstBread;
    int currentIndex = 0;
    [SerializeField] private int numberOfIngredients = 4;

    public int currentScore = 0;

    [SerializeField] private int scoreMaxWithoutPercentDifference = 100;
    [SerializeField] private int maxPercentDifference = 10;
    [SerializeField] private int scoreWhenSamePercent = 100;
    [SerializeField] private int scoreWhenSameMass = 200;
    [SerializeField] private int partLeftPercentRatio = 50;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] NumberValueAnimationUI ratioLeftText;
    [SerializeField] NumberValueAnimationUI ratioRightText;


    [SerializeField] TextMeshProUGUI ratioGapText;
    [SerializeField] TextMeshProUGUI continueText;
    [SerializeField] NumberValueAnimationUI ratioGapScore;
    [SerializeField] NumberValueAnimationUI perfectCutScore;
    [SerializeField] NumberValueAnimationUI extraPerfectCutScore;

    [SerializeField] Animator GlobalVolumeAnimator;


    [SerializeField] private GameObject firstPartOfSandwichLeft;
    [SerializeField] private GameObject firstPartOfSandwichRight;

    private List<GameObject> sandwichParts = new List<GameObject>();

    private int currentLevelIndex = 1;

    InputManager inputManager;

    GameObject lastObject;

    // Start is called before the first frame update
    void Start()
    {
        cuttableObject = new List<GameObject>();
        foreach (Transform item in parentOfCuttableObjects.transform)
        {
            if (item.gameObject.name == "BriocheRotate")
            {
                lastBread = item.gameObject;
                continue;
            }
            else if (item.gameObject.name == "Brioche")
            {
                firstBread = item.gameObject;
                continue;
            }
            else if (item.gameObject == parentOfCuttableObjects)
            {
                //remove self
                continue;
            }

            cuttableObject.Add(item.gameObject);
        }

        foreach (var item in cuttableObject)
        {
            item.SetActive(false);
        }

        firstBread.SetActive(true);

        inputManager = FindAnyObjectByType<InputManager>();
        inputManager.inputActions.Actions.A.performed += ctx =>
        {
            if (inputManager.currentState == InputManager.States.WaitingInputToLoadNextLevel)
            {
                inputManager.currentState = InputManager.States.WaitingAnimationNextLevel;
                StartCoroutine(WaitAnimation());
            }
            else if (inputManager.currentState == InputManager.States.WaitingInputToRestart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        };
        UpdateUI(false);
    }

    public void InstantiateSandwichPart(Sprite spriteL, Sprite spriteR)
    {
        GameObject leftPart = Instantiate(firstPartOfSandwichLeft);
        GameObject rightPart = Instantiate(firstPartOfSandwichRight);


        var leftSpriteRenderer = leftPart.GetComponent<SpriteRenderer>();
        var rightSpriteRenderer = rightPart.GetComponent<SpriteRenderer>();

        leftSpriteRenderer.sprite = spriteL;
        rightSpriteRenderer.sprite = spriteR;

        leftSpriteRenderer.sortingOrder = currentLevelIndex;
        rightSpriteRenderer.sortingOrder = currentLevelIndex;

        leftPart.transform.position = firstPartOfSandwichLeft.transform.position + new Vector3(0, currentLevelIndex, 0);
        rightPart.transform.position = firstPartOfSandwichRight.transform.position + new Vector3(0, currentLevelIndex, 0);
        sandwichParts.Add(leftPart);
        sandwichParts.Add(rightPart);
    }

    void UpdateUI(bool startAnimation = true)
    {
        scoreText.text = currentScore.ToString();
        ratioLeftText.startValue = ratioLeftText.numberValue;
        ratioRightText.startValue = ratioRightText.numberValue;
        ratioLeftText.numberValue = partLeftPercentRatio;
        ratioRightText.numberValue = 100 - partLeftPercentRatio;
        if (startAnimation)
        {
            ratioLeftText.StartAnimation();
            ratioRightText.StartAnimation();
        }
    }

    public void CalculateScore(int percentLeft, int percentRight, int massLeft, int massRight)
    {
        //Set Blur
        GlobalVolumeAnimator.SetBool("Blur", true);
        StartCoroutine(WaitAndAnimateScore(percentLeft, percentRight, massLeft, massRight));
    }

    IEnumerator WaitAndAnimateScore(int percentLeft, int percentRight, int massLeft, int massRight)
    {
        yield return new WaitForSeconds(1);
        int differenceFromLeftRatio = Mathf.Abs(percentLeft - partLeftPercentRatio);

        ratioGapText.text = differenceFromLeftRatio.ToString() + "%";

        int scoreByDifferenceFromLeftRatio = (int)Mathf.Lerp(scoreMaxWithoutPercentDifference, 0, (float)differenceFromLeftRatio / maxPercentDifference);
        ratioGapScore.numberValue = scoreByDifferenceFromLeftRatio;
        ratioGapScore.StartAnimation();
        currentScore += scoreByDifferenceFromLeftRatio;

        ratioGapText.GetComponentInParent<CanvasGroup>().alpha = 1;
        yield return new WaitForSeconds(1);

        if (percentLeft == partLeftPercentRatio)
        {
            perfectCutScore.numberValue = scoreWhenSamePercent;
            perfectCutScore.StartAnimation();
            currentScore += scoreWhenSamePercent;

            perfectCutScore.GetComponentInParent<CanvasGroup>().alpha = 1;
            yield return new WaitForSeconds(1);

            int totalMass = massLeft + massRight;
            int extraPerfectCutExpectedValue = totalMass * partLeftPercentRatio / 100;
            if (massLeft == extraPerfectCutExpectedValue)
            {
                extraPerfectCutScore.numberValue = scoreWhenSameMass;
                extraPerfectCutScore.StartAnimation();
                currentScore += scoreWhenSameMass;

                extraPerfectCutScore.GetComponentInParent<CanvasGroup>().alpha = 1;
                yield return new WaitForSeconds(1);
            }
        }

        continueText.GetComponentInParent<CanvasGroup>().alpha = 1;

        FindAnyObjectByType<InputManager>().currentState = InputManager.States.WaitingInputToLoadNextLevel;

        UpdateUI();
    }

    IEnumerator WaitAnimation()
    {
        //unset Blur
        GlobalVolumeAnimator.SetBool("Blur", false);

        //A revoir, trop de dépandance entre les classes
        FindAnyObjectByType<Cut>().Reset();

        ratioGapText.GetComponentInParent<CanvasGroup>().alpha = 0;
        perfectCutScore.GetComponentInParent<CanvasGroup>().alpha = 0;
        extraPerfectCutScore.GetComponentInParent<CanvasGroup>().alpha = 0;
        continueText.GetComponentInParent<CanvasGroup>().alpha = 0;

        //Update difficulty 
        currentLevelIndex++;
        if (currentLevelIndex > 3)
        {
            partLeftPercentRatio = Random.Range(1, 10) * 10;
        }

        UpdateUI();

        currentIndex++;

        foreach (var item in cuttableObject)
        {
            item.SetActive(false);
        }
        firstBread.SetActive(false);
        lastBread.SetActive(false);

        if (lastObject != null)
        {
            lastObject.SetActive(false);
        }

        if (currentIndex == numberOfIngredients + 3)
        {
            //load end screen
            foreach (var item in sandwichParts)
            {
                item.GetComponent<Rigidbody2D>().gravityScale = 0.8f;
            }

            directorEndLevel.Play();
            yield return new WaitForSeconds((float)directorEndLevel.duration);
            inputManager.currentState = InputManager.States.WaitingInputToRestart;
            yield break;
        }
        else if (currentIndex == numberOfIngredients + 2)
        {
            lastBread.SetActive(true);
        }
        else
        {
            int randomIndex = Random.Range(0, cuttableObject.Count);
            //remove element from list
            lastObject = cuttableObject[randomIndex];
            cuttableObject.RemoveAt(randomIndex);
            lastObject.SetActive(true);
        }

        directorBetweenLevels.Play();
        yield return new WaitForSeconds((float)directorBetweenLevels.duration);
        inputManager.currentState = InputManager.States.WaitingForCut;
    }
}
