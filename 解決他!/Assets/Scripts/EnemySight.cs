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
        
        if(other.CompareTag("Player"))
        {
            AI.Player = other.transform;
            AI.EnemyStatus = EnemyAI.Enemy.Alert;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        AI.Player = null;
        AI.EnemyStatus = EnemyAI.Enemy.Alert;

    }
}
