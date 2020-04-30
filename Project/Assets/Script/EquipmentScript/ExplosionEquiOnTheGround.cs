using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEquiOnTheGround : MonoBehaviour, Vulnerable
{
    public float explosionRange = 5f;
    public float damage = 10f;
    [HideInInspector]
    public bool working;
    [HideInInspector]
    public Transform master;

    protected GameObject model;
    protected Collider activeCollider;
    protected AudioSource audioSource;
    protected ParticleSystem explisionEffect;

    private void Awake()
    {
        model = transform.Find("Model").gameObject;
        activeCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        explisionEffect = transform.Find("Explosion").GetComponent<ParticleSystem>();

        Deactive();
    }

    public void Active()
    {
        working = true;
        model.SetActive(true);
        activeCollider.enabled = true;
        transform.SetAsLastSibling();//激活后放在对象池末尾
    }
    public void Deactive()
    {
        working = false;
        model.SetActive(false);
        activeCollider.enabled = false;
    }

    private void Explosion()
    {
        Deactive();
        audioSource.Play();
        explisionEffect.Play();

        List<Vulnerable> vulnerables = new List<Vulnerable>();
        Collider[] others = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (var obj in others)
        {
            Vulnerable vulnerable = obj.GetComponentInParent<Vulnerable>();
            if (vulnerable != null && !vulnerables.Contains(vulnerable))
            {
                vulnerables.Add(vulnerable);
                if (vulnerable.GotHitted(damage, transform) && vulnerable.AcceptHitBack())
                {
                    Vector3 dir = obj.transform.position - transform.position;
                    obj.GetComponentInParent<Rigidbody>().AddForce(obj.GetComponentInParent<Rigidbody>().mass * dir.normalized * 30, ForceMode.Impulse);
                }
            }
        }
    }

    public bool AcceptHitBack()
    {
        return false;
    }

    public bool GotHitted(float DMG, Transform from)
    {
        Explosion();
        return true;
    }
}
