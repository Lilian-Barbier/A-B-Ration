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
    [SerializeField] private int scoreWhenSamePercent = 150;
    [SerializeField] private int scoreWhenSameMass = 300;
    [SerializeField] private int partLeftPercentRatio = 50;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI ratioLeftText;
    [SerializeField] TextMeshProUGUI ratioRightText;

    
    [SerializeField] TextMeshProUGUI ratioGapText;
    [SerializeField] NumberValueAnimationUI ratioGapScore;

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
        UpdateUI();
    }

    void UpdateUI(){
        scoreText.text = currentScore.ToString();
        ratioLeftText.text = partLeftPercentRatio.ToString() + "%";
        ratioRightText.text = (100 - partLeftPercentRatio).ToString() + "%";
    }

    public void CalculateScore(int percentLeft, int percentRight, int massLeft, int massRight)
    {
        int differenceFromLeftRatio = Mathf.Abs(percentLeft - partLeftPercentRatio);

        ratioGapText.text = differenceFromLeftRatio.ToString() + "%";
        
        int scoreByDifferenceFromLeftRatio = (int)Mathf.Lerp(scoreMaxWithoutPercentDifference, 0, (float)differenceFromLeftRatio / maxPercentDifference);
        ratioGapScore.numberValue = scoreByDifferenceFromLeftRatio;
        ratioGapScore.StartAnimation();
        currentScore += scoreByDifferenceFromLeftRatio;

        ratioGapText.GetComponentInParent<CanvasGroup>().alpha = 1;

        // if (difference <= maxPercentDifference)
        // {
        //     //Todo : a revoir pour adapter le score selon la masse de l'objet / la difficulté
        //     var scoreByPercentDifference = (int)Mathf.Lerp(scoreMaxWithoutPercentDifference, 0, difference / maxPercentDifference);
        //     currentScore += scoreByPercentDifference;

        //     if (percentLeft == percentRight)
        //     {
        //         currentScore += scoreWhenSamePercent;
        //     }
        //     else if (massLeft == massRight)
        //     {
        //         currentScore += scoreWhenSameMass;
        //     }
        // }
        UpdateUI();
    }

    IEnumerator WaitAnimation()
    {
        //A revoir, trop de dépandance entre les classes
        FindAnyObjectByType<Cut>().Reset();

        ratioGapText.GetComponentInParent<CanvasGroup>().alpha = 0;

        //Update difficulty 
        currentLevelIndex++;
        if(currentLevelIndex > 2){
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
