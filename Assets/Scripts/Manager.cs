using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //[SerializeField] Checks[] checkpoints;
    [SerializeField] int current = 0;
    [SerializeField] Vector3 spawnLoc;

    GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        spawnLoc = player.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) GoToCheck();
    }

    public void CheckThis(int serial, Vector3 te, GameObject go)
    {
        if (serial == current + 1)
        {
            current++;
            spawnLoc = te;
            Destroy(go);
        }
    }

    public void GoToCheck()
    {
        player.transform.position = spawnLoc;
        player.GetComponent<PlayerMovement>().Reset();
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
