using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : Character
{
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        Movement(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        base.FixedUpdate();
    }

}
