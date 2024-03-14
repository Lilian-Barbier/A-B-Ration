using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ChangeHint1ToHint2 : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] PlayableAsset Hint2Animation;

    private bool isLoaded = false;
    public bool canChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        FindAnyObjectByType<InputManager>().inputActions.Actions.A.performed += ctx =>
        {
            if (!isLoaded && canChanged)
            {
                isLoaded = true;
                StartCoroutine(ChangeHint());
            }
        };
    }

    IEnumerator ChangeHint()
    {
        yield return new WaitForSeconds(1);
        director.playableAsset = Hint2Animation;
        director.Play();
    }
}
