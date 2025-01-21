using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public bool hit;
    public int currentDamage;

    public GameObject currentEnemy;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HurtEnemy(int Damage)
    {
        currentDamage = Damage;
        hit = true;
    }
}
