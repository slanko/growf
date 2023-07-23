using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class RoomBaseObject : MonoBehaviour
{
    public List<RoomBaseObject> adjacentRooms = new List<RoomBaseObject>();
    public int roomDistanceFromCenter;
    protected int roomMaxHealth = 2;
    protected int roomHealth = 2;
    protected bool isUpgraded;
    protected Upgrade upgrade = null;
    [SerializeField]
    public int Id;
    public bool MarkedForDeath = false;
    [SerializeField]
    GameObject[] upgradePrefabs;

    TestConstruction constructor;


    
    // Start is called before the first frame update
    void Start()
    {
        constructor = GameObject.Find("GOD").GetComponent<TestConstruction>();
    }

    async void CleanupEvaluation(RoomBaseObject[] adjacentRooms, List<RoomBaseObject> roomFilter)
    {
        foreach(var room in adjacentRooms)
        {
            if (roomFilter.Contains(room))
                continue;
            roomFilter.Add(room);
            room.MarkedForDeath = true;
            print($"{room.Id} marked for death state is {room.MarkedForDeath}");
            CleanupEvaluation(room.adjacentRooms.ToArray(), roomFilter);
        }
        await Task.Delay(150);
        Debug.Break();
        PerformCleanup(roomFilter.Where(room => room.MarkedForDeath == true).ToArray());
    }
    void PerformCleanup(RoomBaseObject[] allRooms)
    {
        DestroyChain(allRooms.Where(room=>room.Id != 0).ToList());
    }

    
    void DestroyChain(List<RoomBaseObject> rooms)
    {
        foreach(var room in rooms)
        {
            foreach (var adjRoom in room.adjacentRooms)
            {
                adjRoom.adjacentRooms.Remove(room);
            }
            if(room != null)
            {
                Destroy(room.upgrade);
                Destroy(room.gameObject);
            }
        }
    }
    void DestroyRoom()
    {
        foreach (var room in adjacentRooms)
            room.adjacentRooms.Remove(this);
        Destroy(upgrade);
        Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        constructor.beginEvaluation();
    }

    void AnalyseDestruction()
    {
        var requirements = ChainEvaluation(this, true);
        requirements.roomsToDestroy.Reverse();
        if (requirements.canDestroy)
        {
            DestroyChain(requirements.roomsToDestroy);
        }
        else
        {
            DestroyRoom();
        }
        CleanupEvaluation(this.adjacentRooms.ToArray(), new List<RoomBaseObject>());
    }

    //on room destruction, analyse the chain for other elements to destroy
    (bool canDestroy, List<RoomBaseObject> roomsToDestroy) ChainEvaluation(RoomBaseObject caller, bool firstRoom)
    {
        //if no adjacent rooms, assume destruction enabled
        (bool state, List<RoomBaseObject> destructionTargets) stateTargets = (true, new List<RoomBaseObject>());
        foreach(var room in adjacentRooms)
        {
            //If the room is the caller, we skip it (prevents rooms analyzing each other)
            if (room == caller)
            {
                stateTargets.destructionTargets.Add(this);
                continue;
            }

            //If the next room's distance is greater than this room's
            if (room.roomDistanceFromCenter > this.roomDistanceFromCenter)
            {
                //Set a bool and gameobject list equal to an evaluation of the next part of the chain
                stateTargets = room.ChainEvaluation(this, false);
                //Add this room to the list of destruction targets when returning from recursive calls
                stateTargets.destructionTargets.Add(this);
            }

            //Otherwise there is another branch connecting this chain
            else
            {
                //If this is the first room in the analysis chain
                if (firstRoom)
                {
                    //Skip to the next element in the loop
                    stateTargets.destructionTargets.Add(caller);
                    continue;
                }
                return (false, new List<RoomBaseObject>());
            }
        }
        return stateTargets;
    }

    public void HandleUpgradeBin(itemScriptableObject item)
    {
        Instantiate(item.instantiateRoom, this.transform.position,  Quaternion.identity);
        this.roomMaxHealth = 4;
        this.roomHealth *= 2;
    }

    public void SetUpgrade(Upgrade upgrade)
    {
        this.upgrade = upgrade;
    }


    public void ReduceHealth(int reduceBy)
    {
        roomHealth -= reduceBy;
        if (roomHealth <= 0)
        {
            AnalyseDestruction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "mine")
            ReduceHealth(4);
        if (other.tag == "obstacle")
            ReduceHealth(1);
    }

}
