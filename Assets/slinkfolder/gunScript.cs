using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gunScript : MonoBehaviour
{
    [SerializeField] Transform barrelRotator, shootPoint, effectiveArea;
    [SerializeField] LayerMask targetLayer;
    public int ammoMax;
    public int currentAmmo;
    [SerializeField] float shootRate;
    [SerializeField] GameObject bulletLine;
    [SerializeField] TextMeshPro ammoCounter;
    // Start is called before the first frame update
    void Start()
    {
        //RECURSION!!
        StartCoroutine(targetAcquisition());
        currentAmmo = ammoMax;
    }

    IEnumerator targetAcquisition()
    {
        //get all possible targets
        if(currentAmmo > 0)
        {
            Collider[] possibleTargets = Physics.OverlapBox(effectiveArea.position, effectiveArea.localScale / 2, effectiveArea.rotation, targetLayer);
            if (possibleTargets.Length > 0)
            {
                //then pick a random one and SHOOT IT!!
                Transform target = possibleTargets[Random.Range(0, possibleTargets.Length)].transform;
                barrelRotator.LookAt(target.position);
                BulletLineScript bullet = Instantiate(bulletLine).GetComponent<BulletLineScript>();
                bullet.point0Point = shootPoint.position;
                bullet.point1Point = target.position;
                target.gameObject.GetComponent<obstacleScript>().kill();
                currentAmmo--;
                ammoCounter.text = currentAmmo + "";
            }
        }
        yield return new WaitForSeconds(shootRate);
        StartCoroutine(targetAcquisition());
    }

    public void refillAmmo()
    {
        currentAmmo++;
        if (currentAmmo > ammoMax) currentAmmo = ammoMax;
        ammoCounter.text = currentAmmo + "";
    }
}
