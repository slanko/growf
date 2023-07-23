using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class godscript : MonoBehaviour
{
    public bool gameStarted;
    public float speedMult;
    PlayerInputManager manager;
    [SerializeField] List<GameObject> playerPrefabs;
    [SerializeField] List<Transform> spawnPoints;
    int playerInt = 0;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void unityEventCheck()
    {
        Debug.Log("Ding!");
    }

    public void addEngine()
    {

    }

    public void removeEngine()
    {

    }

    public void playerJoinBehaviour()
    {
        if (playerInt < 3)
        {
            playerInt++;
            if (playerInt > 3) playerInt = 3;
            manager.playerPrefab = playerPrefabs[playerInt];
        }
    }

    public Transform getSpawnPoint()
    {
        return spawnPoints[playerInt - 1];
    }
}
