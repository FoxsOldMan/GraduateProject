using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGAmmo : MonoBehaviour
{
    public float lifeTime = 1f;
    public float damage = 10f;
    public float explosionRange = 5f;
    [HideInInspector]
    public bool working;
    [HideInInspector]
    public Transform master;

    private Collider activeCollider;
    private GameObject ammoModel;
    private AudioSource audioSource;
    private ParticleSystem trailEffect;
    private ParticleSystem explosionEffect;

    private void Awake()
    {
        activeCollider = GetComponent<Collider>();
        ammoModel = transform.Find("Model").gameObject;
        audioSource = GetComponent<AudioSource>();
        trailEffect = transform.Find("Trail").GetComponent<ParticleSystem>();
        explosionEffect = transform.Find("Explosion").GetComponent<ParticleSystem>();

        working = false;
        activeCollider.enabled = false;
        ammoModel.SetActive(false);
    }

    public void Active()
    {
        working = true;
        activeCollider.enabled = true;
        ammoModel.SetActive(true);
        trailEffect.Play();
        transform.SetAsLastSibling();//激活后放在对象池末尾

        StartCoroutine(MoveForward());
    }

    public void Deactive()
    {
        working = false;
        activeCollider.enabled = false;
        ammoModel.SetActive(false);
        trailEffect.Stop();
    }

    private void Explosion()
    {
        Deactive();
        explosionEffect.Play();
        audioSource.Play();

        List<Vulnerable> vulnerables = new List<Vulnerable>();
        Collider[] others = Physics.OverlapSphere(transform.position, explosionRange);

        foreach(var obj in others)
        {
            Vulnerable vulnerable = obj.GetComponentInParent<Vulnerable>();
            if(vulnerable != null && !vulnerables.Contains(vulnerable))
            {
                vulnerables.Add(vulnerable);
                if(vulnerable.GotHitted(damage, transform) && vulnerable.AcceptHitBack())
                {
                    Vector3 dir = obj.transform.position - transform.position;
                    obj.GetComponentInParent<Rigidbody>().AddForce(obj.GetComponentInParent<Rigidbody>().mass * dir.normalized * 30, ForceMode.Impulse);
                }
            }
        }
        transform.SetAsFirstSibling();//爆炸后处于非工作状态，置于对象池最前
    }

    private IEnumerator MoveForward()
    {
        float startTime = Time.time;
        while (working && Time.time - startTime <= lifeTime)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 30, Space.Self);
            yield return new WaitForEndOfFrame();
        }
        if (working)
            Deactive();

        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Light"))
            Explosion();
        Debug.Log(other.name);
    }
}
