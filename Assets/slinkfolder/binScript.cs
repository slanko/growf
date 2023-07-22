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
    bool recipeSet = false;

    [System.Serializable]
    public struct recipeEntry
    {
        public itemType entryType;
        public int amountNeeded;

    }

    private void Start()
    {

    }

    public bool acceptItem(itemScriptableObject theItem)
    {
        bool toReturn = false;

        if (!recipeSet)
        {
            recipe.Clear();
            foreach(recipeEntry entry in theItem.recipe)
            {
                recipeEntry newEntry = new recipeEntry();
                newEntry.entryType = entry.entryType;
                newEntry.amountNeeded = entry.amountNeeded;
                recipe.Add(newEntry);
            }
            recipeSet = true;
            toReturn = true;
            myText.text = generateList();
        }
        else
        {
            for (int i = 0; i < recipe.Count; i++)
            {
                if (recipe[i].entryType == theItem.myType)
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
        }
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
