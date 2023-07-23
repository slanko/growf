using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : Upgrade
{
    public Fishing(RoomBaseObject room) : base(room)
    {
        this.name = "FishingHole";
        //Ignore this area
    }

    public override void InitializeUpgrade()
    {
        associatedRoom.SetUpgrade(this);
        //Treat this area as void Start()
    }


    //Do the fishing holes functionality here

    private void Update()
    {
        
    }
}
