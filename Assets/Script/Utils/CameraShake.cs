using UnityEngine;

public class CameraShake : Singleton<CameraShake>
{
    public Camera mainCam;
    float shakeAmmount = 0;

    public void LittleShake()
    {
        Shake(0.01f, 0.05f); 
    }

    public void MediumShake()
    {
        Shake(0.025f, 0.08f); 
    }

    public void BigShake()
    {
        Shake(0.035f, 0.1f); 
    }

    public void Shake(float ammount, float length)
    {
        if(mainCam == null){
            mainCam = Camera.main;
        }

        shakeAmmount = ammount;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    void DoShake()
    {
        if(shakeAmmount>0)
        {
            Vector3 camPosition = mainCam.transform.position;

            float offSetX = Random.value * shakeAmmount * 2 - shakeAmmount;
            float offSetY = Random.value * shakeAmmount * 2 - shakeAmmount;

            camPosition.x += offSetX;
            camPosition.y += offSetY;

            mainCam.transform.position = camPosition;
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        mainCam.transform.localPosition = new Vector3(0, 0, -10);
    }
}
