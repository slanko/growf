using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    [SerializeField]
    GameObject roomTemplate = null;

    [SerializeField]
    GameObject Core = null;

    int idValue = 1000;

    [SerializeField]
    Material DoorClosed;
    [SerializeField]
    Material DoorOpen;

    private void Start()
    {
        Core.GetComponent<BaseRoom>().receiveGodAndIdentity(this, idValue++);
    }

    //Receive the command to build a room and the direction to check/build for
    public void CreateRoom(string direction, BaseRoom room)
    {
        if (direction == "North")
            FireCast(Vector3.forward, room);

        if (direction == "East")
            FireCast(-Vector3.right, room);

        if (direction == "South")
            FireCast(-Vector3.forward, room);

        if (direction == "West")
            FireCast(Vector3.right, room);
    }

    void FireCast(Vector3 direction, BaseRoom room)
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("RoomCast");
        //TODO: Determine if can be removed
        if (!Physics.Raycast(room.transform.position, direction * 3, out hit, 3, mask))
        {
            Debug.DrawRay(room.transform.position, direction * 3, Color.green, 100);
            ConstructRoom(direction, room);
        }
    }


    void ConstructRoom(Vector3 direction, BaseRoom providedRoom)
    {
        var newRoom = Instantiate(roomTemplate, providedRoom.transform.position + (direction * 3), Quaternion.identity).GetComponent<BaseRoom>();
        newRoom.gameObject.name = $"Room {idValue}";
        providedRoom.adjacentRooms.Add(newRoom);
        newRoom.receiveGodAndIdentity(this, idValue++);

        //Get radius of room to edge
        //Enables finding rooms adjacent to this room without getting diagonals
        float radius = Mathf.Sqrt(1.55f * 1.55f);
        LayerMask mask = LayerMask.GetMask("RoomCast");

        Collider[] adjacentColliders = Physics.OverlapSphere(newRoom.transform.position, radius, mask);
        foreach (var col in adjacentColliders)
        {
            var instance = col.gameObject.GetComponent<BaseRoom>();
            if (instance != newRoom)
                newRoom.adjacentRooms.Add(instance);

            Debug.Log(col.gameObject.name);
        }

        if (newRoom.adjacentRooms.Count > 1)
        {
            foreach(var room in newRoom.adjacentRooms)
            {
                if (!room.adjacentRooms.Contains(newRoom))
                    room.adjacentRooms.Add(newRoom);
            }
            AssignRoomValueFromAdjacents(newRoom);
        }
        else
            newRoom.roomDistanceFromCenter = providedRoom.roomDistanceFromCenter + 1;

        //Get the positional difference between the two rooms (The middle point)
        Vector3 position = newRoom.transform.position - providedRoom.transform.position;
        OpenDoor(position);
    }


    #region Door Handling
    void OpenDoor(Vector3 position)
    {
        LayerMask mask = LayerMask.GetMask("WallLayer");
        Collider[] doors = Physics.OverlapSphere(position, 0.5f, mask);
        foreach(var item in doors)
        {
            //If item is not door or frame
            if (!item.gameObject.name.Contains("Door"))
                continue;

            //if item is frame, set material, else if door, deactivate
            if (item.gameObject.name.Contains("Frame"))
                item.GetComponent<MeshRenderer>().material = DoorOpen;
            else
                item.gameObject.SetActive(false);
        }
    }

    public void CloseDoor(Vector3 position)
    {
        LayerMask mask = LayerMask.GetMask("WallLayer");
        Collider[] doors = Physics.OverlapSphere(position, 0.5f, mask);
        foreach (var item in doors)
        {
            //If item is not door or frame
            if (!item.gameObject.name.Contains("Door"))
                continue;

            //if item is frame, set material, else if door, deactivate
            if (item.gameObject.name.Contains("Frame"))
                item.GetComponent<MeshRenderer>().material = DoorClosed;
            else
                item.gameObject.SetActive(true);
        }
    }

    public void ReassignRoomDistancesAtFrameEnd()
    {
        StartCoroutine(ReassignDistanceFromCoreQueued());
    }

    #endregion

    #region Room Distance Assignments
    void AssignRoomValueFromAdjacents(BaseRoom newRoom)
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
                ReassignDistanceFromJoinRecursively(newRoom);
            }
        }
    }

    //Dives down each branch until all have been updated
    void ReassignDistanceFromJoinRecursively(BaseRoom callingRoom)
    {
        foreach (var room in callingRoom.adjacentRooms)
        {
            if (room.roomDistanceFromCenter > callingRoom.roomDistanceFromCenter)
            {
                room.roomDistanceFromCenter = callingRoom.roomDistanceFromCenter + 1;
                ReassignDistanceFromJoinRecursively(room);
            }
        }
    }

    //Occurs when a delete is triggered
    IEnumerator ReassignDistanceFromCoreQueued()
    {
        yield return new WaitForEndOfFrame();
        List<BaseRoom> roomFilter = new List<BaseRoom>();
        Queue<BaseRoom> rooms = new Queue<BaseRoom>();
        rooms.Enqueue(Core.GetComponent<BaseRoom>());
        roomFilter.Add(Core.GetComponent<BaseRoom>());
        List<BaseRoom> queuedRooms = new List<BaseRoom>();
        int distanceFromCore = 1;
        Debug.Log("Room distance calculation beginning...");
        while (rooms.Count > 0)
        {

            queuedRooms.Add(rooms.Dequeue());

            if (rooms.Count == 0)
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
                    }
                }
                distanceFromCore++;
            }
        }
        Debug.Log("Room distances recalculated");
    }
    #endregion
}
