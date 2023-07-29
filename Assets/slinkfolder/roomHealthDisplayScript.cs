using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomHealthDisplayScript : MonoBehaviour
{
    [SerializeField] Color onColour, offColour;

    [SerializeField] SpriteRenderer[] circleSprites; 
    [SerializeField] BaseRoom myRoom;
    int lastRoomHealth;

    // Update is called once per frame
    void Update()
    {
        if(myRoom.roomHealth != lastRoomHealth)
        {
            updateHealthReadout();
            lastRoomHealth = myRoom.roomHealth;
        }
    }

    void updateHealthReadout()
    {
        for(int i = 0; i < circleSprites.Length; i++)
        {
            if (i < myRoom.roomHealth) circleSprites[i].color = onColour;
            else circleSprites[i].color = offColour;

        }
    }
}
