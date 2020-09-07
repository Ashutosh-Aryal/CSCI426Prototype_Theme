using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject trashPrefab;
    [SerializeField] GameObject recyclablePrefab;

    private static float TIME_IT_SHOULD_TAKE = 1.5f;
    private static float ANGLE_TO_SHOOT_AT = 60.0f;
    private static Vector2 directionToShootAt = new Vector2(Mathf.Cos(ANGLE_TO_SHOOT_AT), Mathf.Sin(ANGLE_TO_SHOOT_AT));

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void ShootObjectAtArc(GameObject objToShoot, GameObject objDest)
    {
        Rigidbody2D newRigidBody = objToShoot.AddComponent<Rigidbody2D>();

        Vector3 initialPosition3D = objToShoot.transform.position;
        Vector3 finalPosition3D = objDest.transform.position;

        Vector2 initialPosition = new Vector2(initialPosition3D.x, initialPosition3D.y);
        Vector2 finalPosition = new Vector2(finalPosition3D.x, finalPosition3D.y);

        Vector2 scalar = (finalPosition - initialPosition - (0.5f * Physics2D.gravity * TIME_IT_SHOULD_TAKE * TIME_IT_SHOULD_TAKE)) / (directionToShootAt * TIME_IT_SHOULD_TAKE);
        Debug.Log(scalar);
    }
}
