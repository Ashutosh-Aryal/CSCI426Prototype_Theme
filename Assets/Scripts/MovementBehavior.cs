using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

enum TrashType
{
    NotTrash,
    Recyclable,
    Trash
};

public class MovementBehavior : MonoBehaviour
{

    private enum AnimationType
    {
        Idle,
        MovingRight,
        MovingLeft,
        StartCrouching,
        StopCrouching,
        StartAttacking,
        StopAttacking
    };

    private enum KeyState
    {
        KeyReleased,
        KeyHeld,
        KeyPressed
    };

    // Static variables
    private static KeyCode moveLeftButton = KeyCode.LeftArrow;
    private static KeyCode moveRightButton = KeyCode.RightArrow;
    private static KeyCode pickUpButton = KeyCode.DownArrow;
    private static KeyCode throwAwayButton = KeyCode.UpArrow;
    private static KeyCode attackButton = KeyCode.Space;

    private static float MAX_DISTANCE = 1.2f;
    private static float MAX_VELOCITY = 5.0f;
    private static float PLAYER_X_LIMIT = 5.4f;
    private static float TIME_IT_SHOULD_TAKE = 1.5f;
    private static float ANGLE_TO_SHOOT_AT = 60.0f;
    private static Vector2 directionToShootAt;

    private static string objectTypePreString = "Currently Holding: ";

    public static string trashObjectName = "Trash";
    public static string recyclableObjectName = "Recycling";

    // Member variables
    private Rigidbody2D myRigidbody;
    private SpriteRenderer mySpriteRenderer;
    private Animator myAnimator;
    private SpriteRenderer myRenderer;
    private Vector3 myPosition;
    private AnimationType myCurrentAnimationType;

    private float shootTrashTimer = 0.25f;
    private bool handlePhysics = false;

    private List<string> retrievableObjectsHeld = new List<string>();
    private List<string> inRangeRetrievableObjects = new List<string>();
    private List<string> inRangeTargets = new List<string>();

    [SerializeField] GameObject trashCanObject;
    [SerializeField] GameObject recyclingCanObject;
    [SerializeField] GameObject leftEnemy;
    [SerializeField] GameObject rightEnemy;
    [SerializeField] GameObject currentlyHeldTextObject;

    //[SerializeField] AudioSource 

    enum ThrowState
    {
        CannotThrow,
        CanThrowLeft,
        CanThrowRight
    };

    private TrashType currentlyHeldTrashType = TrashType.NotTrash;
    private ThrowState throwState = ThrowState.CannotThrow;

    void Start()
    {
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        myAnimator = gameObject.GetComponent<Animator>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        directionToShootAt = new Vector2(Mathf.Cos(Mathf.Deg2Rad * ANGLE_TO_SHOOT_AT), Mathf.Sin(Mathf.Deg2Rad * ANGLE_TO_SHOOT_AT));
    }

    // Update is called once per frame
    void Update()
    {
        if(!handlePhysics)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, EnemyBehavior.enemyLayer); handlePhysics = true;
        }

        shootTrashTimer += Time.deltaTime;
        myPosition = gameObject.transform.position;

        UpdateMovement();
        UpdateAnimation();

        if (retrievableObjectsHeld.Count == 0)
        {
            throwState = ThrowState.CannotThrow;
            currentlyHeldTrashType = TrashType.NotTrash;
            currentlyHeldTextObject.GetComponent<Text>().text = "Currently Holding: None";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidingGameObject = collision.gameObject;

        if (retrievableObjectsHeld.Contains(collidingGameObject.name))
        {
            return;
        }

        bool isTrash = collidingGameObject.CompareTag("retrievable");
        bool isTarget = collidingGameObject.CompareTag("target");
        bool isThrowTrigger = collidingGameObject.CompareTag("throwArea");

        if (isTrash)
        {
            inRangeRetrievableObjects.Add(collidingGameObject.name);
        }
        else if (isThrowTrigger)
        {
            if (collidingGameObject.name == "LeftTrigger")
            {
                throwState = ThrowState.CanThrowLeft;
            }
            else
            {
                throwState = ThrowState.CanThrowRight;
            }
        }
        else if (isTarget)
        {
            inRangeTargets.Add(collidingGameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collidingGameObject = collision.gameObject;

        if (retrievableObjectsHeld.Contains(collidingGameObject.name))
        {
            return;
        }

        bool isTrash = collidingGameObject.CompareTag("retrievable");
        bool isThrowTrigger = collidingGameObject.CompareTag("throwArea");
        bool isTarget = collidingGameObject.CompareTag("target");

        if (isTrash)
        {
            inRangeRetrievableObjects.Remove(collidingGameObject.name);
        }
        else if (isThrowTrigger)
        {
            throwState = ThrowState.CannotThrow;
        }
        else if (isTarget)
        {
            inRangeTargets.Remove(collidingGameObject.name);
        }
    }

    private void UpdateMovement()
    {
        bool atPlayerBounds = Mathf.Abs(myPosition.x) > PLAYER_X_LIMIT;
        if (atPlayerBounds)
        {
            bool isPlayerAtLeftBounds = myPosition.x < 0;
            myPosition = new Vector3((isPlayerAtLeftBounds) ? -PLAYER_X_LIMIT : PLAYER_X_LIMIT, myPosition.y, myPosition.z);
            gameObject.transform.position = myPosition;

            myRigidbody.velocity = new Vector2(0.0f, 0.0f);
            myAnimator.SetFloat("speed", 0.0f);
            return;
        }

        bool isPressingThrowAwayButton = Input.GetKey(throwAwayButton);
        bool isWithinTriggerBounds = throwState != ThrowState.CannotThrow;
        bool hasBeenQuarterSecondSinceLastShot = shootTrashTimer >= 0.25f;
        bool playerHoldsObjects = retrievableObjectsHeld.Count > 0;

        bool shouldThrowAwayTrash = isPressingThrowAwayButton && isWithinTriggerBounds && hasBeenQuarterSecondSinceLastShot && playerHoldsObjects;
        if (isPressingThrowAwayButton)
        {
            if (shouldThrowAwayTrash)
            {
                ThrowOutTrash();
            }
        }

        if (Input.GetKeyDown(attackButton)) {
            OnAttackKeyEvent(KeyState.KeyPressed);
        } else if (Input.GetKeyDown(pickUpButton)) {
            OnPickUpKeyEvent(KeyState.KeyPressed);
        } else if (Input.GetKeyUp(attackButton)) {
            OnAttackKeyEvent(KeyState.KeyReleased);
        } else if (Input.GetKeyUp(pickUpButton)) {
            OnPickUpKeyEvent(KeyState.KeyReleased);
        } else if (Input.GetKey(pickUpButton)) {
            OnPickUpKeyEvent(KeyState.KeyHeld);
        } else if (Input.GetKey(moveLeftButton)) {
            myRigidbody.velocity = new Vector2(-MAX_VELOCITY, 0);
            myCurrentAnimationType = AnimationType.MovingLeft;
        } else if (Input.GetKey(moveRightButton)) {
            myRigidbody.velocity = new Vector2(MAX_VELOCITY, 0.0f);
            myCurrentAnimationType = AnimationType.MovingRight;
        } else {
            myRigidbody.velocity = new Vector2(0.0f, 0.0f);
            myCurrentAnimationType = AnimationType.Idle;
        }
    }

    private void OnAttackKeyEvent(KeyState attackState)
    {
        if (retrievableObjectsHeld.Count != 0) {
            // TODO: Play sound effect here to symbolize inability to attack
            return;
        }

        if (attackState == KeyState.KeyReleased) {
            myCurrentAnimationType = AnimationType.StopAttacking;
        } else if (attackState == KeyState.KeyPressed) {
            myCurrentAnimationType = AnimationType.StartAttacking;

            if(inRangeTargets.Count > 0)
            {
                foreach(string objName in inRangeTargets)
                {
                    string parentName = GameObject.Find(objName).transform.parent.gameObject.name;

                    EnemyBehavior enemyBehavior;
                    if (leftEnemy.name == parentName)
                    {
                        enemyBehavior = leftEnemy.GetComponent<EnemyBehavior>();
                    }
                    else
                    {
                        enemyBehavior = rightEnemy.GetComponent<EnemyBehavior>();
                    }

                    enemyBehavior.OnHit();
                }
                inRangeTargets.Clear();
            }
        }
    }

    private void KillInRangeTargets()
    {
        foreach(string objName in inRangeTargets)
        {
            EnemyBehavior enemyBehavior;
            if(leftEnemy.name == objName)
            {
                enemyBehavior = leftEnemy.GetComponent<EnemyBehavior>();
            } else
            {
                enemyBehavior = rightEnemy.GetComponent<EnemyBehavior>();
            }

            enemyBehavior.OnHit();
        }
    }

    private void OnPickUpKeyEvent(KeyState pickUpState)
    {

        if (pickUpState == KeyState.KeyReleased) {
            myCurrentAnimationType = AnimationType.StopCrouching;
        }
        else if (pickUpState == KeyState.KeyHeld) {
            myRigidbody.velocity = new Vector2(0.0f, 0.0f); // TODO?: Might be useless?
        }
        else /* if is PickUpPressed */ {

            myCurrentAnimationType = AnimationType.StartCrouching;
            List<int> indicesToRemove = new List<int>();
            int x = 0;

            foreach (string objName in inRangeRetrievableObjects)
            {

                if (currentlyHeldTrashType == TrashType.NotTrash) {
                    currentlyHeldTrashType = (objName.Contains(trashObjectName)) ? TrashType.Trash : TrashType.Recyclable;
                    currentlyHeldTextObject.GetComponent<Text>().text = objectTypePreString + GetStringFromTrashType(currentlyHeldTrashType);
                }
                
                if (objName.Contains(GetStringFromTrashType(currentlyHeldTrashType))) {
                    
                    GameObject gameObj = GameObject.Find(objName);
                    Vector3 differenceVector = gameObj.transform.position - gameObject.transform.position;

                    bool withinPickUppableDistance = differenceVector.magnitude < MAX_DISTANCE;

                    if(withinPickUppableDistance)
                    {
                        gameObj.GetComponent<SpriteRenderer>().enabled = false;
                        retrievableObjectsHeld.Add(objName);
                    } 
                    
                    indicesToRemove.Add(x);
                }

                x++;
            }

            for (x = indicesToRemove.Count - 1; x >= 0; x--) {
                inRangeRetrievableObjects.RemoveAt(indicesToRemove[x]);
            }
        }
    }

    private string prevName;

    private void ThrowOutTrash()
    {
        bool canThrowLeft = throwState == ThrowState.CanThrowLeft;
        string objName = retrievableObjectsHeld[0];

        GameObject srcObj = GameObject.Find(objName);

        if (prevName == objName)
        {
            retrievableObjectsHeld.Remove(objName);
            return;
        }

        bool cannotThrowObject = canThrowLeft && currentlyHeldTrashType != TrashType.Trash;
        cannotThrowObject |= !canThrowLeft && currentlyHeldTrashType != TrashType.Recyclable;

        if(cannotThrowObject)
        {
            // TODO: Play sound effect for cannot throw object here
            return;
        }

        prevName = objName;
        srcObj.GetComponent<SpriteRenderer>().enabled = true;
        srcObj.transform.position = gameObject.transform.position;
        GameObject destObj = (throwState == ThrowState.CanThrowLeft) ? trashCanObject : recyclingCanObject;

        ShootObjectAtArc(ref srcObj, destObj);
        retrievableObjectsHeld.Remove(objName);

        shootTrashTimer = 0.0f;
    }

    private void UpdateAnimation()
    {

        switch (myCurrentAnimationType)
        {
            case AnimationType.Idle:
                myAnimator.SetFloat("speed", 0.0f);
                break;

            case AnimationType.MovingLeft:
                myAnimator.SetFloat("speed", 1.0f);
                myRenderer.flipX = true;
                break;

            case AnimationType.MovingRight:
                myAnimator.SetFloat("speed", 1.0f);
                myRenderer.flipX = false;
                break;

            case AnimationType.StopCrouching:
                myAnimator.SetBool("isCrouching", false);
                break;

            case AnimationType.StartCrouching:
                myAnimator.SetBool("isCrouching", true);
                break;

            case AnimationType.StartAttacking:
                myAnimator.SetBool("isAttacking", true);
                break;

            case AnimationType.StopAttacking:
                myAnimator.SetBool("isAttacking", false);
                break;
        }

    }

    private static void ShootObjectAtArc(ref GameObject objToShoot, GameObject objDest)
    {
        Rigidbody2D newRigidBody = objToShoot.AddComponent<Rigidbody2D>();

        Vector3 initialPosition3D = objToShoot.transform.position;
        Vector3 finalPosition3D = objDest.transform.position;

        Vector2 initialPosition = new Vector2(initialPosition3D.x, initialPosition3D.y);
        Vector2 finalPosition = new Vector2(finalPosition3D.x, finalPosition3D.y);

        Vector2 scalar = (finalPosition - initialPosition - (0.5f * Physics2D.gravity * TIME_IT_SHOULD_TAKE * TIME_IT_SHOULD_TAKE)) / (directionToShootAt * TIME_IT_SHOULD_TAKE);
        newRigidBody.velocity = scalar * directionToShootAt;
    }

    private string GetStringFromTrashType(TrashType tt) { 
        if(tt == TrashType.Recyclable)
        {
            return recyclableObjectName;
        } else
        {
            return trashObjectName;
        }
    }

    
}
