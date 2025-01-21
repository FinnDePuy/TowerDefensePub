using System.Collections;
using System.Collections.Generic;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class TomatoTurret : MonoBehaviour
{
    public float Cooldown;
    public bool CooldownActive;


    public float AttackTimer = 1.0f;

    private bool inside;
    // Start is called before the first frame update
    void Start()
    {
        CooldownActive = false;
        Cooldown = Time.deltaTime;
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
            //this is where we will attack any of the enemies with this specific tower
            if(!CooldownActive)
            {
                CooldownActive = true;
                Cooldown = 0.0f;
                EnemyManager.Instance.HurtEnemy(1);
                Debug.Log("Hit");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if(EnemyManager.Instance.currentEnemy == null)
            {
                EnemyManager.Instance.currentEnemy = other.gameObject;
                Debug.Log("Player gettings attacked is " + other.gameObject);
            }

            inside = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        if(EnemyManager.Instance.currentEnemy == null)
            {
                EnemyManager.Instance.currentEnemy = other.gameObject;
                Debug.Log("Player gettings attacked is " + other.gameObject);
            }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            EnemyManager.Instance.currentEnemy = null;
            inside = false;
        }
    }
}
