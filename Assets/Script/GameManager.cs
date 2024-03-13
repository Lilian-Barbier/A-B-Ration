using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayableDirector directorBetweenLevels;
    [SerializeField] GameObject parentOfCuttableObjects;
    private List<GameObject> cuttableObject;
    int currentIndex = 0;


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
            if (inputManager.currentState == InputManager.States.WaitingForNexLevel)
            {
                StartCoroutine(WaitAnimation());
            }
        };
    }

    IEnumerator WaitAnimation()
    {
        //A revoir, trop de dépandance entre les classes
        FindAnyObjectByType<Cut>().Reset();

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
