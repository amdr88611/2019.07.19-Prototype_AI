using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public enum Status{Idle,Walk,Run,Attack}
    public Status GameStatus;


    Animator Anim;
    NavMeshAgent Nav;
    public Transform other;

    float Distance;
    float Sp;
    float Timer;
    int i;

    void Start()
    {
        Anim = gameObject.GetComponent<Animator>();
        Nav = gameObject.GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        Distance = Vector3.Distance(other.position, transform.position);

        switch (GameStatus)
        {
            case Status.Idle:

                if (Distance <= 10)                
                   GameStatus = Status.Walk;                
                    break;
            case Status.Walk:

                Nav.SetDestination(other.position);
                Anim.SetBool("isLookPlayer", true);
                EnemyLookat();

                if (Distance <= 8)
                    GameStatus = Status.Run;

                break;

            case Status.Run:
                Anim.SetBool("isChasingRange", true);

                if (Distance <= 4)
                    GameStatus = Status.Attack;



                break;
            case Status.Attack:
                Timer += Time.deltaTime;
                EnemyLookat();

                if (Timer > 1)
                {
                    if (Timer > 1)
                    {
                        i = Random.Range(1, 4);
                        Timer = 0;
                        Anim.SetTrigger("Attack" + i);
                    }
                }
                else
                    GameStatus = Status.Run;


                break;
        }

    }
    public void EnemyLookat()
    {
        Vector3 EnemyLookat = new Vector3(other.transform.position.x, transform.position.y, transform.position.z);
        transform.LookAt(EnemyLookat);
    }

}

