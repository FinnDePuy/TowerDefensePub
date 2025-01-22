using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class TomatoTurret : MonoBehaviour
{
    public GameObject[] enemies;

    private Dictionary<string, bool> targetMode = new Dictionary<string, bool>();
    public float Cooldown;
    public bool CooldownActive;

    LayerMask layerMask;
    RaycastHit hit;
    Collider current;
    LineRenderer lr;


    public float AttackTimer = 1.0f;

    public int currentHealth;

    private bool inside;
    // Start is called before the first frame update
    void Start()
    {
        CooldownActive = false;
        Cooldown = Time.deltaTime;
        layerMask = LayerMask.GetMask("Enemy");
        lr = GetComponent<LineRenderer>();


        //setting up the line to be drawn
        lr.positionCount = 2;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(1, transform.position);
        lr.SetPosition(0, transform.position);
        lr.startColor = Color.clear;
        lr.endColor = Color.clear;


        //setting up the target modes
        targetMode.Add("First", false);
        targetMode.Add("Last", true);
        targetMode.Add("Strong", false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Cooldown);
        if(Cooldown < AttackTimer && CooldownActive)
        {
            Cooldown += Time.deltaTime;
            EnemyManager.Instance.hit = false;
        }
        if(CooldownActive && Cooldown >= AttackTimer)
        {
            CooldownActive = false;
        }


        if(inside)
        {
            //stop attacking edgecase
            if(current == null) 
            {
                lr.SetPosition(1, transform.position);
                lr.startColor = Color.clear;
                lr.endColor = Color.clear;
                return;
            }

            //Debug.DrawLine(transform.position, current.transform.position, Color.yellow);

            //if not visably attacking attack
            if(lr.startColor != Color.red)
            {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.SetPosition(0, transform.position);
            }

            //keep tracking
            lr.SetPosition(1, current.transform.position);


            //this is where we will do damage any of the enemies with this specific tower
            if(!CooldownActive)
            {
                CooldownActive = true;
                Cooldown = 0.0f;
                currentHealth = current.gameObject.GetComponent<enemyMovement>().DamageEnemy(1);
                if(currentHealth <= 0)
                {
                    current = null;
                }
                EnemyManager.Instance.hit = true;
                Debug.Log("Hit");
            }
        }
        else
        {
            lr.SetPosition(1, transform.position);
            lr.startColor = Color.clear;
            lr.endColor = Color.clear;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            //EnemyManager.Instance.currentEnemy = other.gameObject;
            //Debug.Log("Player gettings attacked is " + other.gameObject);
            if(current == null || targetMode["Last"])
            {
                current = other;
            }
            inside = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        //EnemyManager.Instance.currentEnemy = other.gameObject;
        //Debug.Log("Player gettings attacked is " + other.gameObject);
        if(current == null && !targetMode["First"])
            {
                current = other;
            }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            //EnemyManager.Instance.currentEnemy = null;
            inside = false;
            current = null;
        }
    }



    private void switchTargetMode(string target)
    {
        foreach(var mode in targetMode.Keys)
        {
            if(target == mode)
            {
                targetMode[mode] = true;
            }else
            {
                targetMode[mode] = false;
            }
        }
    }
}
