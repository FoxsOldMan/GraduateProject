using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAndMovement : MonoBehaviour, Vulnerable
{
    public float maxHP = 10;
    [HideInInspector]
    public float curHP;

    [Range(1, 20f)]
    [SerializeField]
    protected float turnSpeed = 12;

    public Transform holdPoint;

    public Equipment oilTank;
    public Equipment axe;
    public Equipment pistol;
    public Equipment shotGun;
    public Equipment RPG;

    public List<Equipment> equipmentList;
    [HideInInspector]
    public int curEquipment;

    private Rigidbody thisRigidbody;
    private Animator animator;
    private Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
        curEquipment = 1;
        equipmentList = new List<Equipment>();
        equipmentList.Add(oilTank);
        equipmentList.Add(axe);
        equipmentList.Add(pistol);
        equipmentList.Add(shotGun);
        equipmentList.Add(RPG);


        for (int i = 0; i < equipmentList.Count; i++)
        {
            equipmentList[i].master = transform;
            equipmentList[i].Close();
            if(equipmentList[i].type != EquipmentType.Place)
            {
                equipmentList[i].transform.SetParent(holdPoint);
                equipmentList[i].transform.localPosition = Vector3.zero;
                equipmentList[i].transform.localRotation = Quaternion.Euler(0, 90, 90);
            }
            else
            {
                equipmentList[i].transform.SetParent(transform);
                equipmentList[i].transform.localPosition = Vector3.zero;
                equipmentList[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            //Debug.Log(equipmentList[i].name);
        }

        thisRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        colliders = GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //AcceptInput();//此为测试用，实际调用在Controller
        //SwithEquipment();
        UpdateHandPose();
        if (!isAlive())
        {
            Debug.Log("Dead");
            //Destroy(this);
            this.enabled = false;
        }
    }

    private void SwithEquipment()
    {
        if (animator.GetBool("Attacking"))
        {
            return;
        }
        equipmentList[curEquipment].Close();
        if (Input.GetKeyDown(KeyCode.Q) && curEquipment > 0)
            curEquipment--;
        if (Input.GetKeyDown(KeyCode.E) && curEquipment < equipmentList.Count-1)
            curEquipment++;

        //curEquipment = Mathf.Clamp(curEquipment, 0, equipmentList.Count);
        equipmentList[curEquipment].Active();
    }

    private void UpdateHandPose()
    {
        if(equipmentList[curEquipment].type == EquipmentType.Ranged)
        {
            animator.SetBool("HoldGun", true);
        }
        else
        {
            animator.SetBool("HoldGun", false);
        }
    }

    private bool UseEquipment()
    {
        switch (equipmentList[curEquipment].type)
        {
            case EquipmentType.Melee:
                break;
            case EquipmentType.Place:
                break;
            case EquipmentType.Ranged:
                if (equipmentList[curEquipment].Work())
                    return true;
                break;
        }

        return false;
    }

    public void AcceptInput()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        MoveForward(dir);

        SwithEquipment();

        if (Input.GetKeyDown(KeyCode.J))
        {
            if(equipmentList[curEquipment].type != EquipmentType.Melee)
                equipmentList[curEquipment].Work();
            else
            {
                MeleeAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("Tumble");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            animator.SetTrigger("GotHitted");
        }

        animator.SetBool("SlowDown", Input.GetKey(KeyCode.LeftShift));
        

    }

    //检查是否可进行转向、移动，被攻击、翻滚时无法移动转向
    private bool Actionable()
    {
        if (animator.GetBool("Tumbling"))
            return false;

        if (animator.GetBool("GotHitting"))
            return false;

        return true;
    }

    private bool MeleeAttack()
    {
        if (equipmentList[curEquipment].type != EquipmentType.Melee)
        {
            Debug.Log("非近战武器");
            return false;
        }

        animator.SetTrigger("Attack");
        return true;
    }

    public void StartMeleeAttackHit()
    {
        animator.SetBool("AttackHit", true);
        equipmentList[curEquipment].Work();
        StartCoroutine(StopAxeHit());
    }

    public void StopMeleeAttackHit()
    {
        animator.SetBool("AttackHit", false);
    }

    public void MoveForward(Vector3 moveDir)
    {
        if (!Actionable())
            return;

        if (moveDir.magnitude > 0)
        {
            animator.SetBool("Walk", true);
            Turn(moveDir);

        }
        else
        {
            animator.SetBool("Walk", false);
        }

    } 

    private void Turn(Vector3 dir)
    {
        Quaternion targetRot = Quaternion.LookRotation(dir);
        //thisRigidbody.MoveRotation(Quaternion.Slerp(thisRigidbody.rotation, targetRot, turnSpeed * 0.01f));
        transform.rotation = Quaternion.Slerp(thisRigidbody.rotation, targetRot, turnSpeed * 0.01f);
    }

    public void SlowDown(bool value)
    {
        animator.SetBool("SlowDown", value);
    }

    public void TumbleAction()
    {
        //StartCoroutine(SetIsTrigger());
        Debug.Log("Tumbling");
    }

    //翻滚时取消重力
    private IEnumerator SetIsTrigger()
    {
        thisRigidbody.useGravity = false;
        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    colliders[i].isTrigger = true;
        //}

        while (animator.GetBool("Tumbling"))
        {
            yield return new WaitForEndOfFrame();
        }

        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    colliders[i].isTrigger = false;
        //}
        thisRigidbody.useGravity = true;
        yield break;
    }

    private IEnumerator StopAxeHit()
    {
        while (animator.GetBool("AttackHit"))
        {
            yield return new WaitForEndOfFrame();
        }

        axe.Stop();
        yield break;
    }

    public bool isAlive()
    {
        if (curHP > 0)
            return true;

        return false;
    }

    public bool GotHitted(float DMG, Transform from)
    {
        Debug.Log("Attack from "+from.gameObject.name);
        if (animator.GetBool("Tumbling"))
            return false;

        else if (curHP > 0)
        {
            if (curHP < DMG)
                curHP = 0;
            else
                curHP -= DMG;

            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(from.position - transform.position,Vector3.up));
            animator.SetTrigger("GotHitted");
            //Debug.Log("GotHitted");
            return true;
        }

        return false;
    }

    public bool AcceptHitBack()
    {
        return false;
    }
}

