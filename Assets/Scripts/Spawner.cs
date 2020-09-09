using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject trashObject;
    [SerializeField] GameObject recycleObject;

    private static int currentNumRetrievables = 0;
    private static float spawnTimer = 2.0f;
    private static float spawnRate = 2.0f;
    private static float trashRange = 5.0f;
    private static int MAX_NUM_RETRIEVABLES_IN_SCENE = 15;

    public static HashSet<string> spawnedRetrievableNames = new HashSet<string>();

    // Update is called once per frame
    void Update() {

        spawnTimer += Time.deltaTime;
        if ((spawnTimer > spawnRate) && (currentNumRetrievables < MAX_NUM_RETRIEVABLES_IN_SCENE)) {
            spawnTimer = 0.0f;

            float chance = Random.Range(0.0f, 1.0f);

            GameObject retrievableObject;
            string customName;

            if(chance < 0.5f)
            {
                retrievableObject = trashObject;
                int randIndex;

                do
                {
                    randIndex = Random.Range(0, 1000);
                    customName = MovementBehavior.trashObjectName + " (" + randIndex + ")";
                } while (spawnedRetrievableNames.Contains(customName));



            } else
            {
                retrievableObject = recycleObject;
                int randIndex;

                do
                {
                    randIndex = Random.Range(0, 1000);
                    customName = MovementBehavior.recyclableObjectName + " (" + randIndex + ")";
                } while (spawnedRetrievableNames.Contains(customName));
            }

            spawnedRetrievableNames.Add(customName);
            GameObject createdObject = Instantiate(retrievableObject,  new Vector3(Random.Range(-trashRange, trashRange), gameObject.transform.position.y, 0), Quaternion.identity);
            createdObject.name = customName;
            currentNumRetrievables += 1;
        }
    }

    public static void DecrementNumRetrievables()
    {
        currentNumRetrievables--;
    }

}
