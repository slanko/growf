using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawner : MonoBehaviour
{
    [SerializeField] godscript GOD;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] GameObject itemToSpawn;
    [SerializeField] itemScriptableObject defaultItem;
    [SerializeField] int minTime, maxTime;
    [System.Serializable]
    struct itemSpawnStruct
    {
        public itemScriptableObject myObject;
        public int chance;
    }
    [SerializeField] List<itemSpawnStruct> possibleItems;
    bool coroutineStarted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GOD.gameStarted && !coroutineStarted)
        {
            StartCoroutine(itemSpawnRoutine());
            Debug.Log("Coroutine Started!");
            coroutineStarted = true;
        }
    }

    IEnumerator itemSpawnRoutine()
    {
        itemScriptableObject pickedItem = defaultItem;
        for(int i = 0; i < possibleItems.Count; i++)
        {
            if(Random.Range(0, 101) < possibleItems[i].chance)
            {
                pickedItem = possibleItems[i].myObject;
            }
        }
        groundItemScript newItem = Instantiate(itemToSpawn, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity).GetComponent<groundItemScript>();
        newItem.myObject = pickedItem;
        yield return new WaitForSeconds(Random.Range(minTime, maxTime) * 1 - (GOD.speedMult / 2));
        StartCoroutine(itemSpawnRoutine());
    }

}
