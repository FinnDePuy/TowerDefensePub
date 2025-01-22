using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Common;
using Unity.VisualScripting;
using UnityEngine;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;

public class EnemyManager : MonoBehaviour
{

    public class Roundinfo
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Yellow { get; set; }
        public int Orange { get; set; }
        public int Green { get; set; }
    }

    public bool hit;
    public int currentDamage;

    public GameObject currentEnemy;

    [SerializeField] private GameObject[] spawnableEnemies;
    [SerializeField] private TextAsset roundJson;

    private Dictionary<string, Roundinfo> rounds;

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
        LoadRoundData(roundJson.text);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadRoundData(string json)
    {
        try
        {
            // Deserialize JSON into a dictionary
            //var roundsData = JsonSerializer.ToJsonString(json);
            Debug.Log("Rounds successfully loaded.");
            Debug.Log(json);
            var cleanedJson = json.Replace("\r", "").Replace("\n", "");
            char[] unwanted = { ' ' }; // Example: you can add more unwanted characters to this array
            var cleanedJsonString = string.Join("", cleanedJson.Where(c => !unwanted.Contains(c)));
            Debug.Log(cleanedJsonString);
            //return roundsData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading JSON: {ex.Message}");
            //return new Dictionary<string, Roundinfo>();
        }
    }


    private void spawnEnemies()
    {
        return;
    }

    private void startRound()
    {
        return;
    }
    // public void HurtEnemy(int Damage)
    // {
    //     currentDamage = Damage;
    //     hit = true;
    // }
}
