using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeightValueControls : MonoBehaviour
{
    public int weightValue = 0;
    public TextMeshProUGUI weightValueText;

    // Start is called before the first frame update
    void Start()
    {
        weightValueText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        weightValueText.text = weightValue.ToString();
    }
}
