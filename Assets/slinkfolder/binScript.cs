using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static itemScriptableObject;
using UnityEngine.Events;

public class binScript : MonoBehaviour
{
    [SerializeField] RoomBaseObject myRoom;
    TestConstruction constructor;
    [SerializeField] bool presetRecipe, noList;

    [SerializeField]
    List<recipeEntry> recipe;
    [SerializeField] TextMeshPro myText;
    bool recipeSet = false;
    string recipeName;
    public itemType firstItem;
    GameObject selectedRoomPrefab;

    [Header("Room Construction Specific")]
    public bool checkAdjacency;
    [SerializeField, Header("EAST AND WEST ARE FLIPPED!!")]
    string direction;
    [SerializeField] LayerMask binLayer, roomCastLayer;
    [SerializeField] QueryTriggerInteraction triggerInteraction;

    [SerializeField]
    UnityEvent myEvent;

    [System.Serializable]
    public struct recipeEntry
    {
        public itemType entryType;
        public int amountNeeded;

    }

    private void Start()
    {
        if (noList) myText.text = "";
        else myText.text = "MATERIAL\nv";
        constructor = GameObject.Find("GOD").GetComponent<TestConstruction>();
        if(checkAdjacency) checkIfAdjacent();
    }

    public bool acceptItem(itemScriptableObject theItem)
    {
        bool toReturn = false;

        if (!recipeSet && !presetRecipe)
        {
            recipe.Clear();
            recipeName = theItem.recipeName;
            foreach(recipeEntry entry in theItem.recipe)
            {
                recipeEntry newEntry = new recipeEntry();
                newEntry.entryType = entry.entryType;
                newEntry.amountNeeded = entry.amountNeeded;
                recipe.Add(newEntry);
                firstItem = theItem.myType;
                if(theItem.instantiateRoom != null) selectedRoomPrefab = theItem.instantiateRoom;
            }
            recipeSet = true;
            toReturn = true;
            myText.text = generateList();
        }
        else
        {
            for (int i = 0; i < recipe.Count; i++)
            {
                if (recipe[i].entryType == theItem.myType && recipe[i].amountNeeded > 0)
                {
                    toReturn = true;
                    recipeEntry copyEntry;
                    copyEntry.entryType = recipe[i].entryType;
                    copyEntry.amountNeeded = recipe[i].amountNeeded - 1;
                    recipe[i] = copyEntry;
                }
            }
            myText.text = generateList();
        }
        if (checkFulfilment())
        {
            myEvent.Invoke();
            Destroy(this.gameObject);
        }
        return toReturn;
    }

    string generateList()
    {
        string theList = "";
        if (!noList)
        {
            theList += "<u>" + recipeName + "</u>\n";
            foreach (recipeEntry entry in recipe)
            {
                if (entry.amountNeeded > 0)
                    theList += entry.entryType.ToString() + " x" + entry.amountNeeded + "\n";
            }
        }

        return theList;
    }

    bool checkFulfilment()
    {
        bool toReturn = true;
        foreach(recipeEntry entry in recipe)
        {
            if (entry.amountNeeded > 0) toReturn = false;
        }
        return toReturn;
    }

    public void passConstructionInfo()
    {
        constructor.CreateRoom(direction, myRoom);
    }

    void checkIfAdjacent()
    {
        //two raycast system. first checks for other bins and deletes them. then the bin checks for rooms (if there are other bins there are probably rooms but hey) and then if there is it deletes itself.
        Vector3 castDirection = new Vector3();
        switch (direction)
        {
            case "North": castDirection = Vector3.forward; break;
            case "South": castDirection = Vector3.back; break;
                //these two are wrong but it's because they're wrong in a script i can't edit. life is just like this
            case "East": castDirection = Vector3.left; break;
            case "West": castDirection = Vector3.right; break;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, castDirection, out hit, 2f, binLayer, triggerInteraction))
        {
            if(hit.collider != null)
            {
                if (hit.collider.GetComponent<binScript>().checkAdjacency) Destroy(hit.collider.gameObject);
            }
        }
        if (Physics.Raycast(transform.position, castDirection, out hit, 2f, roomCastLayer, triggerInteraction))
        {
            if (hit.collider != null) Destroy(this.gameObject);
        }
    }

    public void spawnPrefab()
    {
        
        if(selectedRoomPrefab != null) Instantiate(selectedRoomPrefab, myRoom.transform.position, myRoom.transform.rotation, myRoom.transform);
    }

}
