using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IncrementScore : MonoBehaviour
{
    [SerializeField] bool isTrash;
    [SerializeField] GameObject scoreObject;

    private static string END_GAME_SCENE_NAME = "EndGameScene";
    private static string SCORE_TEXT = "Score/Flower Health: ";
    public static int currentScore = 0;
    private static Text scoreText;

    [SerializeField] AudioSource trashInBinSound;

    private void Start()
    {
        scoreText = scoreObject.GetComponent<Text>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Contains((isTrash)? MovementBehavior.trashObjectName : MovementBehavior.recyclableObjectName))
        {

            //TODO: Add sound effect here for sound thats made when object actually goes into the trash
            trashInBinSound.Play();

            currentScore++;
            UpdateScoreText();
            Destroy(collision.gameObject);
            Spawner.spawnedRetrievableNames.Remove(collision.gameObject.name);
            Spawner.DecrementNumRetrievables();

            if(currentScore >= 50)
            {
                SceneManager.LoadScene(END_GAME_SCENE_NAME, LoadSceneMode.Single);
            }

        }
    }

    public static void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = SCORE_TEXT + currentScore;
        }
    }
}
