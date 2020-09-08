using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator myAnimator;
    private Rigidbody2D myRigidbody;
    private Vector3 mySpawnLocation;
    private Vector3 myDifferenceInPosAfterAnimChance;

    private float damageTimer = 0.0f;
    private float stopTimer = 0.0f;
    private bool didStart = false;
    private bool didSpawn = false;
    private bool isAttacking = false;
    private bool wasHit = false;
    private bool needToReAdjustPosAndScale = false;
  
    private static string ATTACKING_ANIM_PARAMETER_KEY = "isAttacking";
    private static float SCALE_RATIO_BETWEEN_ANIMS = 2.0f;
    private static float MOVEMENT_SPEED = 2.0f;
    private static float DIFFERENCE_IN_X_DIRECTION = 1.5f;
    private static float DIFFERENCE_IN_Y_DIRECTION = 0.55f;
    private static int DAMAGE = 1;
    private static int SCORE_TO_START_ON = 0;
    private static int numEnemies = 0;

    public static int enemyLayer;

    void Start()
    {
        numEnemies++;
        mySpawnLocation = gameObject.transform.position;
        enemyLayer = gameObject.layer;

        if(mySpawnLocation.x > 0)
        {
            myDifferenceInPosAfterAnimChance = new Vector3(-DIFFERENCE_IN_X_DIRECTION, -DIFFERENCE_IN_Y_DIRECTION, 0.0f);
        }
        else
        {
            myDifferenceInPosAfterAnimChance = new Vector3(DIFFERENCE_IN_X_DIRECTION, -DIFFERENCE_IN_Y_DIRECTION, 0.0f);
        }

        gameObject.transform.GetChild(0).gameObject.name += " (" + numEnemies + ")";

        myAnimator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IncrementScore.currentScore < SCORE_TO_START_ON && !didStart)
        {
            return;
        } else if(!didStart)
        {
            didStart = true;
        } else if(stopTimer > 0.0f)
        {
            stopTimer -= Time.deltaTime;
            return;
        }

        if(!isAttacking)
        {
            Vector3 myPosition = gameObject.transform.position;
            myRigidbody.velocity = new Vector2((myPosition.x > 0)? - MOVEMENT_SPEED : MOVEMENT_SPEED, 0.0f);
        } else
        {
            myRigidbody.velocity = Vector2.zero;
        }
    }

    public void OnHit()
    {
        isAttacking = false;
        gameObject.transform.position = mySpawnLocation;
        myAnimator.SetBool(ATTACKING_ANIM_PARAMETER_KEY, isAttacking);

        Vector3 myPosition = gameObject.transform.position;
        Vector3 myScale = gameObject.transform.localScale;

        if(needToReAdjustPosAndScale)
        {
            gameObject.transform.position = myPosition + myDifferenceInPosAfterAnimChance;
            gameObject.transform.localScale /= SCALE_RATIO_BETWEEN_ANIMS;
            needToReAdjustPosAndScale = false;
        }

        stopTimer = Random.Range(8.0f, 12.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemyTrigger") && !isAttacking)
        {
            isAttacking = true;
            myAnimator.SetBool(ATTACKING_ANIM_PARAMETER_KEY, isAttacking);

            Vector3 myPosition = gameObject.transform.position;
            Vector3 myScale = gameObject.transform.localScale;

            gameObject.transform.position = myPosition + myDifferenceInPosAfterAnimChance;
            gameObject.transform.localScale *= SCALE_RATIO_BETWEEN_ANIMS;
            needToReAdjustPosAndScale = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("flower") && damageTimer <= 0.0f && IncrementScore.currentScore > 0)
        {
            damageTimer = 2.0f;
            IncrementScore.currentScore -= DAMAGE;
            IncrementScore.UpdateScoreText();
        }


        damageTimer -= Time.deltaTime;
    }
}
