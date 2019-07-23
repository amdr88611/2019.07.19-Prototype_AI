using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    EnemyAI2 AI;

    void Start()
    {
        AI = GetComponentInParent<EnemyAI2>();   
    }

 
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (AI.CurTarget == null)
        if(other.GetComponent<Player>())
        {
                AI.CurTarget = other.transform;
        }
    }
}
