using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomBaseObject : MonoBehaviour
{
    protected List<RoomBaseObject> adjacentRooms = new List<RoomBaseObject>();
    public int roomDistanceFromCenter;
    protected int roomHealth;
    protected bool isUpgraded;
    //protected UpgradeTypeObject? upgradeType = nulll;


    
    // Start is called before the first frame update
    void Start()
    {
    }

    void AssignAdjacentRooms(RoomBaseObject playersCurrentRoom)
    {
        adjacentRooms.Add(playersCurrentRoom);
    }

    
    void DestroyRoom(GameObject room)
    {

    }

    void AnalyseDestruction()
    {
        var requirements = ChainEvaluation(this, true);
        if (requirements.canDestroy)
        {
            foreach(var room in requirements.roomsToDestroy)
            {
                DestroyRoom(room.gameObject);
            }
        }
    }

    //on room destruction, analyse the chain for other elements to destroy
    (bool canDestroy, List<GameObject> roomsToDestroy) ChainEvaluation(RoomBaseObject caller, bool firstRoom)
    {
        //if no adjacent rooms, assume destruction enabled
        (bool state, List<GameObject> destructionTargets) stateTargets = (true, new List<GameObject>());
        foreach(var room in adjacentRooms)
        {
            //If the room is the caller, we skip it (prevents rooms analyzing each other)
            if (room == caller)
                continue;

            //If the next room's distance is greater than this room's
            if (room.roomDistanceFromCenter > this.roomDistanceFromCenter)
            {
                //Set a bool and gameobject list equal to an evaluation of the next part of the chain
                stateTargets = room.ChainEvaluation(this, false);
                //Add this room to the list of destruction targets when returning from recursive calls
                stateTargets.destructionTargets.Add(gameObject);
            }

            //Otherwise there is another branch connecting this chain
            else
            {
                //If this is the first room in the analysis chain
                if (firstRoom)
                {
                    //Skip to the next element in the loop
                    continue;
                }
                return (false, null);
            }
            
        }
        return stateTargets;
    }


    // Update is called once per frame
    void Update()
    {
        if (roomHealth == 0)
        {
            AnalyseDestruction();
        }
    }
}
