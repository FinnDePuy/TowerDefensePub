using System.Collections;
using System.Collections.Generic;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class TomatoTurret : MonoBehaviour
{
    public float Cooldown;
    public bool CooldownActive;

    LayerMask layerMask;
    RaycastHit hit;
    Collider current;
    LineRenderer lr;

    private Vector3 direction;

    public float AttackTimer = 1.0f;

    private bool inside;
    // Start is called before the first frame update
    void Start()
    {
        CooldownActive = false;
        Cooldown = Time.deltaTime;
        layerMask = LayerMask.GetMask("Enemy");
        lr = GetComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(1, transform.position);
        lr.SetPosition(0, transform.position);
        lr.startColor = Color.clear;
        lr.endColor = Color.clear;

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
            if(current == null) 
            {
                lr.SetPosition(1, transform.position);
                lr.startColor = Color.clear;
                lr.endColor = Color.clear;
                return;
            }
            //Debug.Log("This is = " + transform.position + " Enemy is = " + current.transform.position);

            direction = current.transform.position - transform.position;

            Debug.DrawLine(transform.position, current.transform.position, Color.yellow);
            if(lr.startColor != Color.red)
            {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.SetPosition(0, transform.position);
            }
            lr.SetPosition(1, current.transform.position);
            //this is where we will attack any of the enemies with this specific tower
            if(!CooldownActive)
            {
                CooldownActive = true;
                Cooldown = 0.0f;
                current.gameObject.GetComponent<enemyMovement>().DamageEnemy(1);
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
            Debug.Log("Player gettings attacked is " + other.gameObject);
            current = other;
            inside = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        if(EnemyManager.Instance.currentEnemy == null)
            {
                //EnemyManager.Instance.currentEnemy = other.gameObject;
                Debug.Log("Player gettings attacked is " + other.gameObject);
                current = other;
            }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            //EnemyManager.Instance.currentEnemy = null;
            inside = false;
        }
    }
}
