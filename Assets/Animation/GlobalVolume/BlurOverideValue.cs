using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurOverideValue : MonoBehaviour
{
    public float blurValue = 100f;
    public float lensDistortionValue = 0f;

    private Volume volumeProfile;
    DepthOfField depthOfField;
    LensDistortion lensDistortion;

    // Start is called before the first frame update
    void Start()
    {
        volumeProfile = GetComponent<Volume>();
        volumeProfile.profile.TryGet(out depthOfField);
        volumeProfile.profile.TryGet(out lensDistortion);
    }

    // Update is called once per frame
    void Update()
    {
        depthOfField.focusDistance.Override(blurValue);
        lensDistortion.intensity.Override(lensDistortionValue);
    }
}
