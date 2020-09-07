using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject trash;
    private int numPiecesOfTrash = 0;
    private float time = 0.0f;
    private static float spawnRate = 8.0f;
    private static float trashRange = 5.0f;
    private static int maxPiecesOfTrash = 10;
    public GameObject mObjToShoot;
    public GameObject mObjDest;
    private float timer;
    private static Vector2 pos;

    // Start is called before the first frame update
    void Start() {
        timer = 0.0f;
        pos = mObjToShoot.transform.position;
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
        if ((time > spawnRate) && (numPiecesOfTrash < maxPiecesOfTrash)) {
            time = 0.0f;
            Instantiate(trash, new Vector3(Random.Range(-trashRange, trashRange), 0, 0), Quaternion.identity);
            numPiecesOfTrash += 1;
        }
        // objToShoot.transform.RotateAround(((objDest.transform.position - objToShoot.transform.position) / 2), new Vector3(0, 0, 1), -20 * Time.deltaTime);
    }

}
