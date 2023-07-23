using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static itemScriptableObject;
using UnityEngine.Events;

public class binScript : MonoBehaviour
{
    [SerializeField] RoomBaseObject myRoom;

    [SerializeField] bool presetRecipe, noList;

    [SerializeField]
    List<recipeEntry> recipe;
    [SerializeField] TextMeshPro myText;
    bool recipeSet = false;
    string recipeName;
    public itemType firstItem;

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


}
