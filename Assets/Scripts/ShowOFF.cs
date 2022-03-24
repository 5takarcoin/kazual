using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOFF : MonoBehaviour
{
    public GameObject text;
    public bool toDo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) text.SetActive(toDo);
    }
}
