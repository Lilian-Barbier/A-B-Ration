using UnityEngine;
using TMPro;
public class NumberValueAnimationUI : MonoBehaviour
{
    public int currentValue = 0;
    public int numberValue = 0;
    public int duration = 2;

    public bool animationColor = true;
    public bool animationFontSize = true;
    public bool animationWobble = true;

    public int numberValueMax = 100;

    public bool hideValue = false;

    public TextMeshProUGUI valueText;

    public string prefix = "+";
    public string suffix = "";

    // Start is called before the first frame update
    void Start()
    {
        valueText = GetComponent<TextMeshProUGUI>();
    }

    public void StartAnimation(){
        Animation anim = GetComponent<Animation>();
        AnimationCurve curve;

        // create a new AnimationClip
        AnimationClip clip = new AnimationClip
        {
            legacy = true
        };

        curve = AnimationCurve.EaseInOut(0, currentValue, duration, numberValue);
        clip.SetCurve("", typeof(NumberValueAnimationUI), "numberValue", curve);

        if(animationColor){
            float maxRedValue = 2f;
            float minRedValue = 0.58f;
            float targetRedValue = maxRedValue * numberValue / numberValueMax;
            targetRedValue = Mathf.Clamp(targetRedValue, minRedValue, maxRedValue);

            curve = AnimationCurve.EaseInOut(0, minRedValue, duration, targetRedValue);
            clip.SetCurve("", typeof(TextMeshProUGUI), "m_fontColor.r", curve);
        }

        if(animationFontSize){
            float maxFontSize = 200f;
            float minFontSize = 90f;
            float targetFontValue = maxFontSize * numberValue / numberValueMax;
            targetFontValue = Mathf.Clamp(targetFontValue, minFontSize, maxFontSize);

            curve = AnimationCurve.EaseInOut(0, minFontSize, duration, targetFontValue);
            clip.SetCurve("", typeof(TextMeshProUGUI), "m_fontSize", curve);     
        }

        if(animationWobble){
            float maxWobbleHeight = 7f;
            float minWobbleHeight = 0f;
            float targetWobbleValue = maxWobbleHeight * numberValue / numberValueMax;
            targetWobbleValue = Mathf.Clamp(targetWobbleValue, minWobbleHeight, maxWobbleHeight);

            curve = AnimationCurve.EaseInOut(0, minWobbleHeight, duration, targetWobbleValue);
            clip.SetCurve("", typeof(SimpleTextWobble), "wobbleAmount", curve);
        }

        // now animate the GameObject
        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);
    }

    // Update is called once per frame
    void Update()
    {
        if(hideValue){
            valueText.text = "";
            return;
        }
        valueText.text = prefix + numberValue.ToString() + suffix;
    }
}
