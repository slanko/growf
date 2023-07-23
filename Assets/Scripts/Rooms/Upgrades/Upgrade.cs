using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Upgrade : MonoBehaviour
{
    [SerializeField]
    protected GameObject itemPrefab;
    protected RoomBaseObject associatedRoom = null;
    
    public Upgrade(RoomBaseObject room)
    {
        itemPrefab.name = "Upgrade";
        associatedRoom = room;
    }
    public abstract void InitializeUpgrade();
    
}
