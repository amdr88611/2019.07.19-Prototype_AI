using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    EnemyAI AI;

    void Start()
    {
        AI = GetComponentInParent<EnemyAI>();   
    }

 
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (AI.CurTarget == null)
        if(other.CompareTag("Player"))
        {
                AI.CurTarget = other.transform;
                AI.EnemyStatus = EnemyAI.Enemy.Run;

            }
    }
}
