using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayableDirector directorBetweenLevels;
    [SerializeField] GameObject parentOfCuttableObjects;
    private List<GameObject> cuttableObject;
    int currentIndex = 0;

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
    [SerializeField] NumberValueAnimationUI ratioGapScore;
    [SerializeField] NumberValueAnimationUI perfectCutScore;
    [SerializeField] NumberValueAnimationUI extraPerfectCutScore;

    [SerializeField] Animator GlobalVolumeAnimator;

    private int currentLevelIndex = 1;

    InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        cuttableObject = new List<GameObject>();
        foreach (Transform item in parentOfCuttableObjects.transform)
        {
            //remove self
            if (item.gameObject == parentOfCuttableObjects)
            {
                continue;
            }
            cuttableObject.Add(item.gameObject);
        }

        foreach (var item in cuttableObject)
        {
            item.SetActive(false);
        }
        cuttableObject[currentIndex].SetActive(true);

        inputManager = InputManager.Instance;
        inputManager.inputActions.Actions.A.performed += ctx =>
        {
            if (inputManager.currentState == InputManager.States.WaitingInputToLoadNextLevel)
            {
                inputManager.currentState = InputManager.States.WaitingAnimationNextLevel;
                StartCoroutine(WaitAnimation());
            }
        };
        UpdateUI(false);
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

        int differenceFromLeftRatio = Mathf.Abs(percentLeft - partLeftPercentRatio);

        ratioGapText.text = differenceFromLeftRatio.ToString() + "%";

        int scoreByDifferenceFromLeftRatio = (int)Mathf.Lerp(scoreMaxWithoutPercentDifference, 0, (float)differenceFromLeftRatio / maxPercentDifference);
        ratioGapScore.numberValue = scoreByDifferenceFromLeftRatio;
        ratioGapScore.StartAnimation();
        currentScore += scoreByDifferenceFromLeftRatio;

        ratioGapText.GetComponentInParent<CanvasGroup>().alpha = 1;

        if (percentLeft == partLeftPercentRatio)
        {
            perfectCutScore.numberValue = scoreWhenSamePercent;
            perfectCutScore.StartAnimation();
            currentScore += scoreWhenSamePercent;

            perfectCutScore.GetComponentInParent<CanvasGroup>().alpha = 1;

            int totalMass = massLeft + massRight;
            int extraPerfectCutExpectedValue = totalMass * partLeftPercentRatio / 100;
            Debug.Log("extraPerfectCutExpectedValue: " + extraPerfectCutExpectedValue);
            if (massLeft == extraPerfectCutExpectedValue)
            {
                extraPerfectCutScore.numberValue = scoreWhenSameMass;
                extraPerfectCutScore.StartAnimation();
                currentScore += scoreWhenSameMass;

                extraPerfectCutScore.GetComponentInParent<CanvasGroup>().alpha = 1;
            }
        }

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

        //Update difficulty 
        currentLevelIndex++;
        if (currentLevelIndex > 2)
        {
            partLeftPercentRatio = Random.Range(1, 10) * 10;
        }

        UpdateUI();

        currentIndex++;
        if (currentIndex >= cuttableObject.Count)
        {
            currentIndex = 0;
        }
        foreach (var item in cuttableObject)
        {
            item.SetActive(false);
        }
        cuttableObject[currentIndex].SetActive(true);

        directorBetweenLevels.Play();
        yield return new WaitForSeconds((float)directorBetweenLevels.duration);
        inputManager.currentState = InputManager.States.WaitingForCut;
    }
}
