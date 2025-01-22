using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class enemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private int currentWaypointsIndex = 0;

    public Material[] colors;

    private MeshRenderer mr;

    public int currentHealth;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(EnemyManager.Instance.waypoints.Length > 0)
        {
            agent.SetDestination(EnemyManager.Instance.waypoints[currentWaypointsIndex].position);
        }

        mr = GetComponent<MeshRenderer>();

        currentHealth = Array.IndexOf(colors, mr.sharedMaterial);
        //Debug.Log(currentHealth);        

        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaypointsIndex < EnemyManager.Instance.waypoints.Length && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(EnemyManager.Instance.waypoints[currentWaypointsIndex].position);
            currentWaypointsIndex += 1;
        }

        if(EnemyManager.Instance.hit)
        {
            DamageEnemy(EnemyManager.Instance.currentDamage);
        }
    }

    public int DamageEnemy(int Damage)
    {
        currentHealth -= Damage;

        transform.localScale -= new Vector3(0.075f * Damage, 0.075f * Damage, 0.075f * Damage);
        //Debug.Log(currentHealth);

        if(currentHealth <= 0)
        {
            //EnemyManager.Instance.currentEnemy = null;
            Destroy(gameObject);
        }

        mr.material = colors[currentHealth];

        return currentHealth;


        //Debug.Log(colors[currentHealth]);
        

        //EnemyManager.Instance.currentEnemy.GetComponent<MeshRenderer>().material = colors[currentHealth];

        //Debug.Log(EnemyManager.Instance.currentEnemy.GetComponent<MeshRenderer>().material);
        
    }
}