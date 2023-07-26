using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hydroponicsScript : MonoBehaviour
{
    [SerializeField] itemScriptableObject plant;
    [SerializeField] SpriteRenderer spriteOne, spriteTwo;
    [SerializeField] float growthTime, growthRandomness;
    public bool oneGrown, twoGrown;
    float oneGrowth, twoGrowth;
    [SerializeField] Sprite plantSprite;

    // Start is called before the first frame update
    void Start()
    {
        oneGrowth = growthTime + Random.Range(0, growthRandomness);
        twoGrowth = growthTime + Random.Range(0, growthRandomness);
    }

    // Update is called once per frame
    void Update()
    {
        if (oneGrowth > 0)
        {
            oneGrown = false;
            oneGrowth -= Time.deltaTime;
        }
        else oneGrown = true;
        if (twoGrowth > 0)
        {
            twoGrown = false;
            twoGrowth -= Time.deltaTime;
        }
        else twoGrown = true;

        if (oneGrown) spriteOne.sprite = plantSprite;
        else spriteOne.sprite = null;        
        if (twoGrown) spriteTwo.sprite = plantSprite;
        else spriteTwo.sprite = null;


    }

    public void takePlantOne(binScript bin)
    {
        if (oneGrown)
        {
            oneGrowth = growthTime + Random.Range(0, growthRandomness);
            bin.lastInteractedPlayer.getItem(plant);
        }
    }

    public void takePlantTwo(binScript bin)
    {
        if (twoGrown)
        {
            twoGrowth = growthTime + Random.Range(0, growthRandomness);
            bin.lastInteractedPlayer.getItem(plant);
        }
    }
}
