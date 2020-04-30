using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBossTurnToTarget : Action
{
    public SharedTransform target;
    public float turnSpeed = 2f;
    public float moveSpeed = 3f;
    private float rotationOffset = 10f;

    private Animator animator;


    // 判断是否面对目标
    bool IsFacingTarget()
    {
        if (target.Value == null)
        {
            return false;
        }
        Vector3 dir = target.Value.position - transform.position;
        dir.y = 0;
        if (Vector3.Angle(transform.forward, dir) < rotationOffset)
        {
            return true;
        }
        return false;
    }

    // 转向目标，每次只转一点，速度受turnSpeed控制,同时向目标前进
    void RotateToTarget()
    {
        animator.SetBool("GoForward", false);
        Debug.Log("转向");
        if (target.Value == null)
        {
            return;
        }
        Vector3 distance = target.Value.position - transform.position;
        distance.y = 0;
        Vector3 cross = Vector3.Cross(transform.forward, distance);//unity是左手坐标系

        if(Vector3.Dot(cross, Vector3.up) >= 0)
        {
            animator.SetBool("TurnRight", true);
        }
        else
        {
            animator.SetBool("TurnLeft", true);
        }

        float angle = Vector3.Angle(transform.forward, distance);
        transform.Rotate(cross, Mathf.Min(2, Mathf.Abs(angle)));
    }

    //通过动画往前，animator参数GoForward，在DistanceDetection中置为false
    void GoForward()
    {
        animator.SetBool("GoForward", true);
        animator.SetBool("TurnLeft", false);
        animator.SetBool("TurnRight", false);
    }

    public override void OnAwake()
    {
        animator = GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (IsFacingTarget())
        {
            GoForward();
            return TaskStatus.Success;
            //return TaskStatus.Running;
        }
        RotateToTarget();
        return TaskStatus.Running;
    }
}

public class DogBossAttack : Action
{
    public string attackName;

    public override TaskStatus OnUpdate()
    {
        if (attackName == null || attackName.Length < 1)
        {
            Debug.Log("未获取攻击名称");
            return TaskStatus.Failure;
        }

        GetComponent<Animator>().SetTrigger(attackName);
        Debug.Log("发动攻击：" + attackName);
        return TaskStatus.Success;
    }
}

//检测是否存活
public class DogBossAliveCheck : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (GetComponent<DogBossStateAndAction>().isAlive)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}

//检测boss是否半血以下
public class DogBossHalfHP : Conditional
{
    Animator animator;
    public override void OnAwake()
    {
        animator = GetComponent<Animator>();
    }
    public override TaskStatus OnUpdate()
    {
        if (animator.GetBool("HalfHP"))
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}

public class DogBossHalfHPAction : Action
{
    DogBossStateAndAction dogBoss;
    Animator animator;
    public override void OnAwake()
    {
        dogBoss = GetComponent<DogBossStateAndAction>();
        animator = GetComponent<Animator>();
    }
    public override TaskStatus OnUpdate()
    {
        if (!animator.GetBool("HalfHP") && dogBoss.hp / dogBoss.maxHP <= 0.5f)
        {
            animator.SetTrigger("Yowl");
            animator.SetBool("HalfHP", true);
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}

// 检测是否在攻击范围内
public class DogBossDistanceDetection : Conditional
{
    public float attackingDistance;
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        Vector3 distance = target.Value.position - transform.position;
        distance.y = 0;
        if (distance.magnitude <= attackingDistance)
        {
            Debug.Log("在攻击范围内");
            GetComponent<Animator>().SetBool("GoForward", false);
            return TaskStatus.Failure;
        }

        Debug.Log("不在攻击范围内");
        return TaskStatus.Success;
    }

}

// 检测目标是否在前方，在前则返回success
public class DogBossIsFacingTarget : Conditional
{
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        Vector3 distance = target.Value.position - transform.position;
        float result = Vector3.Dot(transform.forward, distance);
        if (result > 0)
        {
            Debug.Log("目标在前");
            return TaskStatus.Success;
        }

        Debug.Log("目标在后");
        return TaskStatus.Failure;
    }
}

// 检测目标能否行动
public class DogBossActionableDetection : Conditional
{
    private Animator animator;

    public override void OnAwake()
    {
        animator = GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (animator.GetBool("Attacking") || animator.GetBool("Dead"))
        {
            Debug.Log("DogBOSS无法进行动作");
            return TaskStatus.Failure;
        }

        Debug.Log("DogBOSS空闲，可攻击");
        return TaskStatus.Success;
    }
}


