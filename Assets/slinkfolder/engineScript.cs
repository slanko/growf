using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class engineScript : MonoBehaviour
{
    godscript GOD;
    float fuelLevel;
    [SerializeField] float fuelMax = 100, fuelPerRefill = 25, fuelUseRate = 1;
    [SerializeField] Slider fuelBar;
    bool outOfFuel;
    // Start is called before the first frame update
    void Start()
    {
        GOD = GameObject.Find("GOD").GetComponent<godscript>();
        GOD.addEngine();
        fuelLevel = fuelMax;
    }

    private void Update()
    {
        fuelBar.value = fuelLevel;
        if(fuelLevel > 0)
        {
            fuelLevel -= fuelUseRate * Time.deltaTime;
        }
        if (fuelLevel < 0)
        {
            fuelLevel = 0;
            if (!outOfFuel)
            {
                GOD.removeEngine();
                outOfFuel = true;
            }
        }
        if (fuelLevel > fuelMax) fuelLevel = fuelMax;
    }

    public void refillFuel()
    {
        fuelLevel += fuelPerRefill;
        if (fuelLevel > fuelMax) fuelLevel = fuelMax;
        if (outOfFuel)
        {
            outOfFuel = false;
            GOD.addEngine();
        }
    }

    private void OnDestroy()
    {
        GOD.removeEngine();
    }
}
