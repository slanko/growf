using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundItemScript : MonoBehaviour
{
    public itemScriptableObject myObject;
    [SerializeField] SpriteRenderer mySprite;
    godscript GOD;

    [SerializeField] float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        GOD = GameObject.Find("GOD").GetComponent<godscript>();
        mySprite.sprite = myObject.mySprite;
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime * GOD.speedMult);
        if (transform.position.z < -20) Destroy(this.gameObject);
    }
}
