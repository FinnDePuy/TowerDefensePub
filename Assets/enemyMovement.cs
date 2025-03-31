using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class enemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool inside = false;
    private int currentWaypointsIndex = 0;
    public Material[] colors;
    private MeshRenderer mr;
    public int currentHealth;
    public int startingHealth;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(EnemyManager.Instance.waypoints.Length > 0)
        {
            agent.SetDestination(EnemyManager.Instance.waypoints[currentWaypointsIndex].position);
        }
        mr = GetComponent<MeshRenderer>();
        currentHealth = Array.IndexOf(colors, mr.sharedMaterial);
        startingHealth = currentHealth;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaypointsIndex < EnemyManager.Instance.waypoints.Length && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(EnemyManager.Instance.waypoints[currentWaypointsIndex].position);
            currentWaypointsIndex += 1;
        }
        if(currentWaypointsIndex == EnemyManager.Instance.waypoints.Length)
        {
            playerManager.Instance.loseHealth(currentHealth);
            Destroy(gameObject);
        }
        if(EnemyManager.Instance.hit)
        {
            DamageEnemy(EnemyManager.Instance.currentDamage);
        }
        animateSlime();
    }

    public int DamageEnemy(int Damage)
    {
        currentHealth -= Damage;
        transform.localScale -= new Vector3(0.075f * Damage, 0.075f * Damage, 0.075f * Damage);
        if(currentHealth <= 0)
        {
            playerManager.Instance.generateGold(startingHealth);
            //EnemyManager.Instance.currentEnemy = null;
            Destroy(gameObject);
        }
        mr.material = colors[currentHealth];
        return currentHealth;  
    }
    private void animateSlime()
    {
        if(agent.destination != null)
        {
            animator.SetBool("Movement", true);
            Debug.Log("test");
        }
        else
        {
            animator.SetBool("Movement", false);
            Debug.Log("False");
        }
    }
}