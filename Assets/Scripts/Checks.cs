using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checks : MonoBehaviour
{
    public Vector3 te;
    public int serial;
    [SerializeField] private float height;

    Manager man;


    private void Start()
    {
        te = transform.position;
        man = GameObject.Find("Manager").GetComponent<Manager>();
    }
    void Update()
    {
        Vector3 temp = te;
        temp.y += Mathf.Sin(Time.time) * height;
        transform.position = temp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            man.CheckThis(serial, te, gameObject);
        }
    }
}
