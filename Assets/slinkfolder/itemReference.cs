using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemReference : MonoBehaviour
{
    public itemScriptableObject myItem;
    [SerializeField] SpriteRenderer mySprite;
    Transform cam;
    [SerializeField] Transform spriteTransform;

    private void Start()
    {
        mySprite.sprite = myItem.mySprite;
        cam = GameObject.Find("CamBuddy/Main Camera").transform;
    }

    private void Update()
    {
        spriteTransform.LookAt(cam);
    }



}
