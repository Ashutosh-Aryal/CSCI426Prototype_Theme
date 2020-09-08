using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavior : MonoBehaviour
{
    private static float startY;
    private static float MAX_Y = -1.7f;
    private static float Y_INCREMENT = 0.06f;

    // Start is called before the first frame update
    void Start()
    {
        startY = gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 myPosition = gameObject.transform.position;
        float newYValue = startY + Y_INCREMENT * IncrementScore.currentScore;

        if(newYValue >= MAX_Y)
        {
            newYValue = MAX_Y;
        }

        gameObject.transform.position = new Vector3(myPosition.x, newYValue, myPosition.z);
    }
}
