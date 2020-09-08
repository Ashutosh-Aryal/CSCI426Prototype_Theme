using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncrementScore : MonoBehaviour
{
    [SerializeField] bool isTrash;
    //[SerializeField] GameObject scoreObject;

    private int currentScore = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Contains((isTrash)? MovementBehavior.trashObjectName : MovementBehavior.recyclableObjectName))
        {

            /*Text scoreText = scoreObject.GetComponent<Text>();

            if(scoreText != null)
            {
                scoreText.text = "Score: " + currentScore;
            }**/

            Destroy(collision.gameObject);
            Spawner.spawnedRetrievableNames.Remove(collision.gameObject.name);
            Spawner.DecrementNumRetrievables();
        }

    }
}
