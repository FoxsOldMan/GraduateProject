using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationTip : MonoBehaviour
{
    public Transform destination;
    public GameObject follow;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.transform.position + offset;
        transform.rotation = Quaternion.LookRotation(destination.position - transform.position, Vector3.up);
    }
}
