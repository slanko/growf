using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slinkScoopScript : MonoBehaviour
{
    public itemScriptableObject[] inventory;
    [SerializeField] SpriteRenderer display1, display2, display3;
    [SerializeField] Transform scoopZone;
    [SerializeField] LayerMask groundItemLayer;
    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] hitItems = Physics.OverlapBox(scoopZone.position, scoopZone.localScale / 2, scoopZone.rotation, groundItemLayer);
        if(hitItems.Length > 0 && !checkScoopFull())
        {
            groundItemScript item = hitItems[0].GetComponent<groundItemScript>();
            itemScriptableObject[] tempList = inventory;
            for (int i = 0; i < inventory.Length; i++)
            {
                if(inventory[i].myType == itemScriptableObject.itemType.NOTHING)
                {
                    tempList[i] = item.myObject;
                    break;
                }
            }
            inventory = tempList;
            Destroy(hitItems[0].gameObject);
        }
        display1.sprite = inventory[0].mySprite;
        display2.sprite = inventory[1].mySprite;
        display3.sprite = inventory[2].mySprite;
    }

    bool checkScoopFull()
    {
        bool toReturn = true;
        foreach(itemScriptableObject item in inventory)
        {
            if (item.myType == itemScriptableObject.itemType.NOTHING) toReturn = false;
        }
        return toReturn;
    }

}
