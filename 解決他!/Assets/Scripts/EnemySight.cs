using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    EnemyAI AI;
    SphereCollider Col;
    EnemyDetection Ed;

    void Start()
    {
        AI = GetComponentInParent<EnemyAI>();
        Col = GetComponent<SphereCollider>();
        Ed = GetComponentInParent<EnemyDetection>();
    }

    private void OnTriggerEnter(Collider other)
    {
        AI.EnemyStatus = EnemyAI.Enemy.Alert;
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Col.radius = 3; //Trigger範圍變大
            AI.Player = other.transform;
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
