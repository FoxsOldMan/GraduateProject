using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBossStateAndAction : MonoBehaviour, Vulnerable
{
    public float maxHP;
    [HideInInspector]
    public float hp;

    [HideInInspector]
    public bool isAlive;
    //public bool hitBackable;

    public Collider breathAttackCollider;
    public Collider yowlAttackCollider;
    public ParticleSystem yowlEffect;
    public ParticleSystem breathOfFireEffect;

    private float damage;
    private Animator animator;
    private List<Collider> triggerColliders;
    private List<Vulnerable> triggerVulnerables;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (breathAttackCollider == null)
            Debug.Log("breathAttackCollider缺失");
        else
            breathAttackCollider.enabled = false;

        if (yowlAttackCollider == null)
            Debug.Log("breathAttackCollider缺失");
        else
            yowlAttackCollider.enabled = false;

        if (yowlEffect == null)
            Debug.Log("YowlEffect缺失");

        if (breathOfFireEffect == null)
            Debug.Log("breathOfFireEffect缺失");

        hp = maxHP;
        isAlive = true;
        damage = 10f;
        triggerColliders = new List<Collider>();
        triggerVulnerables = new List<Vulnerable>();
        //hitBackable = true;

    }

    private void Update()
    {
        AliveCheck();
        if (!isAlive)
        {
            animator.SetTrigger("Die");
            Debug.Log(gameObject.name + " die");
        }
    }

    public bool StartYowlEffect()
    {
        if(yowlEffect != null)
        {
            yowlEffect.Play();
            return true;
        }

        return false;
    }

    public bool StartBreathOfFireEffect()
    {
        if (breathOfFireEffect != null)
        {
            breathOfFireEffect.Play();
            return true;
        }

        return false;
    }

    public bool StopBreathOfFireEffect()
    {
        if (breathOfFireEffect != null)
        {
            breathOfFireEffect.Stop();
            return true;
        }

        return false;
    }

    public void ActiveBreathAttackCollider()
    {
        //刷新
        triggerColliders.Clear();
        triggerVulnerables.Clear();

        breathAttackCollider.enabled = true;
        StartBreathOfFireEffect();
        Debug.Log("触发breathAttack");
    }

    public void DeactiveBreathAttackCollider()
    {
        breathAttackCollider.enabled = false;
        StopBreathOfFireEffect();
        Debug.Log("结束breathAttack");
    }

    public void ActiveYowlAttackCollider()
    {
        //刷新
        triggerColliders.Clear();
        triggerVulnerables.Clear();

        yowlAttackCollider.enabled = true;
        StartYowlEffect();
        Debug.Log("触发YowlAttack");
    }

    public void DeactiveYowlAttackCollider()
    {
        yowlAttackCollider.enabled = false;
        Debug.Log("结束YowlAttack");

    }

    public void HandAttack()
    {
        //LayerMask layer = LayerMask.GetMask("Enemy");
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
                while(objHitted.GetComponent<Vulnerable>() == null)
                {
                    objHitted = objHitted.parent;
                }

                if(vulnerable.GotHitted(damage,transform) && vulnerable.AcceptHitBack())
                {
                    Vector3 force = (objHitted.position - transform.position).normalized;
                    objHitted.GetComponent<Rigidbody>().AddForce(objHitted.GetComponent<Rigidbody>().mass * force * 30, ForceMode.Impulse);
                }

                Debug.Log("Attack:" + objHitted.gameObject.name);
            }

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
        //if (hitBackable)
        //{
        //    if (GetComponent<Rigidbody>() != null)
        //        return true;

        //    Debug.Log(transform.name + " 缺少RigidBody，无法击退");
        //}


        return false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (triggerColliders.Contains(other))
            return;

        triggerColliders.Add(other);

        if (other.gameObject.layer == LayerMask.NameToLayer("Light"))
            return;

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

        //Old Version
        //Transform rootParent = other.transform;
        //while (rootParent.parent != null)
        //    rootParent = rootParent.parent;

        //if (rootParent.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        //{
        //    Vector3 forceDirection = Vector3.ProjectOnPlane(rootParent.position - transform.position, Vector3.up).normalized;
        //    rootParent.GetComponent<Rigidbody>().AddForce(rootParent.GetComponent<Rigidbody>().mass * forceDirection * 50, ForceMode.Impulse);

        //    Debug.Log("Attack: " + rootParent.name);

        //    breathAttackCollider.enabled = false;
        //    yowlAttackCollider.enabled = false;
        //}
        //else
        //{
        //    Debug.Log(rootParent.name + ">Layer:" + rootParent.gameObject.layer + " Layermask" + LayerMask.GetMask("Enemy"));
        //}


        Debug.Log(other.name+"触发器触发");
    }
}
