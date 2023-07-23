using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydroponics : Upgrade
{
    [SerializeField]
    float timePerHarvest = 20;
    [SerializeField]
    float growthRate = 0.5f;
    [SerializeField]
    float status = 0f;
    bool canHarvest = false;
    
    public Hydroponics(RoomBaseObject room) : base(room)
    {
        this.name = "Hydroponics";
    }

    public override void InitializeUpgrade()
    {
        associatedRoom.SetUpgrade(this);
    }

    public void Harvest()
    {
        status = 0;
    }

    private void Update()
    {
        canHarvest = status < timePerHarvest;
        if(canHarvest)
            status += growthRate * Time.deltaTime;
    }
}
