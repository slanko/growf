using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game Specific/Item Data")]
public class itemScriptableObject : ScriptableObject
{
    public enum itemType
    {
        NOTHING,
        METAL,
        FUEL,
        SAW,
        BULLET,
        MINE,
        PLANT,
        ELECTRONICS
    }



    public itemType myType;
    public Sprite mySprite;
    public List<binScript.recipeEntry> recipe;


}
