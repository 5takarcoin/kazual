using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetsTry : MonoBehaviour
{
    PlayerMovement pm;
    Platforms pfs;

    bool dhokse;

    Vector3 jog;

    public LayerMask layer;

    private void Start()
    {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pfs = GetComponent<Platforms>();

        jog = pfs.cholarGoti;
    }

    private void Update()
    {
        if (pm.dekhoTo)
        {
            if (Physics.OverlapSphere(pm.ledgePosition, 0.3f, layer).Length > 0) dhokse = true;
            else dhokse = false;
        }

        if (dhokse && pm.dekhoTo)
        {
            pm.ledgePosition += jog * Time.deltaTime;
            pm.gameObject.transform.parent = pfs.gameObject.transform;
        }
    }

    

}
