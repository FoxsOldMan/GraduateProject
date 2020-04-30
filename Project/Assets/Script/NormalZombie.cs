using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : Character, Photoreceptor, Hearable
{
    //view
    public Transform viewPoint;

    public float detectionScope;
    public float attackScope;

    [Range(10,180)]
    [SerializeField]
    public float scopeAngle = 60;
    private float scopeSectorsAmount;//检测射线数

    //voice
    private AudioSource audioSource;
    public AudioClip alertAudio;
    public AudioClip attackAudio;

    //Animator
    private Animator animator;

    //state
    private EnemyState state;

    //UI
    public EnemyStateUI enemyStateUI;
    private Transform uiPos;

    //PatrolOrStandby
    public bool isPatrol;
    public GameObject patrolPath;
    [HideInInspector]
    public List<Vector3> patrolPosList;
    private int patrolCount;
    private Vector3 originPos;
    private Quaternion originRot;

    //Alert
    private Vector3 alertPos;

    [Range(0.1f, 1f)]
    [SerializeField]
    public float alertResponseTime = 0.5f;

    [Range(1,10f)]
    [SerializeField]
    public float alertTime = 4f;
    private float alertTimeLeft;

    //Attack
    private GameObject attackedTarget;

    [Range(1, 10f)]
    [SerializeField]
    public float additionSpeed = 2;

    [Range(1,10f)]
    [SerializeField]
    public float damage = 5;

    [Range(0.1f, 1f)]
    [SerializeField]
    public float attackResponseTime = 0.5f;

    [Range(0.1f,3f)]
    [SerializeField]
    public float attackCDTime = 1.5f;
    private float attackCDLeft;

    //visual
    public bool illuminationActive;
    private bool illuminated;
    //public List<Illuminant> illuminents;
    //private int lastIlluminentCount;
    private Renderer[] renderers;
    private int illuminatedFrame = -100;

    //other
    private float posError = 0.2f;              //position误差
    private float waitTime;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        uiPos = transform.Find("UIPos").transform;
        scopeSectorsAmount = scopeAngle / 3f;

        if (isPatrol && patrolPath != null)
        {
            patrolPosList = new List<Vector3>();
            for (int i = 0; i < patrolPath.transform.childCount; i++)
            {
                patrolPosList.Add(patrolPath.transform.GetChild(i).position);
            }
            patrolCount = 0;
        }

        originPos = transform.position;
        originRot = transform.rotation;
        illuminated = true;
        //illuminents = new List<Illuminant>();
        //lastIlluminentCount = illuminents.Count;
        renderers = GetComponentsInChildren<Renderer>();
        //Debug.Log("renderers"+renderers.Length);
        state = EnemyState.StandbyOrPatrol;

        if (enemyStateUI != null)
        {
            enemyStateUI.transform.SetParent(uiPos);
            enemyStateUI.transform.localPosition = Vector3.zero;
        }

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected void Update()
    {
        if (!isAlive)
        {
            animator.SetTrigger("Die");
            if(enemyStateUI != null)
                enemyStateUI.ShowDead();

            this.enabled = false;
            StartCoroutine(DestroyBody());
        }

        velocity = Vector3.zero;
        UpdateVisual();

        UpdateStateMovement();
        UpdataAnimatorState();

        if (enemyStateUI != null && isAlive)
            UpdateEnemyUI();
    }

    private void UpdateStateMovement()
    {
        if (animator.GetBool("GotHitting"))
            return;

        switch (state)
        {
            case EnemyState.StandbyOrPatrol:
                Detect();
                if (isPatrol)
                    Patrol();
                else
                    Standby();
                break;
            case EnemyState.Alerted:
                Detect();
                Search(alertPos);
                break;
            case EnemyState.Attacking:
                AttackTarget();
                break;
        }
    }

    private void UpdataAnimatorState()
    {
        animator.SetFloat("Speed", velocity.magnitude);

        animator.SetBool("StateOrigin", state == EnemyState.StandbyOrPatrol);
        animator.SetBool("StateAlerted", state == EnemyState.Alerted);
        animator.SetBool("StateAttacking", state == EnemyState.Attacking);
        
    }

    private void UpdateEnemyUI()
    {
        if (enemyStateUI == null)
            return;

        enemyStateUI.SetSate(state);
    }

    public void AddIlluminant(Illuminant illuminant)
    {
        //if (illuminents.Contains(illuminent))
        //    return;
        //else
        //    illuminents.Add(illuminent);
        //Debug.Log("Illuminant:" + illuminant.sourceName + " Illuminanted:" + gameObject.name);
        illuminatedFrame = Time.frameCount;
    }

    public bool IsIlluminated()
    {
        return illuminated;
    }

    private void UpdateVisual()
    {
        //Debug.Log("UpdateVisual");
        //if (illuminents.Count > 0)
        //{
        //    if (lastIlluminentCount > 0)
        //        return;
        //}
        //else if (lastIlluminentCount <= 0)
        //    return; 

        //if (illuminents.Count > 0)
        //    illuminated = true;
        //else
        //    illuminated = false;

        //lastIlluminentCount = illuminents.Count;

        //illuminents.Clear();


        //如果关闭光源接收器，直接显现
        if (!illuminationActive)
        {
            return;
        }

        bool lastIlluminated = illuminated;
        if (illuminatedFrame >= Time.frameCount - 10)
        {
            illuminated = true;
        }
        else
        {
            illuminated = false;
        }

        if(illuminated != lastIlluminated)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = illuminated;
                //Debug.Log(renderers[i].enabled);

            }
        }
    }

    public void Heard(Sound sound)
    {
        
    }

    private void Detect()
    {
        Vector3 originDir = Quaternion.Euler(0, -scopeAngle/2, 0) * viewPoint.forward;
        for (int i = 0; i <= scopeSectorsAmount; i++)
        {
            Vector3 dir = Quaternion.Euler(0, 3 * i, 0) * originDir;

            Ray ray = new Ray(viewPoint.position, dir);

            Vector3 detectRayEnd;
            Vector3 attackRayEnd;

            RaycastHit hit = new RaycastHit();
            int mask = LayerMask.GetMask("Obstacle", "Player");

            detectRayEnd = viewPoint.position + dir * detectionScope;
            attackRayEnd = viewPoint.position + dir * attackScope;
            Physics.Raycast(ray, out hit, float.MaxValue, mask);
            if(hit.transform != null)
            {
                GameObject objHitted = hit.transform.gameObject;
                //Vector3 distance = hit.transform.position - transform.position;
                Vector3 distance = Vector3.ProjectOnPlane(hit.transform.position, Vector3.up) - Vector3.ProjectOnPlane(viewPoint.position, Vector3.up);

                //射线终点设为物体
                if (distance.magnitude < detectionScope)
                {
                    detectRayEnd = hit.point;
                    if (distance.magnitude < attackScope)
                        attackRayEnd = hit.point;
                    
                }

                //射线打到的是Player
                if(objHitted.layer == LayerMask.NameToLayer("Player"))
                {
                    if (distance.magnitude < detectionScope)
                    {
                        if (distance.magnitude < attackScope)
                            SetAttackTarget(objHitted);                 
                        else
                            Alert(objHitted.transform.position);
                    }
                    else //不在警觉范围内，判断角色身上是否有光照
                    {
                        if (objHitted.GetComponent<Photoreceptor>() == null)
                        {
                            Debug.Log("Player 没有实现Photoreceptor接口");
                        }
                        else if (objHitted.GetComponent<Photoreceptor>().IsIlluminated())
                        {
                            Alert(objHitted.transform.position);
                        }
                    }
                }

            }
            Debug.DrawLine(viewPoint.position, detectRayEnd, Color.yellow);
            Debug.DrawLine(viewPoint.position, attackRayEnd, Color.red);
        }
    }

    private void Standby()
    {
        Vector3 difference = Vector3.ProjectOnPlane(originPos - transform.position, Vector3.up);
        if (difference.magnitude > posError)
        {
            Movement(difference);
            animator.SetBool("Checking", false);
        }
        else if(Quaternion.Angle(transform.rotation, originRot) > 1)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, originRot, 1.4f);
            animator.SetBool("Checking", false);

        }
        else if(state == EnemyState.StandbyOrPatrol)
        {
            animator.SetBool("Checking", true);
        }
    }

    private void Patrol()
    {
        if (patrolPosList.Count <= 1)
        {
            return;
        }

        Vector3 difference = Vector3.ProjectOnPlane(patrolPosList[patrolCount] - transform.position, Vector3.up);
        if(difference.magnitude > posError)
        {
            Movement(difference);
        }
        else
        {
            patrolCount = ++patrolCount % patrolPosList.Count;
        }
    }

    //private void Check()
    //{
    //    animator.SetBool("Checking", true);
    //}

    public void Alert(Vector3 position)
    {
        if (state == EnemyState.Attacking)
            return;

        alertPos = position;
        alertTimeLeft = alertTime;
        if(state != EnemyState.Alerted)
        {
            //从其他状态转移至Alerted
            audioSource.clip = alertAudio;
            audioSource.Play();
            waitTime = alertResponseTime;
            state = EnemyState.Alerted;
            animator.SetBool("Checking", false);
            Debug.Log("Checkgin"+animator.GetBool("Checking"));
            //Debug.Log("Alerted");
        }

    }

    private void Search(Vector3 position)
    {
        if(waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }

        Vector3 difference = Vector3.ProjectOnPlane(position - transform.position, Vector3.up);
        if(difference.magnitude > posError)
        {
            if(!animator.GetBool("GotHitted"))
            {
                Movement(difference);
                Debug.Log("move");
            }
        }
        else
        {
            Debug.Log("Searching");
            if (alertTimeLeft > 0.5 * alertTime) //Checkstart
            {
                animator.SetBool("Checking", true);
                alertTimeLeft -= Time.deltaTime;
                //Debug.Log("Searching");
            }
            else if(alertTimeLeft > 0)
            {
                animator.SetBool("Doubting", true);
                alertTimeLeft -= Time.deltaTime;
            }
            else//search end, check end
            {
                animator.SetBool("StateAlerted", false);
                animator.SetBool("Doubting", false);
                animator.SetBool("Checking", false);
                alertTimeLeft -= Time.deltaTime;
                state = EnemyState.StandbyOrPatrol;
                //Debug.Log("Search end, alert end");
            }
        }
    }

    public void SetAttackTarget(GameObject target)
    {
        if(state != EnemyState.Attacking)
        {
            //从其他状态转到attacking
            audioSource.clip = attackAudio;
            audioSource.Play();
            waitTime = attackResponseTime;
            state = EnemyState.Attacking;
            animator.SetBool("Checking", false);
            speed += additionSpeed;
            Debug.Log("Start Attacking");
        }

        attackedTarget = target;
        attackCDLeft = 0;
    }

    public void AttackTarget()
    {
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }

        if (animator.GetBool("Attacking"))
            return;

        Vector3 difference = Vector3.ProjectOnPlane(attackedTarget.transform.position - transform.position, Vector3.up);
        if(attackCDLeft <=0 && difference.magnitude > posError)
        {
            Movement(difference);
        }
        else if(attackCDLeft > 0)
        {
            attackCDLeft -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        LayerMask layer = LayerMask.GetMask("Player");
        List<Transform> vulnerableObj = new List<Transform>();
        Collider[] enemys = Physics.OverlapSphere(viewPoint.position, 5, layer);
        Vector3 difference;
        foreach (var item in enemys)
        {
            //判定是否在前方90°角内
            difference = Vector3.ProjectOnPlane(item.transform.position - transform.position, Vector3.up);
            if (Vector3.Angle(transform.forward, difference) > 45)
                continue;

            //获取带有实现vulnerable接口脚本的ojb
            Transform obj = item.transform;
            while (obj.GetComponent<Vulnerable>() == null && obj.parent != null)
            {
                obj = obj.parent;
            }

            if (!vulnerableObj.Contains(obj))
            {
                vulnerableObj.Add(obj);
                Debug.Log("Attack:" + obj.name);
            }


        }

        foreach (var item in vulnerableObj)
        {
            Vulnerable vulnerable = item.GetComponent<Vulnerable>();
            if (vulnerable.GotHitted(damage, transform) && vulnerable.AcceptHitBack())
            {
                item.GetComponent<Rigidbody>().AddForce(item.GetComponent<Rigidbody>().mass * transform.forward * 200,ForceMode.VelocityChange);
                Debug.Log("Attack: " + item.name);
            }
            //Debug.Log("Attack:" + item.name);
        }
    }

    public override bool GotHitted(float DMG, Transform from)
    {
        if (!isAlive)
            return false;
        if(from != null)
            Alert(from.position);
        animator.SetTrigger("GotHitted");
        return base.GotHitted(DMG, from);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        collision.collider.gameObject.GetComponentInParent<Vulnerable>().GotHitted(damage);
    //        attackCDLeft = attackCDTime;
    //        Debug.Log(gameObject.name + " hit " + collision.collider.name);
    //    }
    //}

    private void OnCollisionStay(Collision collision)
    {

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (animator.GetBool("Attacking"))
                return;

            Vector3 difference = Vector3.ProjectOnPlane(collision.collider.transform.position - transform.position, Vector3.up);
            if (Vector3.Angle(transform.forward, difference) <= 45) //player触碰到enemy，且在enemy前方90°内视野内。
            {
                animator.SetTrigger("Attack");
                Debug.Log("Test");
            }
            //collision.collider.gameObject.GetComponentInParent<Vulnerable>().GotHitted(damage);
            //attackCDLeft = attackCDTime;
            //Debug.Log(gameObject.name + " hit " + collision.collider.name);
        }
        
    }

    private IEnumerator DestroyBody()
    {
        float time = Time.time;
        while (Time.time - time < 3)
            yield return new WaitForEndOfFrame();

        Destroy(gameObject);
        yield break;
    }
}


