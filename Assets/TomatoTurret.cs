using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class TomatoTurret : MonoBehaviour
{
    public GameObject[] enemies;

    public GameObject enemyFloor;

    private Dictionary<string, bool> targetMode = new Dictionary<string, bool>();
    public float Cooldown;
    public bool CooldownActive;
    LayerMask layerMask;
    RaycastHit hit;
    Collider current;
    LineRenderer lr;
    public bool purchased = false;
    Vector3 mousePosition;

    BoxCollider boxCollider;
    private bool isInNoPlacementArea = false;
    public LayerMask PlacementLayer;

    bool targetLock = false;
    public float AttackTimer = 1.0f;
    public int currentHealth;
    private bool inside;
    private bool airborn;
    // Start is called before the first frame update
    void Start()
    {

        airborn = false;
        CooldownActive = false;
        Cooldown = Time.deltaTime;
        layerMask = LayerMask.GetMask("Enemy");
        lr = GetComponent<LineRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider>();

        //setting up the line to be drawn
        lr.positionCount = 2;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(1, transform.position);
        lr.SetPosition(0, transform.position);
        lr.startColor = Color.clear;
        lr.endColor = Color.clear;


        //setting up the target modes
        targetMode.Add("First", true);
        targetMode.Add("Last", false);
        targetMode.Add("Strong", false);


        enemyFloor = GameObject.Find("Enemy Floor");
    }

    // Update is called once per frame
    void Update()
    {
        if(inside)
        {
            if(Cooldown < AttackTimer && CooldownActive)
            {
                Cooldown += Time.deltaTime;
                EnemyManager.Instance.hit = false;
            }
            if(CooldownActive && Cooldown >= AttackTimer)
            {
                CooldownActive = false;
                targetLock = false;
            }
            //stop attacking edgecase
            if(current == null) 
            {
                lr.SetPosition(1, transform.position);
                lr.startColor = Color.clear;
                lr.endColor = Color.clear;
                return;
            }

            //if not visably attacking attack
            if(lr.startColor != Color.red && !airborn)
            {
                Cooldown = 0.0f;
                CooldownActive = true;
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.SetPosition(0, transform.position);
                targetLock = true;
            }

            if(!airborn) lr.SetPosition(1, current.transform.position);


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
                Debug.Log("Hit from -> " + name);
                targetLock = false;
            }
        }
        else
        {
            lr.SetPosition(1, transform.position);
            lr.startColor = Color.clear;
            lr.endColor = Color.clear;
            Cooldown = 0.0f;
            CooldownActive = true;
            targetLock = false;
        }

        Vector3 bottomOfCube = transform.position - new Vector3(0, GetComponent<Collider>().bounds.extents.y - 4.5f, 0);
        // Create a ray from the bottom of the cube, pointing downward or in the direction you want to check
        Ray ray = new Ray(bottomOfCube, Vector3.down); // Ray goes downward from the bottom of the cube
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);  // Visualize the ray in the Scene view
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~PlacementLayer)) 
        {
            isInNoPlacementArea = true;
            Debug.Log("In Area");
        }
        else
        {
            isInNoPlacementArea = false;
            Debug.Log("Out Area");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(targetLock) return;
        other.gameObject.GetComponent<enemyMovement>().inside = true;
        if(other.gameObject.tag == "Enemy")
        {
            if(current == null || targetMode["Last"]) {current = other;}
            inside = true;
            if(targetMode["First"]) {current = EnemyManager.Instance.enemiesParent.transform.GetChild(0).GetComponent<Collider>();}
            if(targetMode["Strong"])
            {
                current = EnemyManager.Instance.enemiesParent.transform.GetChild(0).GetComponent<Collider>();
                for(int i = 0; i < EnemyManager.Instance.enemiesParent.transform.childCount; i++)
                {
                    if((current.gameObject.GetComponent<enemyMovement>().currentHealth < EnemyManager.Instance.enemiesParent.transform.GetChild(i).GetComponent<enemyMovement>().currentHealth) && EnemyManager.Instance.enemiesParent.transform.GetChild(i).GetComponent<enemyMovement>().inside == true)
                    {
                    current = EnemyManager.Instance.enemiesParent.transform.GetChild(i).GetComponent<Collider>();
                    }
                }
            }
            if(targetMode["Last"] && EnemyManager.Instance.enemiesParent.transform.GetChild(EnemyManager.Instance.enemiesParent.transform.childCount-1).GetComponent<enemyMovement>().inside == true)
            {
                current = EnemyManager.Instance.enemiesParent.transform.GetChild(EnemyManager.Instance.enemiesParent.transform.childCount-1).GetComponent<Collider>();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(targetLock) return;
        if(current == null && !targetMode["First"]) {current = other;}
        if(targetMode["First"] && current == null) {current = EnemyManager.Instance.enemiesParent.transform.GetChild(0).GetComponent<Collider>();}
        if(targetMode["Strong"] && current == null)
        {
            current = EnemyManager.Instance.enemiesParent.transform.GetChild(0).GetComponent<Collider>();
            for(int i = 0; i < EnemyManager.Instance.enemiesParent.transform.childCount; i++)
            {
                if((current.gameObject.GetComponent<enemyMovement>().currentHealth < EnemyManager.Instance.enemiesParent.transform.GetChild(i).GetComponent<enemyMovement>().currentHealth) && EnemyManager.Instance.enemiesParent.transform.GetChild(i).GetComponent<enemyMovement>().inside == true)
                {
                    current = EnemyManager.Instance.enemiesParent.transform.GetChild(i).GetComponent<Collider>();
                    Debug.Log("New enemy Selected");
                }
            }
        }
        if(targetMode["Last"] && EnemyManager.Instance.enemiesParent.transform.GetChild(EnemyManager.Instance.enemiesParent.transform.childCount-1).GetComponent<enemyMovement>().inside == true)
        {
            current = EnemyManager.Instance.enemiesParent.transform.GetChild(EnemyManager.Instance.enemiesParent.transform.childCount-1).GetComponent<Collider>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(targetLock) return;
        if(other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<enemyMovement>().inside = false;
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

    private Vector3 getMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }



    private void OnMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (boxCollider.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            mousePosition = Input.mousePosition - getMousePos();
        }
    }

    private void OnMouseDrag()
    {
        if (!purchased || playerManager.Instance.gold < 5) return;

        // Perform the raycast to check if we're interacting with the BoxCollider
        airborn = true;
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        lr.SetPosition(1, transform.position);
        lr.SetPosition(0, transform.position);
        Cooldown = 0.0f;
        CooldownActive = true;
        targetLock = false;
    }

    void OnMouseUp()
    {
        if(!airborn) return;
        airborn = false;
        if(playerManager.Instance.gold < 5) return;
        playerManager.Instance.spendGold(5);
    }
    public void purchaseThis()
    {
        purchased = true;
    }

}
