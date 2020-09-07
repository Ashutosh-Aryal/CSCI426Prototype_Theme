using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCollider : MonoBehaviour
{

    private static string groundName = "Ground";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name != groundName)
        {
            return;
        }

        Destroy(gameObject.GetComponent<Rigidbody2D>());
    }
}
