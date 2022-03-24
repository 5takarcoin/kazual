using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    Manager man;
    PlayerMovement pm;

    private void Start()
    {
        man = GameObject.Find("Manager").GetComponent<Manager>();
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.collider.CompareTag("Player")) man.GoToCheck();
        pm.SetState();
    }
}
