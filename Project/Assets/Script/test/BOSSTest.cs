using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSTest : MonoBehaviour
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("KEY_W_DOWN", true);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            animator.SetBool("KEY_W_DOWN", false);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetBool("KEY_S_DOWN", true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            animator.SetBool("KEY_S_DOWN", false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("BreathOfFire");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("HandAttack");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetTrigger("WhirlSlam");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            animator.SetTrigger("Yowl");
        }
    }
}
