using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Common;
using Unity.VisualScripting;
using UnityEngine;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;

public class EnemyManager : MonoBehaviour
{

    public Transform[] waypoints;


    private int currentRound;
    public bool hit;
    public int currentDamage;
    public GameObject currentEnemy;
    [SerializeField] private GameObject spawnpoint;
    [SerializeField] private GameObject[] spawnableEnemies;
    public static EnemyManager Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if there is already an instance of this class
        if (Instance != null && Instance != this)
        {
            // Destroy the duplicate instance
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        hit = false;
        currentRound = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(currentRound);
            startRound(currentRound);
            currentRound++;
        }
    }
    private void startRound(int round)
    {
        StartCoroutine(roundData(round));
        return;
    }


    //StartCoroutine(WaitForTime(5f)); // Wait for 5 seconds
    IEnumerator WaitForTimeandSpawn(float seconds, int type, int count)
    {
        for(int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(seconds);
            Instantiate(spawnableEnemies[type], spawnpoint.transform.position, spawnpoint.transform.rotation);
        }
    }




    public IEnumerator roundData(int Round)
    {
        switch (Round){
            case 0:
            {
                yield break;
            }
            case 1:
            {
                yield return StartCoroutine(WaitForTimeandSpawn(0.5f, 0, 5));
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(WaitForTimeandSpawn(1f, 1, 2));
                break;
            }
            case 2:
            {
                break;
            }
            case 3:
            {
                break;
            }
            case 4:
            {
                break;
            }
            case 5:
            {
                break;
            }
            case 6:
            {
                break;
            }
            case 7:
            {
                break;
            }
            case 8:
            {
                break;
            }
            case 9:
            {
                break;
            }
            case 10:
            {
                break;
            }
        }
    }
}
