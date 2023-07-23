using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundSpeedScript : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] godscript god;
    void Update()
    {
        anim.SetFloat("speedMult", god.speedMult);
    }
}
