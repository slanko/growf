using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseRoom : MonoBehaviour
{
    public List<BaseRoom> adjacentRooms = new List<BaseRoom>();
    public int roomDistanceFromCenter;
    protected int roomMaxHealth = 2;
    protected int roomHealth = 2;
    protected bool isUpgraded;
    protected Upgrade upgrade = null;

    [SerializeField]
    int obstacleHealthReduction = 1;
    [SerializeField]
    int mineHealthReduction = 2;

    [SerializeField]
    int Id;

    RoomHandler constructor;

    //Receive a reference to the constructor
    //Being given a thing is more efficient than searching the scene for it
    public void receiveGodAndIdentity(RoomHandler constructor, int Id)
    {
        this.constructor = constructor;
        this.Id = Id;
    }
    public RoomHandler StealGod()
    {
        return constructor;
    }

    //If we hit an obstacle, bad stuff happens
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "obstacle")
            ReduceHealth(obstacleHealthReduction);
        if (other.tag == "mine")
            ReduceHealth(mineHealthReduction);
    }

    public void ReduceHealth(int reduceHealthBy)
    {
        roomHealth -= reduceHealthBy;
        if (roomHealth <= 0)
            HandleRoomDestruction();
    }

    void HandleRoomDestruction()
    {
        (bool canDestroy, List<BaseRoom> targets) targetsOfDestruction = TargetsCanBeDestroyed();
        foreach (var room in adjacentRooms)
            room.adjacentRooms.Remove(this);

        foreach (var room in adjacentRooms)
        {
            Vector3 position = room.transform.position - gameObject.transform.position;
            constructor.CloseDoor(position);
        }

        if (targetsOfDestruction.canDestroy)
        {
            foreach (var room in targetsOfDestruction.targets)
                DestroyRoom(room);
        }
        else
            DestroyRoom(this);
    }

    void DestroyRoom(BaseRoom roomToDestroy)
    {
        foreach (var room in roomToDestroy.adjacentRooms)
            room.adjacentRooms.Remove(roomToDestroy);
        Destroy(roomToDestroy.gameObject);
    }
    (bool, List<BaseRoom>) TargetsCanBeDestroyed()
    {
        constructor.ReassignRoomDistancesAtFrameEnd();
        Queue<BaseRoom> roomQueue = new Queue<BaseRoom>();
        List<BaseRoom> queuedRooms = new List<BaseRoom>();
        HashSet<BaseRoom> roomFilter = new HashSet<BaseRoom>();
        List<BaseRoom> targets = new List<BaseRoom>();
        //Add the first room
        roomQueue.Enqueue(this);
        roomFilter.Add(this);

        bool firstRoom = true;
        //As long as there are rooms in the queue
        while(roomQueue.Count > 0)
        {
            //Remove the rooms from the queue and add them to a list of queued rooms
            queuedRooms.Add(roomQueue.Dequeue());
            //When the queue runs out of rooms, execute our behaviour on this level of the tree
            if(roomQueue.Count == 0)
            {
                //Foreach room at this level
                foreach(var room in queuedRooms)
                {
                    //Foreach room adjacent to our current room instance
                    foreach(var roomAdjacent in room.adjacentRooms)
                    {
                        //If the current adjacent room is in the filter, we skip it
                        if (roomFilter.Contains(roomAdjacent))
                            continue;
                        //if the room adjacent to this room has a lower distance, it is connected to the core, return canDestroy false and targets null
                        //If this is the first room, then do NOT do this
                        if (roomAdjacent.roomDistanceFromCenter < roomDistanceFromCenter)
                            if (firstRoom)
                            {
                                targets.Add(room);
                                continue;
                            }
                            else
                                return (false, null);
                        else
                        {
                            targets.Add(room);
                            roomQueue.Enqueue(roomAdjacent);
                            if (roomAdjacent.adjacentRooms.Count == 1)
                                targets.Add(roomAdjacent);
                        }
                    }
                    if(!roomFilter.Contains(room))
                        roomFilter.Add(room);
                }
                firstRoom = false;
                queuedRooms.Clear();
            }
        }
        return(true, targets);
    }

}
