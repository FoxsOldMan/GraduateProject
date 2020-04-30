using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    //private Rigidbody rigidbody;
    //private ParticleSystem ps;

    void Start()
    {
        //Debug.Log(transform.forward);

        //Debug.Log(Vector3.Cross(Vector3.forward, Vector3.right));

        //rigidbody = GetComponent<Rigidbody>();
        //Debug.Log(rigidbody.position);

        //ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up, 3,Space.Self);
        //Debug.Log(transform.forward);

        //transform.position += Vector3.forward*0.01f;
    }


    //private void OnParticleCollision(GameObject other)
    //{
    //    Debug.Log("碰撞到:" + other.name);

    //}


    private void OnParticleTrigger()
    {
        //List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        //int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        //for (int i = 0; i < numEnter; i++)
        //{
        //    ParticleSystem.Particle p = enter[i];
        //    p.startColor = Color.red;
        //    enter[i] = p;
        //}
        //ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        //if (numEnter > 0)
        //{
        //    Debug.Log("触发");
        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " 触发："+other.name);
    }


}
