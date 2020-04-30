using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffectHit : MonoBehaviour
{
    [Range(1,100)]
    public float bulletDamage = 5;

    [Range(1,100)]
    public float hitForce = 30;

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Hit " + other.name);

        Transform trans = other.transform;
        Vulnerable gotHittable = other.GetComponent<Vulnerable>();
        while(gotHittable == null && trans.parent != null)
        {
            trans = trans.parent;
            gotHittable = trans.GetComponent<Vulnerable>();
        }

        if(gotHittable != null)
        {
            if (gotHittable.GotHitted(bulletDamage, transform) && gotHittable.AcceptHitBack())
            {
                trans.GetComponent<Rigidbody>().AddForce(trans.GetComponent<Rigidbody>().mass * transform.forward * hitForce, ForceMode.Impulse);
                //Debug.Log("Bullet hit " + trans.name);
            }
        }
    }
}
