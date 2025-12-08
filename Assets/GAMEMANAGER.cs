using System.Collections;
using UnityEngine;
using TMPro;

public class EnduranceManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text chickenCountText;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text gardenHealthText;

    [Header("Chicken Settings")]
    [SerializeField] private GameObject chickenPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxChickens = 20;

    [Header("Damage Distances")]
    [SerializeField] private float flowerReachDistance = 1.5f;
    [SerializeField] private float playerDamageDistance = 1f;

    [Header("Zones")]
    [SerializeField] private Collider gardenZone;

    [Header("Health Settings")]
    [SerializeField] private int playerMaxHealth = 3;
    [SerializeField] private int gardenMaxHealth = 5;

    [Header("Player Damage")]
    [SerializeField] private int playerDamage = 1;
    [SerializeField] private float playerDamageInterval = 1.5f;  // <-- NEW TIMER CONTROL

    [Header("Garden Damage Over Time")]
    [SerializeField] private int gardenDamageAmount = 2;
    [SerializeField] private float gardenDamageInterval = 10f;

    private float survivalTime = 0f;
    private int playerHealth;
    private int gardenHealth;

    private Transform playerTransform;
    private Transform flowerTransform;

    private float gardenDamageTimer = 0f;
    private float playerDamageTimer = 0f; // <-- NEW TIMER

    private void Start()
    {
        playerHealth = playerMaxHealth;
        gardenHealth = gardenMaxHealth;
        UpdateHealthUI();

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        flowerTransform = GameObject.FindGameObjectWithTag("flower")?.transform;

        StartCoroutine(SpawnChickens());
        StartCoroutine(UpdateTimer());
    }

    private void Update()
    {
        ApplyGardenDamageOverTime();
        ApplyPlayerDamage();
    }

    // --------------------------------------------------
    // GARDEN DAMAGE
    // --------------------------------------------------
    private void ApplyGardenDamageOverTime()
    {
        if (flowerTransform == null) return;

        GameObject[] chickens = GameObject.FindGameObjectsWithTag("Chicken");
        bool chickenNearFlower = false;

        foreach (GameObject c in chickens)
        {
            Vector3 a = new Vector3(c.transform.position.x, 0, c.transform.position.z);
            Vector3 b = new Vector3(flowerTransform.position.x, 0, flowerTransform.position.z);

            if (Vector3.Distance(a, b) <= flowerReachDistance)
            {
                chickenNearFlower = true;
                break;
            }
        }

        if (chickenNearFlower)
        {
            gardenDamageTimer += Time.deltaTime;

            if (gardenDamageTimer >= gardenDamageInterval)
            {
                DamageGarden(gardenDamageAmount);
                gardenDamageTimer = 0f;
            }
        }
        else
        {
            gardenDamageTimer = 0f;
        }
    }

    // --------------------------------------------------
    // PLAYER DAMAGE (now on cooldown)
    // --------------------------------------------------
    private void ApplyPlayerDamage()
    {
        if (playerTransform == null) return;

        GameObject[] chickens = GameObject.FindGameObjectsWithTag("Chicken");

        bool chickenClose = false;

        foreach (GameObject c in chickens)
        {
            if (Vector3.Distance(c.transform.position, playerTransform.position) <= playerDamageDistance)
            {
                chickenClose = true;
                break;
            }
        }

        if (chickenClose)
        {
            playerDamageTimer += Time.deltaTime;

            if (playerDamageTimer >= playerDamageInterval)
            {
                DamagePlayer(playerDamage);
                playerDamageTimer = 0f;
            }
        }
        else
        {
            playerDamageTimer = 0f;
        }
    }

    // --------------------------------------------------
    // SPAWNING
    // --------------------------------------------------
    private IEnumerator SpawnChickens()
    {
        while (true)
        {
            var allChickens = GameObject.FindGameObjectsWithTag("Chicken");

            if (allChickens.Length < maxChickens)
            {
                int index = Random.Range(0, spawnPoints.Length);
                GameObject chicken = Instantiate(chickenPrefab, spawnPoints[index].position, Quaternion.identity);
                chicken.tag = "Chicken";

                var ai = chicken.GetComponent<ithappy.Animals_FREE.AIChickenController>();
                if (ai != null)
                {
                    ai.player = playerTransform;
                    ai.flowerTarget = flowerTransform;
                    ai.gardenZone = gardenZone;
                }
            }

            UpdateChickenCounter();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // --------------------------------------------------
    // UI + TIMER
    // --------------------------------------------------
    private IEnumerator UpdateTimer()
    {
        while (true)
        {
            survivalTime += Time.deltaTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(survivalTime / 60f);
                int seconds = Mathf.FloorToInt(survivalTime % 60f);
                timerText.text = $"Time Survived: {minutes:00}:{seconds:00}";
            }

            yield return null;
        }
    }

    private void UpdateChickenCounter()
    {
        int count = GameObject.FindGameObjectsWithTag("Chicken").Length;
        chickenCountText.text = $"Chickens Alive: {count}";
    }

    private void UpdateHealthUI()
    {
        playerHealthText.text = $"Player Health: {playerHealth}";
        gardenHealthText.text = $"Garden Health: {gardenHealth}";
    }

    // --------------------------------------------------
    // HEALTH MANAGEMENT
    // --------------------------------------------------
    public void DamagePlayer(int amount)
    {
        playerHealth -= amount;
        playerHealth = Mathf.Max(0, playerHealth);
        UpdateHealthUI();
        if (playerHealth <= 0)
            GameOver();
    }

    public void DamageGarden(int amount)
    {
        gardenHealth -= amount;
        gardenHealth = Mathf.Max(0, gardenHealth);
        UpdateHealthUI();
        if (gardenHealth <= 0)
            GameOver();
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        messageText.text = "GAME OVER!";
        messageText.gameObject.SetActive(true);
    }
}
