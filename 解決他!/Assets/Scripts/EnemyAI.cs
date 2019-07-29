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
        SecondStage,
        Dead
    }
    public Enemy EnemyStatus;

    EnemyDetection EnemyDetection;
    public NavMeshAgent Agent; //導航
    Animator Anim;
    CapsuleCollider EnemyCollider;
    SphereCollider HearingZone;
    ParticleSystem SecondStageStand;

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
    public float AttackRange;   //攻擊距離
    [Range(4,10)]
    [Header("攻擊頻率")]
    public int AttackRate;    //攻擊頻率
    [Header("敵人攻擊判定")]
    public GameObject DamageCollider1;
    public GameObject DamageCollider2;
    [Header("巡邏地點")]
    public Transform[] points;
    private int DestPoint = 0; //巡邏順位

    [Header("是否進入第二階段")]
    public bool IsSecondStage;
    [Header("是否發現玩家")]
    public bool IsFindPlayer;
    bool AttackOnce;
    bool IsDead;
    bool IsCharge;

    float Distance;  //玩家與敵人之距離

    int AttackCombo;


    bool SecondStage;


    public GameObject b;

    void Start()
    {
        
        Anim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        EnemyDetection = GetComponent<EnemyDetection>();
        EnemyCollider = GetComponent<CapsuleCollider>();
        SecondStageStand = GetComponent<ParticleSystem>();

        HearingZone = GameObject.FindGameObjectWithTag("HearingZone").GetComponent<SphereCollider>();
        Agent.stoppingDistance = AttackRange;
        AttackCombo = 5;
        IsCharge = false;
        EnemyStatus = Enemy.Patrol;
    }


    void Update()
    {
       print(IsFindPlayer);
        if (Hp <= 50 && !SecondStage)
        {
            EnemyStatus = Enemy.SecondStage;
        }

        if (Hp <= 0)
        {
            Anim.SetBool("Dead", true);
            EnemyStatus = Enemy.Dead;
        }

        if (Input.GetKeyDown(KeyCode.Y)&& Hp >0 && !IsSecondStage)
        { 
            Hp -= 10;
            EnemyStatus = Enemy.Impact;
            Anim.SetTrigger("Hit");
        }
        if (Player != null)
        {
            //敵人和玩家之間的距離
            Distance = Vector3.Distance(transform.position, Player.position);
        }
        switch (EnemyStatus)
        {
            case Enemy.Patrol: // 巡邏
                if (!IsDead)
                {
                    Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
                    Anim.SetFloat("Move", relDirection.z, 0.5f, Time.deltaTime);

                    if (!Agent.pathPending && Agent.remainingDistance < AttackRange)
                        Patrol();
                }
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
            case Enemy.SecondStage: //進入第二階段
                SwitchSecondStage();
                break;
            case Enemy.Dead: // 死亡
                if (!IsDead)
                {
                    SecondStageStand.Stop();
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
        else if (IsFindPlayer)
        {
            //導航至玩家
            Agent.SetDestination(Player.position);
            //移動動畫
            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Anim.SetFloat("Move", relDirection.z, 0.8f, Time.deltaTime);

            if (Distance <= AttackRange)
            {
                EnemyStatus = Enemy.Attack;
            }
        }
        else if (!IsFindPlayer)
        {
            //導航至玩家最後位置
            Agent.SetDestination(EnemyDetection.PlayerLastPosition.position);
            //移動動畫
            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Anim.SetFloat("Move", relDirection.z, 0.8f, Time.deltaTime);
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
        else if (IsFindPlayer)

        {
            Agent.SetDestination(Player.position);
            //移動動畫
            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Anim.SetFloat("Move", relDirection.z, 0.8f, Time.deltaTime);


            if (Distance < ChargeRange/*4*/ && !IsCharge)
            {
                Anim.SetTrigger("Charge");
                IsCharge = true;
            }
        }
        else if (!IsFindPlayer)
        {
            Agent.SetDestination(EnemyDetection.PlayerLastPosition.position);

            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);
            Anim.SetFloat("Move", relDirection.z, 0.8f, Time.deltaTime);
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
            //float angle = Vector3.Angle(transform.forward, dir);
            #endregion

            if (Distance > ChargeRange/*4*/)
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
            #region 攻擊和繞著玩家跑
            //敵人面向玩家角度小於6 和攻擊過後
            if (/*angle < 6 &&*/ !AttackOnce)
            {
                int i = Random.Range(1, AttackCombo);
                Anim.SetTrigger("Attack" + i);
                StartCoroutine(CloseAttack());
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
        else if(IsFindPlayer)
        {
            Anim.SetFloat("Move", 0.4f);
            Agent.SetDestination(Player.position);

            if (Distance < ChaseRange/*6*/)
            {
                EnemyStatus = Enemy.Chase;
            }
            else if (Distance >= AlertRange/*11*/)
            {
                Player = null;
            }
        }
        else if (!IsFindPlayer)
        {
            Agent.SetDestination(EnemyDetection.PlayerLastPosition.position);
            AlertToPatrol();
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
        HearingZone.radius = 10f;
    }

    void SwitchSecondStage()
    {
        if (!SecondStage)
        {
            IsSecondStage = true;
            EnemyStatus = Enemy.SecondStage;
            Agent.SetDestination(transform.position);
            print("進入第二階段");
            Anim.SetTrigger("SecondStage");
            SecondStage = true;
        }
    }

    //動畫事件 攻擊判定
    public void AttackEventsStart()
    {
        DamageCollider1.GetComponent<BoxCollider>().enabled = true;
        if (SecondStage)
            DamageCollider2.GetComponent<BoxCollider>().enabled = true;
    }
    public void AttackEventsEnd()
    {
        DamageCollider1.GetComponent<BoxCollider>().enabled = false;
        if (SecondStage)
            DamageCollider2.GetComponent<BoxCollider>().enabled = false;
    }
    //突擊動畫事件
    public void ChargeEnd()
    {
        EnemyStatus = Enemy.Attack;
    }
    //第二階段動畫事件
    public void Stand()
    {
        EnemyCollider.enabled = false;
        SecondStageStand.Play();
    }
    public void SecondStageStart()
    {
        b.SetActive(true);

    }
    public void SecondStageEnd()
    {
        EnemyCollider.enabled = true;
        AttackCombo = 7;
        AttackRate = 5;
        EnemyStatus = Enemy.Attack;
        HearingZone.radius = 10f;
        IsSecondStage = false;
    }
    public void LookAtPlayer()
    {
        transform.LookAt(Player.transform.position);
    }

    //重置判定
    IEnumerator CloseAttack()
    {
        IsCharge = false;
        yield return new WaitForSeconds(Random.Range(3, AttackRate));
        Anim.SetBool("LeftMove", false);
        AttackOnce = false;
    }
    IEnumerator AlertToPatrol()
    {
        yield return new WaitForSeconds(3);
        Player = null;
        EnemyStatus = Enemy.Patrol;
    }


}
