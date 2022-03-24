using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    public float distanceToCover;
    public Vector3 goti;

    Vector3 firstPos;

    public Vector3 cholarGoti;

    PlayerMovement pm;

    Vector3 lpos;

    private void Start()
    {
        firstPos = transform.position;
        cholarGoti = goti;

        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        lpos = pm.ledgePosition;
    }

    private void Update()
    {
        float rakhlam = Vector3.Distance(firstPos, transform.position);
        if (rakhlam >= distanceToCover) cholarGoti = goti * -1;
        if (rakhlam <= .1) cholarGoti = goti;

        transform.position += cholarGoti * Time.deltaTime;

        if(lpos != Vector3.zero)
        {
            Collider[] hehe = Physics.OverlapSphere(lpos, 0.3f);

            if (hehe.Length > 0)
            {
                for (int i = 0; i < hehe.Length; i++)
                {
                    if (hehe[i].CompareTag("platos")) pm.ledgePosition += cholarGoti;
                }
                
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.parent = transform;
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.transform.parent = null;
    }
}
