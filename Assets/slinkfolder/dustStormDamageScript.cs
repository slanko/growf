using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dustStormDamageScript : MonoBehaviour
{
    [SerializeField] float damageRate;
    [SerializeField] LayerMask roomLayer;

    // Start is called before the first frame update
    void Start()
    {
        //RECURSION??!?!??!
        StartCoroutine(damageRandomRoom());
    }

    IEnumerator damageRandomRoom()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation, roomLayer);
        //there should only be rooms with the uhh room script in here
        if(colliders.Length > 0)
        {
            colliders[Random.Range(0, colliders.Length)].GetComponent<BaseRoom>().ReduceHealth(1);
        }
        yield return new WaitForSeconds(damageRate);
        StartCoroutine(damageRandomRoom());
    }
}
