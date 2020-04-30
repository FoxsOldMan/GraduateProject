using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, Vulnerable
{
    [HideInInspector]
    public float hp;
    public float maxHP;

    public bool hitBackable = true;

    [HideInInspector]
    public bool isAlive;


    protected Vector3 velocity;

    [Min(0.1f)]
    [SerializeField]
    protected float speed = 3;

    [Range(1,20f)]
    [SerializeField]
    protected float turnSpeed = 7;

    private Rigidbody thisRigidbody;


    protected virtual void Awake()
    {
        hp = maxHP;
        isAlive = true;
        thisRigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        if (velocity.magnitude > 0)
        {
            Turn(velocity);
            thisRigidbody.MovePosition(thisRigidbody.position + velocity * Time.fixedDeltaTime);
        }
    }

    public virtual void Movement(Vector3 move)
    {
        if (move.magnitude > 1)
            move.Normalize();

        velocity = move * speed;
    }

    public virtual void Turn(float angle)
    {

    }

    public virtual void Turn(Vector3 dir)
    {
        Quaternion targetRot = Quaternion.LookRotation(dir);
        thisRigidbody.MoveRotation(Quaternion.Slerp(thisRigidbody.rotation, targetRot, turnSpeed*0.01f));
    }

    public virtual bool GotHitted(float DMG, Transform from)
    {
        if (hp > 0)
        {
            if(hp < DMG)
                hp = 0;
            else
                hp -= DMG;

            AliveCheck();
            return true;
        }
        return false;
    }

    public bool AcceptHitBack()
    {
        if (hitBackable)
        {
            if (GetComponent<Rigidbody>() != null)
                return true;

            Debug.Log(transform.name + " 缺少RigidBody，无法击退");
        }
  

        return false;
    }

    protected void AliveCheck()
    {
        if(hp <= 0 && isAlive)
        {
            isAlive = false;
        }
        else
        {
            isAlive = true;
        }

    }

}
