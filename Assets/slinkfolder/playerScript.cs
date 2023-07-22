using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour
{
    [SerializeField] Animator anim;

    [SerializeField] float moveSpeed;

    bool noMove;
    Vector2 moveVals;
    Rigidbody rb;

    Transform gameCam;

    [SerializeField] LayerMask itemLayer, binLayer;

    //item stuff
    [SerializeField] Transform itemPoint;
    itemScriptableObject.itemType currentItem;
    Transform itemTransform;


    public void getMovementInputs(InputAction.CallbackContext context)
    {
        moveVals = context.ReadValue<Vector2>();
        if (!noMove) 
        {
            anim.SetFloat("xVal", moveVals.x);
            anim.SetFloat("moveMagnitude", moveVals.magnitude);
        }
        else
        {
            anim.SetFloat("xVal", 0);
            anim.SetFloat("moveMagnitude", 0);
        }
        anim.SetFloat("moveMagnitudeNegative", -anim.GetFloat("moveMagnitude"));
    }

    public void pickUpObject(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentItem == itemScriptableObject.itemType.NOTHING)
            {
                Collider[] contacts = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z), 1.5f, itemLayer);
                foreach (Collider thing in contacts)
                {
                    if (thing.gameObject.tag == "item")
                    {
                        currentItem = thing.gameObject.GetComponent<itemReference>().myItem.myType;
                        itemTransform = thing.transform;
                        thing.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                }
            }
            else
            {
                Rigidbody rb2 = itemTransform.gameObject.GetComponent<Rigidbody>();
                rb2.constraints = RigidbodyConstraints.None;
                rb2.velocity = rb.velocity;
                itemTransform = null;
                currentItem = itemScriptableObject.itemType.NOTHING;
            }
        }

    }

    public void useButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("USE!!");
            if (currentItem != itemScriptableObject.itemType.NOTHING)
            {
                Collider[] contacts = Physics.OverlapSphere(transform.position, .6f, binLayer);
                foreach (Collider thing in contacts)
                {
                    binScript theBin = thing.GetComponent<binScript>();
                    if (theBin.acceptItem(currentItem))
                    {
                        currentItem = itemScriptableObject.itemType.NOTHING;
                        Destroy(itemTransform.gameObject);
                        itemTransform = null;
                    }
                }
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameCam = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float currentYVelocity = rb.velocity.y;
        Vector3 moveVector = new Vector3(moveVals.x, 0, moveVals.y);
        if (moveVector.magnitude != 0 && !noMove)
        {
            Vector3 velToSet = transform.TransformDirection(moveVector) * moveSpeed;
            velToSet.y = currentYVelocity;
            rb.velocity = velToSet;
        }

        //item snapping
        if (itemTransform != null)
        {
            itemTransform.position = itemPoint.position;
            itemTransform.rotation = itemPoint.rotation;
        }
        }
    }
