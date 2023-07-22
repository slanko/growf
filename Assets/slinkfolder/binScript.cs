using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static itemScriptableObject;

public class binScript : MonoBehaviour
{
    [SerializeField]
    List<recipeEntry> recipe;
    [SerializeField] TextMeshPro myText;

    [System.Serializable]
    public struct recipeEntry
    {
        public itemType entryType;
        public int amountNeeded;

    }

    private void Start()
    {
        myText.text = generateList();
    }

    public bool acceptItem(itemScriptableObject.itemType theItem)
    {
        bool toReturn = false;
        
        for(int i = 0; i < recipe.Count; i++)
        {
            if (recipe[i].entryType == theItem)
            {
                toReturn = true;
                recipeEntry copyEntry;
                copyEntry.entryType = recipe[i].entryType;
                copyEntry.amountNeeded = recipe[i].amountNeeded - 1;
                recipe[i] = copyEntry;
            }
        }
        myText.text = generateList();
        if (checkFulfilment()) Debug.Log("WAHOO WAHOO YOU DID IT!!! YOU DID IT!!! YES!@!!!");
        return toReturn;
    }

    string generateList()
    {
        string theList = "";
        foreach(recipeEntry entry in recipe)
        {
            theList += entry.entryType.ToString() + " x" + entry.amountNeeded + "\n";
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
