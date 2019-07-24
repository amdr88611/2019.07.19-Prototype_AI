using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum Enemy
    {
        Patrol,
        Impact,
        Attack,
        Run,
        Dead
    }
    public Enemy EnemyStatus;

    NavMeshAgent Agent; //導航
    Animator Anim;

    [Header("玩家")]
    public Transform CurTarget;     //玩家
    public int Hp = 100;
    [Header("攻擊距離")]
    public float AttackRange = 3;   //攻擊距離
    [Header("攻擊頻率")]
    public float AttackRate = 2;    //攻擊頻率
    [Header("敵人攻擊判定")]
    public GameObject DamageCollider;
    [Header("巡邏地點")]
    public Transform[] points;
    private int DestPoint = 0; //巡邏順位


    float AttTimer;  //計時器
    float Distance;  //玩家與敵人之距離
    bool AttackOnce;
    bool StopRotating;
    


    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        Agent.stoppingDistance = AttackRange;
        //Patrol();
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
            EnemyStatus = Enemy.Impact;

        switch (EnemyStatus)
        {
            case Enemy.Patrol:
                Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
                Anim.SetFloat("Move", relDirection.z, 0.5f, Time.deltaTime);

                if (!Agent.pathPending && Agent.remainingDistance < AttackRange)
                    Patrol();
                break;
            case Enemy.Impact:
                ImpactHandler();
                break;
            case Enemy.Attack:
                AttackHandler();
                break;
            case Enemy.Run:
                MovementHandler();
                break;
            case Enemy.Dead:
                Anim.SetBool("Dead",true);
                break;
            default:
                break;
        }
    }
    //移動判定
    void MovementHandler()
    {
        if (CurTarget != null)
        {
            //轉向玩家
            Vector3 dir = CurTarget.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);

            //導航至玩家
            Agent.SetDestination(CurTarget.position);
            //移動動畫
            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Anim.SetFloat("Move", relDirection.z, 0.5f, Time.deltaTime);

            //敵人跟玩家之間的距離
            Distance = Vector3.Distance(transform.position, CurTarget.position);

            if (Distance <= AttackRange)
            {
                AttTimer += Time.deltaTime;
                if (AttTimer > AttackRate)
                {
                    AttTimer = 0;
                    EnemyStatus = Enemy.Attack;
                }
            }
        }
    }
    //攻擊判定
    void AttackHandler()
    {            
        //敵人跟玩家之間的距離
        Distance = Vector3.Distance(transform.position, CurTarget.position);

        if (Distance > AttackRange)
        {
            EnemyStatus = Enemy.Run;
        }
        /*if (distance < 0.8f )
        {
            int a = Random.Range(0, 11);
            if(a < 2)
            Anim.SetTrigger("Backward");
        }*/
        if (!StopRotating)
        {
            Vector3 dir = CurTarget.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);

            float angle = Vector3.Angle(transform.forward, dir);
            //敵人面向玩家角度小於6時
            if (angle < 6)
            {
                if (!AttackOnce)
                {
                    Agent.isStopped = true;
                    int i = Random.Range(1, 5);
                    Anim.SetTrigger("Attack" + i);
                    StartCoroutine("CloseAttack");
                    AttackOnce = true;
                }
            }
        }
        else
        {
            Vector3 dir = CurTarget.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10);

            Anim.SetBool("LeftMove",true);
        }

    }
    IEnumerator CloseAttack()
    {
        int i = Random.Range(3, 8);
        yield return new WaitForSeconds(i);
        //Anim.SetBool("Attack", false);
        AttackOnce = false;
        Agent.isStopped = false;
        StopRotating = false;
        Anim.SetBool("LeftMove", false);
    }
    //受擊判定
    void ImpactHandler()
    {
        Anim.SetTrigger("Hit");
        if (Hp == 0)
            EnemyStatus = Enemy.Dead;
        else
            Hp -= 10;
            EnemyStatus = Enemy.Patrol;
    }
    //巡邏
    void Patrol()
    {
        if (points.Length > AttackRange)
            return;

        Agent.destination = points[DestPoint].position;
        DestPoint = (DestPoint + 1) % points.Length;

    }
    //動畫事件
    public void AttackEventsStart()
    {
        StopRotating = true;
        DamageCollider.GetComponent<BoxCollider>().enabled = true;
    }
    public void AttackEventsEnd()
    {
        StopRotating = true;
        DamageCollider.GetComponent<BoxCollider>().enabled = false;
    }

}
