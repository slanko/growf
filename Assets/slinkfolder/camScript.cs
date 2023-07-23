using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScript : MonoBehaviour
{
    public List<Transform> playerTransforms;
    [SerializeField] float lerpSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransforms.Count > 0)
        {
            Vector3 totalVector = new Vector3();
            Vector3 midpoint = new Vector3();
            foreach (Transform player in playerTransforms)
            {
                totalVector += player.position;
            }
            midpoint = totalVector / playerTransforms.Count;

            transform.position = Vector3.Lerp(transform.position, midpoint, lerpSpeed * Time.deltaTime);

        }
    }
}
