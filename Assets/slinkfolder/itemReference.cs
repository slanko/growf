using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemReference : MonoBehaviour
{
    public itemScriptableObject myItem;
    [SerializeField] SpriteRenderer mySprite;

    private void Start()
    {
        mySprite.sprite = myItem.mySprite;
    }

}
