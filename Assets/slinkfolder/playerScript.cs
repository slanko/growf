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

    public void getMovementInputs(InputAction.CallbackContext context)
    {
        moveVals = context.ReadValue<Vector2>();
        if (!noMove) anim.SetFloat("xVal", moveVals.x);
        else anim.SetFloat("xVal", 0);
        anim.SetFloat("xValNegative", -anim.GetFloat("xVal"));
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
    }
}
