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

    EnemyDetection EnemyDetection;
    NavMeshAgent Agent; //導航
    Animator Anim;
    CapsuleCollider EnemyCollider;
    SphereCollider HearingZone;

    [Header("玩家")]
    public Transform Player;     //玩家
    public int Hp = 100;
    [Header("警戒距離")]
    public float AlertRange;
    [Header("追擊距離")]
    public float ChaseRange;
    [Header("跳斬距離")]
    public float ChargeRange;
    [Header("攻擊距離")]
    public float AttackRange = 3;   //攻擊距離
    [Range(4,10)]
    [Header("攻擊頻率")]
    public int AttackRate;    //攻擊頻率
    [Header("敵人攻擊判定")]
    public GameObject DamageCollider;
    [Header("巡邏地點")]
    public Transform[] points;
    private int DestPoint = 0; //巡邏順位


    float Distance;  //玩家與敵人之距離
    bool AttackOnce;
    bool IsDead;
    bool IsCharge;

    public GameObject player;

    public GameObject b,b2;

    void Start()
    {
        EnemyDetection = GetComponent<EnemyDetection>();
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        HearingZone = GameObject.FindGameObjectWithTag("HearingZone").GetComponent<SphereCollider>();
        Agent.stoppingDistance = AttackRange;
        EnemyCollider = GetComponent<CapsuleCollider>();
        IsCharge = false;
        //Patrol();
    }


    void Update()
    {
        if (Hp <= 50)
        {
            b.SetActive(true);
            b2.SetActive(true);
        }

        if (Hp <= 0)
        {
            EnemyStatus = Enemy.Dead;
        }

        if (Input.GetKeyDown(KeyCode.H)&& Hp >0)
        { 
            Hp -= 10;
            EnemyStatus = Enemy.Impact;
            Anim.SetTrigger("Hit");
        }
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
                    Anim.SetBool("Dead",true);
                    Agent.SetDestination(transform.position );
                    IsDead = true;
                    EnemyCollider.enabled = false;
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

        if (Player == null)
        {
            EnemyStatus = Enemy.Patrol;
        }
        else
        {
            //導航至玩家
            Agent.SetDestination(EnemyDetection.PlayerLastPosition.position);
            //移動動畫
            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Anim.SetFloat("Move", relDirection.z,0.8f, Time.deltaTime);


            if (Distance <= 3)
            {
                EnemyStatus = Enemy.Attack;
            }
        }
    }
    
    //突刺
    void Charge()
    {
        Anim.SetBool("LeftMove", false);
        if (Player == null)
        {
            EnemyStatus = Enemy.Chase;
        }
        else
        {
            //移動動畫
            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Agent.SetDestination(EnemyDetection.PlayerLastPosition.position);
            Anim.SetFloat("Move", relDirection.z, 0.8f, Time.deltaTime);


            if (Distance < 4 && !IsCharge)
            {
                Anim.SetTrigger("Attack6");
                IsCharge = true;
            }
        }

    }
    
    //攻擊
    void Attack()
    {
        
        if (Player == null)
        {
            EnemyStatus = Enemy.Patrol;
        }
        else
        {
            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Anim.SetFloat("Move", relDirection.z, 0f, Time.deltaTime);

            #region 看著玩家
            Vector3 dir = Player.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);
            float angle = Vector3.Angle(transform.forward, dir);
            #endregion

            if (Distance > 4)
            {
                int ChargeProbability = Random.Range(1, 11);
                if (ChargeProbability < 8)
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
            if (/*angle < 6 &&*/ !AttackOnce)
            {
                int i = Random.Range(1, 5);
                Anim.SetTrigger("Attack" + i);
                StartCoroutine("CloseAttack");
                AttackOnce = true;
            }
            else
            {
                Anim.SetBool("LeftMove", true);
                //看著玩家繞圈
            }
            #endregion

        }
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
            Agent.SetDestination(EnemyDetection.PlayerLastPosition.position);

            if (Distance < 6 )
            {
                EnemyStatus = Enemy.Chase;
            }
            else if (Distance >= 11)
            {
                Player = null;
            }
        }

    }
    
    //巡邏
    void Patrol()
    {
        if (!IsDead)
        {
            Anim.SetBool("LeftMove", false);
            HearingZone.radius = 3f;
            if (points.Length > AttackRange)
                return;

            Agent.destination = points[DestPoint].position;
            DestPoint = (DestPoint + 1) % points.Length;
        }
    }

    //受擊
    void Impact()
    {
        EnemyStatus = Enemy.Chase;
    }

    //動畫事件 攻擊判定
    public void AttackEventsStart()
    {
        DamageCollider.GetComponent<BoxCollider>().enabled = true;
    }
    public void AttackEventsEnd()
    {
        DamageCollider.GetComponent<BoxCollider>().enabled = false;
    }

    //突擊動畫事件
    public void ChargeEnd()
    {
        EnemyStatus = Enemy.Attack;
    }

    //重置判定
    IEnumerator CloseAttack()
    {
        IsCharge = false;
        yield return new WaitForSeconds(Random.Range(3, AttackRate));
        Anim.SetBool("LeftMove", false);
        AttackOnce = false;
    }


}
