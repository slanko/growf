using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour
{

    [SerializeField] Animator anim;

    [SerializeField] float moveSpeed;
    [SerializeField] Transform myVisual, lookAtHelper;
    [SerializeField] float xRotationOffset;
    godscript god;
    bool noMove, fishing;
    Vector2 moveVals;
    Rigidbody rb;

    Transform gameCam;

    [SerializeField] LayerMask itemLayer, binLayer, interactibleLayer, groundItemLayer, roomMaskLayer;
    [SerializeField] QueryTriggerInteraction triggerInteract;
    [SerializeField] int interactCooldownFrames;
    int cooldown;

    bool fishflip;
    [Header("Hunger Stuff"), SerializeField] float hungerMax;
    [SerializeField] float hungerDecayRate, plantRestoreAmount;
    float currentHunger;
    Slider myHungerSlider;
    bool dead;
    [SerializeField] GameObject myHungerDisplay;
    hungerBarScript hunger;

    [Header("Death Stuff"), SerializeField] 
    float roadDeathHeightThreshold;
    [SerializeField] float camRefocusDelay;



    //item stuff
    [SerializeField] Transform itemPoint;
    itemScriptableObject.itemType currentItem;
    Transform itemTransform;

    itemScriptableObject delayItem;

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
                Collider[] contacts = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z), .5f, itemLayer);
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
        if (context.performed && cooldown <= 0)
        {
            bool usedBin = false;
            Debug.Log("USE!!");
            cooldown = interactCooldownFrames;
            if (!fishing)
            {
                //FIRST CHECK - IS THERE A BIN THAT ACCEPTS NOTHING?
                if(currentItem == itemScriptableObject.itemType.NOTHING)
                {
                    Collider[] contacts = Physics.OverlapSphere(transform.position, .6f, binLayer);
                    foreach (Collider thing in contacts)
                    {
                        binScript theBin = thing.GetComponent<binScript>();
                        if (theBin.acceptsNothing)
                        {
                            theBin.forceEvent(this);
                            usedBin = true;
                            break;
                        }
                    }
                }

                //IF HOLDING AN ITEM IT IS USED IN ITEM SPECIFIC STUFF
                if (currentItem != itemScriptableObject.itemType.NOTHING)
                {
                    Collider[] contacts = Physics.OverlapSphere(transform.position, .6f, binLayer);
                    foreach (Collider thing in contacts)
                    {
                        binScript theBin = thing.GetComponent<binScript>();
                        if (theBin.acceptItem(itemTransform.gameObject.GetComponent<itemReference>().myItem, this))
                        {
                            theBin.lastInteractedPlayer = this;
                            currentItem = itemScriptableObject.itemType.NOTHING;
                            Destroy(itemTransform.gameObject);
                            itemTransform = null;
                            usedBin = true;
                            break;
                        }
                    }
                }
                //IF YOU HOLD NOTHING THEN YOU CAN USE THE OTHER STUFF. SPECIFIED HEREIN
                if (currentItem == itemScriptableObject.itemType.NOTHING && !usedBin)
                {
                    Collider[] contacts2 = Physics.OverlapSphere(transform.position, .6f, interactibleLayer);
                    foreach (Collider thing in contacts2)
                    {
                        if (thing.tag == "fishing hole")
                        {
                            if (thing.GetComponent<fishingHelper>().flip) fishflip = true;
                            else fishflip = false;
                            fishing = true;
                            if (!fishflip) anim.Play("fishingIdle");
                            else anim.Play("fishingIdleFlipped");
                            rb.constraints = RigidbodyConstraints.FreezeAll;
                            transform.position = new Vector3(thing.transform.position.x, transform.position.y, thing.transform.position.z);
                            break;
                        }
                    }
                }
            }
            else
            {
                fishingCheck();
                anim.SetTrigger("REEL");
                Invoke("stopFishing", 0.5f);
            }
        }
    }

    public void consumeButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (currentItem)
            {
                case itemScriptableObject.itemType.SAW:
                    Collider[] roomColliders = Physics.OverlapBox(transform.position, new Vector3(.1f, .1f, .1f), Quaternion.identity, roomMaskLayer, triggerInteract);
                    if (roomColliders.Length > 0)
                    {
                        BaseRoom room = roomColliders[0].GetComponent<BaseRoom>();
                        room.ReduceHealth(1);
                        Debug.Log("Saw Used");
                    }
                    break;

                case itemScriptableObject.itemType.PLANT:
                    currentHunger += plantRestoreAmount;
                    currentItem = itemScriptableObject.itemType.NOTHING;
                    Destroy(itemTransform.gameObject);
                    itemTransform = null;
                    break;

                case itemScriptableObject.itemType.METAL:
                    Collider[] roomColliders2 = Physics.OverlapBox(transform.position, new Vector3(.1f, .1f, .1f), Quaternion.identity, roomMaskLayer, triggerInteract);
                    if (roomColliders2.Length > 0)
                    {
                        BaseRoom room = roomColliders2[0].GetComponent<BaseRoom>();
                        if(room.roomHealth < room.roomMaxHealth)
                        {
                            room.ReduceHealth(-1);
                            currentItem = itemScriptableObject.itemType.NOTHING;
                            Destroy(itemTransform.gameObject);
                            itemTransform = null;
                        }
                        Debug.Log("Saw Used");
                    }
                    break;
            }
        }
    }

    public void startGame(InputAction.CallbackContext context)
    {
        god.startGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameCam = GameObject.Find("CamBuddy/Main Camera").transform;
        GameObject.Find("CamBuddy").GetComponent<camScript>().playerTransforms.Add(transform); //when you die do this in reverse after a delay.
        god = GameObject.Find("GOD").GetComponent<godscript>();
        transform.position = god.getSpawnPoint().position;
        setUpHungerBar();
        currentHunger = hungerMax;
        god.livingPlayers++;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float currentYVelocity = rb.velocity.y;
        Vector3 moveVector = new Vector3(moveVals.x, 0, moveVals.y);
        if (moveVector.magnitude != 0 && !noMove && !fishing)
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
        interactCooldownFrames--;
        if (interactCooldownFrames < 0) interactCooldownFrames = 0;
    }

    void Update()
    {
        lookAtHelper.LookAt(gameCam);
        myVisual.eulerAngles = new Vector3(lookAtHelper.eulerAngles.x + xRotationOffset, lookAtHelper.eulerAngles.y, 0);
        if(god.gameStarted) hungerLogic();
        if (transform.position.y < roadDeathHeightThreshold)
        {
            if (!dead) die();
            else transform.Translate(Vector3.back * 33 * god.speedMult * Time.deltaTime); //33 is the magic road speed number that syncs with the animation. very sorry
        }
    }
    
    void stopFishing()
    {
        fishing = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        if(delayItem != null)
        {
            getItem(delayItem);
            delayItem = null;
        }
    }

    void fishingCheck()
    {
        Collider[] contacts = Physics.OverlapSphere(transform.position, 2, groundItemLayer);
        if (contacts.Length != 0)
        {
            delayItem = contacts[0].GetComponent<groundItemScript>().myObject;
            Destroy(contacts[0].gameObject);
        }

    }

    public void getItem(itemScriptableObject myObject)
    {
        GameObject newObject = Instantiate(myObject.itemObject);
        currentItem = newObject.gameObject.GetComponent<itemReference>().myItem.myType;
        itemTransform = newObject.transform;
        newObject.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    void setUpHungerBar()
    {
        hunger = Instantiate(myHungerDisplay, GameObject.Find("UI/Hunger Bar Zone").transform).GetComponent<hungerBarScript>();
        myHungerSlider = hunger.mySlider;
    }

    void hungerLogic()
    {
        if (!dead)
        {
            if (currentHunger > hungerMax) currentHunger = hungerMax;
            currentHunger -= hungerDecayRate * Time.deltaTime;
            if (currentHunger <= 0) die();
        }
        myHungerSlider.value = currentHunger;
    }

    void die()
    {
        anim.SetBool("dead", true);
        noMove = true;
        dead = true;
        god.livingPlayers--;
        if (god.livingPlayers > 0) Invoke("camStopLookingAtMe", camRefocusDelay);
        hunger.playerDied();
    }

    void camStopLookingAtMe()
    {
        GameObject.Find("CamBuddy").GetComponent<camScript>().playerTransforms.Remove(transform);
    }
    


}


