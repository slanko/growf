using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleScript : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    godscript GOD;

    [SerializeField] LayerMask collisionLayers;
    [SerializeField] Transform collisionBox;
    [SerializeField] QueryTriggerInteraction triggerInteraction;

    // Start is called before the first frame update
    void Start()
    {
        GOD = GameObject.Find("GOD").GetComponent<godscript>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime * GOD.speedMult);

        Collider[] colliders = Physics.OverlapBox(transform.position, collisionBox.localScale / 2, collisionBox.rotation, collisionLayers, triggerInteraction);
        if(colliders.Length > 0)
        {
            if (colliders[0] != null)
            {
                //if it has a baseroom script, hit it and die.
                BaseRoom room = colliders[0].GetComponent<BaseRoom>();
                if (room != null)
                {
                    room.ReduceHealth(1);
                    Debug.Log("Obstacle Hit!");
                    Destroy(this.gameObject);
                }
            }
        }

        if (transform.position.z < -20) Destroy(this.gameObject);
    }

    public void kill()
    {
        Destroy(this.gameObject);
    }
}
