using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : Upgrade
{
    [SerializeField]
    float fuelLevel = 10f;
    [SerializeField]
    float rate = 0.5f;

    public Engine(RoomBaseObject room) : base(room)
    {
        this.name = "Engine";
        InitializeUpgrade();
        //TODO: Uncomment
        //godscript.addEngine();
    }

    public override void InitializeUpgrade()
    {
        associatedRoom.SetUpgrade(this);
    }

    public void addFuel(float fuel)
    {
        fuelLevel += fuel;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Uncomment
        //fuelLevel > 0 ? fuelLevel -= Time.deltaTime * rate : godscript.removeEngine();
    }
}
