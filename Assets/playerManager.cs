using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerManager : MonoBehaviour
{

    public int gold;

    public int playerHealth;

    public GameObject uiHolder;

    private TextMeshProUGUI uiGold;
    private TextMeshProUGUI uiHealth;


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
        uiGold = uiHolder.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        uiHealth = uiHolder.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        displayGold();
        displayHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            buyTomatoTower();
            displayGold();
        }
    }

    public void displayGold() {uiGold.text = "Gold: " + gold;}
    public void displayHealth() {uiHealth.text = "Health: " + playerHealth;}
    public void generateGold(int value) {gold += value; displayGold();}
    public void spendGold(int value) {gold -= value; displayGold();}
    public void loseHealth(int value) {playerHealth -= value; displayHealth();}
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
