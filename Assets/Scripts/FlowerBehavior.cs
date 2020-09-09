using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavior : MonoBehaviour
{
    private int currentScorePosAndPitchShown = 0;

    private static float startY;
    private static float MAX_Y = -2.0f;
    private static float Y_INCREMENT = 0.05f;

    private static float startPitch;
    private static float MAX_PITCH = 3.0f;
    private static float PITCH_INCREMENT = 0.04f;

    [SerializeField] AudioSource flowerGrowSound;

    // Start is called before the first frame update
    void Start()
    {
        startY = gameObject.transform.position.y;
        startPitch = flowerGrowSound.pitch;
    }

    // Update is called once per frame
    void Update()
    { 
        Vector3 myPosition = gameObject.transform.position;
        float newYValue = startY + Y_INCREMENT * IncrementScore.currentScore;
        float newPitch = startPitch + PITCH_INCREMENT * IncrementScore.currentScore;

        if(newYValue >= MAX_Y)
        {
            newYValue = MAX_Y;
        }

        if(newPitch >= MAX_PITCH)
        {
            newPitch = MAX_PITCH;
        }

        gameObject.transform.position = new Vector3(myPosition.x, newYValue, myPosition.z);
        flowerGrowSound.pitch = newPitch;

        if(currentScorePosAndPitchShown != IncrementScore.currentScore)
        {
            currentScorePosAndPitchShown = IncrementScore.currentScore;
            flowerGrowSound.Play();
        }
    }
}
