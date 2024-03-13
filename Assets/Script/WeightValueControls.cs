using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeightValueControls : MonoBehaviour
{
    public int weightValue = 0;
    public TextMeshProUGUI weightValueText;
    AudioSource audioSource;
    int lastValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        weightValueText = GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        weightValueText.text = weightValue.ToString();
        if (audioSource.isPlaying == false && lastValue != weightValue)
        {
            audioSource.Play();
            lastValue = weightValue;
        }
    }
}
