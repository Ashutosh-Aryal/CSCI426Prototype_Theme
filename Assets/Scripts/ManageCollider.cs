using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCollider : MonoBehaviour
{

    private static float INCREMENT_VAL = 0.1f;
    private static string groundName = "Ground";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name != groundName)
        {
            return;
        }

        Vector3 myPosition = gameObject.transform.position;
        gameObject.transform.position = new Vector3(myPosition.x, myPosition.y + INCREMENT_VAL, myPosition.z);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
    }
}
