using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Unity.VisualStudio.Editor;
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
    UnityEngine.Vector3 mousePosition;

    private GameObject upgradeMenuParent;
    public GameObject upgradeMenu;

    BoxCollider boxCollider;
    private bool isInNoPlacementArea = false;
    public LayerMask PlacementLayer;

    bool targetLock = false;
    public float AttackTimer = 1.0f;
    public int currentHealth;
    private bool inside;
    private bool airborn;
    private bool locked;

    private GameObject body;
    [SerializeField] private GameObject laserStart;

    // Start is called before the first frame update
    void Start()
    {

        airborn = false;
        CooldownActive = false;
        locked = false;
        Cooldown = Time.deltaTime;
        layerMask = LayerMask.GetMask("Enemy");
        lr = GetComponentInChildren<LineRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        body = lr.gameObject;


        //setting up the line to be drawn
        lr.positionCount = 2;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(1, laserStart.transform.position);
        lr.SetPosition(0, laserStart.transform.position);
        lr.startColor = Color.clear;
        lr.endColor = Color.clear;


        //setting up the target modes
        targetMode.Add("First", true);
        targetMode.Add("Last", false);
        targetMode.Add("Strong", false);

        upgradeMenuParent = GameObject.Find("Upgrade Menus");
        upgradeMenu = upgradeMenuParent.transform.GetChild(0).gameObject;
        upgradeMenu.SetActive(false);
        enemyFloor = GameObject.Find("Enemy Floor");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L)) locked = !locked;
        if(Input.GetKeyDown(KeyCode.Escape) && upgradeMenu.activeSelf) upgradeMenu.SetActive(false);
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
                lr.SetPosition(1, laserStart.transform.position);
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
                lr.SetPosition(0, laserStart.transform.position);
                targetLock = true;
            }

            //actually aim the laser
            if(!airborn) 
            {
                lr.SetPosition(1, current.transform.position);
                lr.SetPosition(0, laserStart.transform.position);
                body.transform.LookAt(current.transform.position);
                body.transform.Rotate(new UnityEngine.Vector3(0, 180f, 0));
            }


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
            lr.SetPosition(0, laserStart.transform.position);
            lr.SetPosition(1, laserStart.transform.position);
            lr.startColor = Color.clear;
            lr.endColor = Color.clear;
            Cooldown = 0.0f;
            CooldownActive = true;
            targetLock = false;
        }

        UnityEngine.Vector3 bottomOfCube = transform.position - new UnityEngine.Vector3(0, GetComponent<Collider>().bounds.extents.y - 4.5f, 0);
        // Create a ray from the bottom of the cube, pointing downward or in the direction you want to check
        Ray ray = new Ray(bottomOfCube, UnityEngine.Vector3.down); 
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~PlacementLayer)) 
        {
            isInNoPlacementArea = true;
            Debug.Log("In Area");

            MoveToDefaultValidPosition();
        }
        else
        {
            isInNoPlacementArea = false;
            Debug.Log("Out Area");
        }
    }

    private void MoveToDefaultValidPosition()
    {
        float searchRadius = 20f;
        // Perform a search for valid positions in the surrounding area
        Collider[] validSpots = Physics.OverlapSphere(transform.position, searchRadius, PlacementLayer);

        if (validSpots.Length > 0)
        {
            Collider nearestSpot = validSpots[0];
            if(airborn) return;
            transform.position = nearestSpot.transform.position;
            lr.SetPosition(1, laserStart.transform.position);
            lr.SetPosition(0, laserStart.transform.position);
            Cooldown = 0.0f;
            CooldownActive = true;
            targetLock = false;
            playerManager.Instance.generateGold(5);
            Debug.Log("Moved to the default position.");
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

    private UnityEngine.Vector3 getMousePos()
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
        if (!purchased || playerManager.Instance.gold < 5 || locked) return;

        // Perform the raycast to check if we're interacting with the BoxCollider
        airborn = true;
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        lr.SetPosition(1, new UnityEngine.Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z));
        lr.SetPosition(0, new UnityEngine.Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z));
        Cooldown = 0.0f;
        CooldownActive = true;
        targetLock = false;
    }

    void OnMouseDown()
    {
        if(!locked) return;
        upgradeMenu.SetActive(true);
        Debug.Log("Test");
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
