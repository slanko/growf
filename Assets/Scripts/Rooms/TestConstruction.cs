using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System;

public class TestConstruction : MonoBehaviour
{
    [SerializeField]
    GameObject roomTemplate = null;

    [SerializeField]
    GameObject Core = null;
    [SerializeField]
    GameObject Selected = null;
    int idValue = 1000;

    // Start is called before the first frame update
    void Start()
    {
        Selected = Core;
    }


    void ConstructRoom(Vector3 direction)
    {
        //Array of directions for iteration and getting adjacency
        Vector3[] rayDirections = { transform.forward, -transform.forward, transform.right, -transform.right };

        //Create new room with reference
        var newRoom = Instantiate(roomTemplate, Selected.transform.position + (direction*3), transform.rotation).GetComponent<RoomBaseObject>();
        newRoom.Id = idValue++;
        
        //fire ray in cardinal directions to get newRoom's adjacents
        foreach(var dir in rayDirections)
        {
            Debug.DrawRay(Selected.transform.position, direction * 3, Color.yellow, 10);
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("RoomCast");
            if(Physics.Raycast(newRoom.transform.position, dir, out hit, 1.5f, mask))
            {
                Debug.DrawRay(newRoom.transform.position, dir * 1.5f, Color.red, 10);
                newRoom.adjacentRooms.Add(hit.transform.gameObject.GetComponent<RoomBaseObject>());
                hit.transform.gameObject.GetComponent<RoomBaseObject>().adjacentRooms.Add(newRoom);
            }
        }

        //If multiple adjacencies, iterate through all to find and assign the lowest number + 1
        if (newRoom.adjacentRooms.Count > 1)
        {
            int lowestRoomValue = 1000;
            foreach (var room in newRoom.adjacentRooms)
            {
                if (room.roomDistanceFromCenter < lowestRoomValue)
                    lowestRoomValue = room.roomDistanceFromCenter;
            }
            newRoom.roomDistanceFromCenter = lowestRoomValue + 1;
            foreach (var room in newRoom.adjacentRooms)
            {
                int result = lowestRoomValue > room.roomDistanceFromCenter ?
                    lowestRoomValue - room.roomDistanceFromCenter : room.roomDistanceFromCenter - lowestRoomValue;
                if (result > 1)
                {
                    ReassignDistanceValuesFromJoin(newRoom);
                }
            }
        }
        //Else get the value + 1
        else
        {
            newRoom.roomDistanceFromCenter = Selected.GetComponent<RoomBaseObject>().roomDistanceFromCenter + 1;
        }

        Selected = newRoom.gameObject;
    }


    void ReassignDistanceValuesFromJoin(RoomBaseObject callingRoom)
    {
        foreach (var room in callingRoom.adjacentRooms)
        {
            if (room.roomDistanceFromCenter > callingRoom.roomDistanceFromCenter)
            {
                room.roomDistanceFromCenter = callingRoom.roomDistanceFromCenter + 1;
                ReassignDistanceValuesFromJoin(room);
            }
        }
    }
    [SerializeField]
    GameObject obj;
    private void Update()
    {
        obj.transform.position = Selected.transform.position;
    }

    IEnumerator ReassignDistanceFromCoreQueued()
    {
        yield return new WaitForFixedUpdate();
        List<RoomBaseObject> roomFilter = new List<RoomBaseObject>();
        Queue<RoomBaseObject> rooms = new Queue<RoomBaseObject>();
        rooms.Enqueue(Core.GetComponent<RoomBaseObject>());
        roomFilter.Add(Core.GetComponent<RoomBaseObject>());
        List<RoomBaseObject> queuedRooms = new List<RoomBaseObject>();
        int distanceFromCore = 1;
        Core.GetComponent<RoomBaseObject>().MarkedForDeath = false;
        while (rooms.Count > 0){

            queuedRooms.Add(rooms.Dequeue());

            if(rooms.Count == 0)
            {
                foreach (var room in queuedRooms)
                {
                    foreach (var element in room.adjacentRooms)
                    {
                        if (roomFilter.Contains(element))
                            continue;
                        element.roomDistanceFromCenter = distanceFromCore;
                        rooms.Enqueue(element);
                        roomFilter.Add(element);
                        element.MarkedForDeath = false;
                    }
                }
                distanceFromCore++;
            }
        }
        print("Reassignment");
    }


 

    void FireCast(Vector3 direction)
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("RoomCast");
        if(Physics.Raycast(Selected.transform.position, direction * 3, out hit, 3, mask))
        {
            Debug.DrawRay(Selected.transform.position, direction * 3, Color.green, 100);
            Selected = hit.transform.gameObject;
        }
        else
        {
            Debug.DrawRay(Selected.transform.position, direction * 3, Color.green, 100);
            ConstructRoom(direction);
        }
    }

    public void DamageRoom(InputAction.CallbackContext context)
    {
        if (context.performed && Selected != Core)
        {
            print("hit");
            Selected.GetComponent<RoomBaseObject>().ReduceHealth(100);
            StartCoroutine(ReassignDistanceFromCoreQueued());
            Selected = Core;
        }
    }

    public void GetInputs(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var value = Path.GetFileName(context.ToString());
            if (value[0] == 'w')
                FireCast(Vector3.forward);

            if (value[0] == 'a')
                FireCast(-Vector3.right);

            if (value[0] == 's')
                FireCast(-Vector3.forward);

            if (value[0] == 'd')
                FireCast(Vector3.right);
        }
    }
}
