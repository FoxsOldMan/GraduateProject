using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBossStateAndAction : MonoBehaviour, Vulnerable
{
    public float maxHP;
    [HideInInspector]
    public float hp;

    [HideInInspector]
    public bool isAlive = true;

    public Collider rangeAttackCollider;
    public ParticleSystem breathOfFireEffect;
    public ParticleSystem trampleEffect;
    public ParticleSystem yowlEffect;

    private float damage;
    private List<Collider> triggerColliders;
    private List<Vulnerable> triggerVulnerables;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (rangeAttackCollider == null)
            Debug.Log("rangeAttackCollider缺失");
        else
            rangeAttackCollider.enabled = false;

        if (trampleEffect == null)
            Debug.Log("trampleEffect缺失");

        if (breathOfFireEffect == null)
            Debug.Log("breathOfFireEffect缺失");

        if (yowlEffect == null)
            Debug.Log("yowlEffect缺失");

        hp = maxHP;
        isAlive = true;
        triggerColliders = new List<Collider>();
        triggerVulnerables = new List<Vulnerable>();
    }

    // Update is called once per frame
    void Update()
    {
        AliveCheck();
        if (!isAlive)
        {
            animator.SetTrigger("Die");
            Debug.Log(gameObject.name + " die");
        }
    }

    //启动yowl特效,并更改damage
    public bool StartYowl()
    {
        if (yowlEffect != null)
        {
            yowlEffect.Play();
            damage = 0;
            ActiveRangeAttackCollider();
            return true;
        }
        return false;
    }

    public bool StopYowl()
    {
        if (yowlEffect != null)
        {
            yowlEffect.Stop();
            DeactiveRangeAttackCollider();
            return true;
        }
        return false;
    }
    //启动BreathAttack特效,并更改damage
    public bool StartBreathOfFire()
    {
        if (breathOfFireEffect != null)
        {
            breathOfFireEffect.Play();
            damage = 10;
            ActiveRangeAttackCollider();
            return true;
        }

        return false;
    }

    public bool StopBreathOfFire()
    {
        if (breathOfFireEffect != null)
        {
            breathOfFireEffect.Stop();
            DeactiveRangeAttackCollider();
            return true;
        }

        return false;
    }
    //启动Tample特效,并更改damage
    public bool StartTrample()
    {
        if (trampleEffect != null)
        {
            trampleEffect.Play();
            damage = 20;
            ActiveRangeAttackCollider();
            return true;
        }

        return false;
    }

    public bool StopTrample()
    {
        if (trampleEffect != null)
        {
            trampleEffect.Stop();
            DeactiveRangeAttackCollider();
            return true;
        }

        return false;
    }

    private void ActiveRangeAttackCollider()
    {
        triggerColliders.Clear();
        triggerVulnerables.Clear();
        rangeAttackCollider.enabled = true;
    }

    private void DeactiveRangeAttackCollider()
    {
        rangeAttackCollider.enabled = false;
    }

    public void CloseRangeAttack()
    {
        List<Vulnerable> vulnerables = new List<Vulnerable>();
        Collider[] objs = Physics.OverlapSphere(transform.position, 5);
        foreach (var obj in objs)
        {
            if (obj.transform.IsChildOf(transform))
                continue;

            //判定是否在前方90°角内
            Vector3 difference = Vector3.ProjectOnPlane(obj.transform.position - transform.position, Vector3.up);
            if (Vector3.Angle(transform.forward, difference) > 45)
                continue;

            Vulnerable vulnerable = obj.GetComponentInParent<Vulnerable>();
            if (vulnerable != null && !vulnerables.Contains(vulnerable))
            {
                vulnerables.Add(vulnerable);
                Transform objHitted = obj.transform;
                while (objHitted.GetComponent<Vulnerable>() == null)
                {
                    objHitted = objHitted.parent;
                }

                if (vulnerable.GotHitted(damage, transform) && vulnerable.AcceptHitBack())
                {
                    Vector3 force = (objHitted.position - transform.position).normalized;
                    objHitted.GetComponent<Rigidbody>().AddForce(objHitted.GetComponent<Rigidbody>().mass * force * 30, ForceMode.Impulse);
                }

                Debug.Log("Attack:" + objHitted.gameObject.name);
            }

        }
    }

    private void AliveCheck()
    {
        if (hp <= 0 && isAlive)
        {
            isAlive = false;
        }
        else
        {
            isAlive = true;
        }
    }

    public bool GotHitted(float DMG, Transform from)
    {
        if (hp > 0)
        {
            if (hp < DMG)
                hp = 0;
            else
                hp -= DMG;

            return true;
        }
        return false;
    }

    public bool AcceptHitBack()
    {
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerColliders.Contains(other))
            return;

        triggerColliders.Add(other);

        Vulnerable vulnerable = other.GetComponentInParent<Vulnerable>();
        if (vulnerable != null && !triggerVulnerables.Contains(vulnerable))
        {
            triggerVulnerables.Add(vulnerable);
            Transform objHitted = other.transform;
            while (objHitted.GetComponent<Vulnerable>() == null)
            {
                objHitted = objHitted.parent;
            }

            if (vulnerable.GotHitted(damage, transform) && vulnerable.AcceptHitBack())
            {
                Vector3 force = (objHitted.position - transform.position).normalized;
                objHitted.GetComponent<Rigidbody>().AddForce(objHitted.GetComponent<Rigidbody>().mass * force * 30, ForceMode.Impulse);
            }

            Debug.Log("Attack:" + objHitted.gameObject.name);
        }

        Debug.Log("触发器触发");
    }
}
