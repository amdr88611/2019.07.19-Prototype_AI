using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum Enemy
    {
        Patrol,
        Alert,
        Attack,
        Chase,
        Charge,
        Impact,
        Dead
    }
    public Enemy EnemyStatus;

    NavMeshAgent Agent; //導航
    Animator Anim;

    [Header("玩家")]
    public Transform Player;     //玩家
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
    bool IsDead;
    bool IsCharge;

    public GameObject player;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        Agent.stoppingDistance = AttackRange;
        //Patrol();
    }


    void Update()
    {
        //Debug.Log(Distance);
        if (Hp <= 0)
            EnemyStatus = Enemy.Dead;
        if (Input.GetKeyDown(KeyCode.H))
            EnemyStatus = Enemy.Impact;
        if (Player != null)
        {
            Distance = Vector3.Distance(transform.position, Player.position);
        }
        switch (EnemyStatus)
        {
            case Enemy.Patrol: // 巡邏
                Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
                Anim.SetFloat("Move", relDirection.z, 0.5f, Time.deltaTime);

                if (!Agent.pathPending && Agent.remainingDistance < AttackRange)
                    Patrol();
                break;
            case Enemy.Alert: //警戒
                Alert();
                break;
            case Enemy.Attack: //攻擊
                Attack();
                break;
            case Enemy.Chase: //追擊
                Chasing();
                break;
            case Enemy.Charge: //突刺
                Charge();
                break;
            case Enemy.Impact: //受擊
                Impact();
                break;
            case Enemy.Dead: // 死亡
                if (!IsDead)
                {
                    Anim.SetTrigger("Dead");
                    IsDead = true;
                }
                break;
            default:
                break;
        }
    }
    //追擊
    void Chasing()
    {
        Anim.SetBool("LeftMove", false);
        
        //導航至玩家
        Agent.SetDestination(Player.position);
        //移動動畫
        Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
        Anim.SetFloat("Move", relDirection.z, 0.5f, Time.deltaTime);

        //敵人跟玩家之間的距離
        //Distance = Vector3.Distance(transform.position, CurTarget.position);

        if (Distance <= 4)
        {
            EnemyStatus = Enemy.Attack;
        }
    }
    
    //攻擊
    void Attack()
    {
        //敵人跟玩家之間的距離
        //Distance = Vector3.Distance(transform.position, CurTarget.position);

        #region 看著玩家
        Vector3 dir = Player.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);
        float angle = Vector3.Angle(transform.forward, dir);
        #endregion

        if (Distance > 4)
        {
            int ChargeProbability = Random.Range(1, 11);
            if (ChargeProbability < 10)
            {
                EnemyStatus = Enemy.Chase;
            }
            else
            {
                EnemyStatus = Enemy.Charge;
            }
        }
        //敵人面向玩家角度小於6 和攻擊過後
        #region 攻擊和繞著玩家跑
        if (angle < 6 && !AttackOnce)
        {
            int i = Random.Range(1, 5);
            Anim.SetTrigger("Attack" + i);
            StartCoroutine("CloseAttack");
            AttackOnce = true;
        }
        else
        {
            //看著玩家繞圈
            Anim.SetBool("LeftMove",true);
        }
        #endregion
    }

    //警戒
    void Alert()
    {
        if (Player == null)
        {
            EnemyStatus = Enemy.Patrol;
        }
        else
        {
            Anim.SetFloat("Move", 0.4f);

            Vector3 dir = Player.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);

            if (Distance < 6)
            {
                EnemyStatus = Enemy.Chase;
            }
        }

    }
    
    //巡邏
    void Patrol()
    {
        Anim.SetBool("LeftMove", false);
        if (points.Length > AttackRange)
            return;

        Agent.destination = points[DestPoint].position;
        DestPoint = (DestPoint + 1) % points.Length;

    }
    
    //突刺
    void Charge()
    {
        Anim.SetBool("LeftMove", false);

        //Distance = Vector3.Distance(transform.position, CurTarget.position);

        if (Distance < 5)
        {

            //Agent.angularSpeed = 0;
            //Anim.SetTrigger("Attack6");
            //if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack6"))
            //{
            //    Agent.angularSpeed = 360;
            //    EnemyStatus = Enemy.Attack;
            //}
        }
    }

    //受擊
    void Impact()
    {
        Anim.SetTrigger("Hit");
        Hp -= 10;
        Player = player.transform;
        EnemyStatus = Enemy.Chase;
    }
    //動畫事件
    public void AttackEventsStart()
    {
        DamageCollider.GetComponent<BoxCollider>().enabled = true;
    }
    public void AttackEventsEnd()
    {
        DamageCollider.GetComponent<BoxCollider>().enabled = false;
    }

    //重置動畫判定
    IEnumerator CloseAttack()
    {
        yield return new WaitForSeconds(Random.Range(3, 6));
        AttackOnce = false;
        Anim.SetBool("LeftMove", false);
    }

}
