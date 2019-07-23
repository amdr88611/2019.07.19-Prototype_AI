using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI2 : MonoBehaviour
{
    NavMeshAgent Agent; //導航
    Animator Anim;

    public Transform CurTarget; //玩家
    public float AttackRange = 3; //攻擊距離
    public float AttackRate = 2; //攻擊頻率
    public bool CurrentlyAttacking; //目前攻擊

    float AttTimer; //計時器
    bool AttackOnce;
    bool StopRotating;

    public GameObject DamageCollider;
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        SetupAnimator();
        Agent.stoppingDistance = AttackRange;
    }

    void Update()
    {
        if (!CurrentlyAttacking)
        {
            MovementHandler();
        }
        AttackHandler();
    }
    void MovementHandler()
    {
        if (CurTarget != null)
        {
            Agent.SetDestination(CurTarget.position);

            Vector3 relDirection = transform.InverseTransformDirection(Agent.desiredVelocity);

            Anim.SetFloat("Move", relDirection.z, 0.5f, Time.deltaTime);

            float distance = Vector3.Distance(transform.position, CurTarget.position);

            if (distance <= AttackRange)
            {
                AttTimer += Time.deltaTime;
                if (AttTimer > AttackRate)
                {
                    CurrentlyAttacking = true;
                    AttTimer = 0;
                }
            }
        }
    }
    void AttackHandler()
    {
        if (CurrentlyAttacking)//目前攻擊
        {
            if (!StopRotating)
            {
                Vector3 dir = CurTarget.position - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10);
                
                float angle = Vector3.Angle(transform.forward, dir);

                //敵人面向玩家角度小於5
                if (angle < 5)
                {
                    if (!AttackOnce)
                    {
                        int i = Random.Range(1, 6);
                        Agent.isStopped = true;
                        Anim.SetTrigger("Attack"+i);
                        StartCoroutine("CloseAttack");
                        AttackOnce = true;
                    }
                }
            }
        }
    }
    IEnumerator CloseAttack()
    {
        yield return new WaitForSeconds(2f);
        //Anim.SetBool("Attack", false);
        AttackOnce = false;
        Agent.isStopped = false;
        CurrentlyAttacking = false;
        StopRotating = false;
        
    }
    void SetupAnimator()
    {
        //this is a ref to the animator component on the root.
        Anim = GetComponent<Animator>();
        //we use avatar from a child animator component if present.
        //this is to enable easy swpping of the character model as a child node.
        foreach (var ChildAnimator in GetComponentsInChildren<Animator>())
        {
            if (ChildAnimator != Anim)
            {
                Anim.avatar = ChildAnimator.avatar;
                Destroy(ChildAnimator);
                break; //if you find firse animator, stop searching

            }
        }
    }
    public void AttackEventsStart()
    {
        print("123");
        StopRotating = true;
        DamageCollider.GetComponent<BoxCollider>().enabled = true;
    }
    public void AttackEventsEnd()
    {
        StopRotating = true;
        DamageCollider.GetComponent<BoxCollider>().enabled = false;
    }
}
