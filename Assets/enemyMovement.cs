using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class enemyMovement : MonoBehaviour
{

    public Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWaypointsIndex = 0;

    public Material[] colors;

    private MeshRenderer mr;

    private int currentHealth;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointsIndex].position);
        }

        mr = GetComponent<MeshRenderer>();

        currentHealth = Array.IndexOf(colors, mr.sharedMaterial);
        Debug.Log(currentHealth);        

        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaypointsIndex < waypoints.Length && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(waypoints[currentWaypointsIndex].position);
            currentWaypointsIndex += 1;
        }

        if(EnemyManager.Instance.hit && EnemyManager.Instance.currentEnemy == this.gameObject)
        {
            DamageEnemy(EnemyManager.Instance.currentDamage);
        }
    }

    public void DamageEnemy(int Damage)
    {
        currentHealth -= Damage;
        Debug.Log(currentHealth);

        if(currentHealth <= 0)
        {
            EnemyManager.Instance.currentEnemy = null;
            Destroy(this.gameObject);
        }

        mr.material = colors[currentHealth];


        Debug.Log(colors[currentHealth]);
        

        //EnemyManager.Instance.currentEnemy.GetComponent<MeshRenderer>().material = colors[currentHealth];

        //Debug.Log(EnemyManager.Instance.currentEnemy.GetComponent<MeshRenderer>().material);
        
    }
}