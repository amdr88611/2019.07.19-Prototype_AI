using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    EnemyAI AI;
    SphereCollider Col;

    void Start()
    {
        AI = GetComponentInParent<EnemyAI>();
        Col = GetComponent<SphereCollider>();
    }

 
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Player"))
        {
            Col.radius = 10; //Trigger範圍變大
            AI.Player = other.transform;
            AI.EnemyStatus = EnemyAI.Enemy.Alert;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Col.radius = 3;//Trigger範圍回到原本
            AI.Player = null;
            AI.EnemyStatus = EnemyAI.Enemy.Alert;
        }
    }
}
