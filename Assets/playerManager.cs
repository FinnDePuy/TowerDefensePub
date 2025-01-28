using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerManager : MonoBehaviour
{

    public int gold;

    public int playerHealth;


    [SerializeField] public GameObject turrets;

    public GameObject tomatoTurrets;

    [SerializeField] private GameObject tomatoTurret;



    public static playerManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        tomatoTurrets = turrets.transform.GetChild(0).gameObject;
        gold = 30;
        playerHealth = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            buyTomatoTower();
        }
    }
    public void generateGold(int value) {gold += value;}
    public void spendGold(int value) {gold -= value;}
    public void buyTomatoTower()
    {
        if(gold >= 15)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Instantiate(tomatoTurret, worldPosition + new Vector3 (0f, 3.5f, 0f), Quaternion.identity, tomatoTurrets.transform);
            tomatoTurret.GetComponent<TomatoTurret>().purchaseThis();
            gold -= 15;
        }
    }
}
