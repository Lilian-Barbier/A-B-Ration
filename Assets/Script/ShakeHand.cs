using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeHand : MonoBehaviour
{

    [SerializeField] private int shakeLevel;
    [SerializeField] private float speed;
    [SerializeField] private Transform line;
    private Vector2 originalPos;
    private Vector2 originalLinePos;


    // Start is called before the first frame update
    void Start()
    {
        shakeLevel = PlayerPrefs.GetInt("ShakeLevel", 0);
        originalPos = transform.position;
        originalLinePos = line.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeLevel > 0)
        {
            Vector3 shakeOffset = new Vector3(Random.Range(-shakeLevel, shakeLevel + 1), Random.Range(-shakeLevel, shakeLevel + 1), 0);
            transform.position += shakeOffset * speed * Time.deltaTime;
            line.position += shakeOffset * speed * Time.deltaTime;

            float minX = originalPos.x - shakeLevel * 0.5f;
            float maxX = originalPos.x + shakeLevel * 0.5f;
            float minY = originalPos.y - shakeLevel * 0.5f;
            float maxY = originalPos.y + shakeLevel * 0.5f;

            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);

            minX = originalLinePos.x - shakeLevel * 0.5f;
            maxX = originalLinePos.x + shakeLevel * 0.5f;
            minY = originalLinePos.y - shakeLevel * 0.5f;
            maxY = originalLinePos.y + shakeLevel * 0.5f;

            clampedX = Mathf.Clamp(line.position.x, minX, maxX);
            clampedY = Mathf.Clamp(line.position.y, minY, maxY);

            line.position = new Vector3(clampedX, clampedY, line.position.z);
        }
    }
}
