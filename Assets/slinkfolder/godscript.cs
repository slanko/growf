using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class godscript : MonoBehaviour
{
    public bool gameStarted;
    PlayerInputManager manager;
    [SerializeField] List<GameObject> playerPrefabs;
    [SerializeField] List<Transform> spawnPoints;
    int playerInt = 0;
    [SerializeField] GameObject gameStartText;
    [SerializeField] TextMeshProUGUI speedometerText, distanceText;
    [Header("Storm Stuff"), SerializeField] Slider stormDistSlider;
    [SerializeField] Transform stormTransform;
    [SerializeField] float stormSpeed, stormSpeedUpRate, stormDistance = 1000f;



    //engines
    [Header("Global Speed Stuff")]
    float distanceTraveled;
    public float speedMult = 0;
    [SerializeField] float acceleration;
    int engineCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted) speedMult = 0;
        else
        {
            speedMult = Mathf.Lerp(speedMult, 0.25f * engineCount, acceleration * Time.deltaTime);
        }
        speedometerText.text = (Mathf.Ceil(100 * speedMult)) + "Km/h";
        distanceTraveled += (100f * speedMult / 3.6f) * Time.deltaTime;
        distanceText.text = Mathf.Ceil(distanceTraveled) + "m";

        //storm stuff
        if (gameStarted)
        {
            stormDistance -= stormSpeed - (speedMult * 100) * Time.deltaTime;
            stormSpeed += stormSpeedUpRate * Time.deltaTime;
            if (stormDistance > 1000) stormDistance = 1000;
            if (stormDistance < 0) stormDistance = 0;
            stormDistSlider.value = 1000 - stormDistance;
            stormTransform.position = new Vector3(0, 5, -stormDistance);
        }
    }

    public void unityEventCheck()
    {
        Debug.Log("Ding!");
    }

    public void addEngine()
    {
        engineCount++;
    }

    public void removeEngine()
    {
        engineCount--;
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

    public void startGame()
    {
        gameStarted = true;
        manager.DisableJoining();
        gameStartText.SetActive(false);
    }
}
