using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoop : Upgrade
{
    List<itemScriptableObject> collectedItems = new List<itemScriptableObject>();

    [SerializeField]
    SpriteRenderer[] collectedItemSprites;
    public Scoop(RoomBaseObject room) : base(room)
    {
        this.name = "Scoop";
        InitializeUpgrade();
    }

    public override void InitializeUpgrade()
    {
        associatedRoom.SetUpgrade(this);
    }

    public void retrieveItem(int index)
    {
        collectedItems.RemoveAt(index);
        collectedItemSprites[index].sprite = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: Uncomment
        //if (other.tag == "GroundItem" && collectedItems.Count < 3)
        //{
        //    var item = other.gameObject.GetComponent<groundItemScript>().myObject;
        //    collectedItems.Add(item);
        //    foreach (var display in collectedItemSprites)
        //    {
        //        if (display.sprite != null)
        //            continue;
        //        display.sprite = item.mySprite;
        //        break;
        //    }
        //}
    }
}
