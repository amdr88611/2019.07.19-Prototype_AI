using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    ParticleSystem Par;
    private void Start()
    {
        Par = GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Par.Play();
        if(other.CompareTag("Player"))
        print("打到玩家了");
    }
}
