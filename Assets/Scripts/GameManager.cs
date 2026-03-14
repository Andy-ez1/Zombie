using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Zombie Selection")]
    public List<GameObject> zombies;           
    public GameObject selectedZombie;          
    public Vector3 selectedSize = new Vector3(1.5f, 1.5f, 1.5f);
    public Vector3 defaultSize = Vector3.one;
    private int selectedZombiePosition = 0;

    [Header("Score")]
    public TMP_Text scoreText;                 
    private int score = 0;

    [Header("Timer & GameOver")]
    public TMP_Text timerText;                 
    private float timer = 0f;                  
    private bool timerRunning = true;
    public GameObject gameOverPanel;           
    public TMP_Text finalScoreText;            

    [Header("Collectibles")]
    public GameObject brainPrefab;             
    public float spawnInterval = 3f;           
    private float spawnTimer = 0f;

    // =========================
    // Papildus arrays collectibles uzraudzībai
    // =========================
    private GameObject[] activeBrains;       // saglabā katras līnijas collectible
    private float[] xPositions = { -7.5f, -4.5f, -1.46f, 1.53f }; // katram zombijam
    private float zMin = -9f;
    private float zMax = 6f;

    void Start()
    {
        if (zombies.Count > 0)
            SelectZombie(zombies[0]);

        UpdateScoreUI();
        if (timerText != null)
            timerText.text = "Time: 0.0s";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Inicializē activeBrains masīvu
        activeBrains = new GameObject[xPositions.Length];
    }

    void Update()
    {
        // =========================
        // Input kontrole
        // =========================
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            GetZombieLeft();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            GetZombieRight();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            PushUp();

        // =========================
        // Timer
        // =========================
        if (timerRunning)
        {
            timer += Time.deltaTime;
            if (timerText != null)
                timerText.text = "Time: " + timer.ToString("F1") + "s";
        }

        // =========================
        // Lose state pārbaude
        // =========================
        CheckLose();

        // =========================
        // Spawn collectibles pa zombiju līnijām
        // =========================
        if (timerRunning && brainPrefab != null)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnBrain();
                spawnTimer = 0f;
            }
        }
    }

    // =========================
    // ZOMBIE SELECTION
    // =========================
    public void GetZombieLeft()
    {
        if (selectedZombiePosition == 0)
            selectedZombiePosition = zombies.Count - 1;
        else
            selectedZombiePosition--;

        SelectZombie(zombies[selectedZombiePosition]);
    }

    public void GetZombieRight()
    {
        selectedZombiePosition++;
        if (selectedZombiePosition >= zombies.Count)
            selectedZombiePosition = 0;

        SelectZombie(zombies[selectedZombiePosition]);
    }

    public void SelectZombie(GameObject newZombie)
    {
        if (selectedZombie != null)
            selectedZombie.transform.localScale = defaultSize;

        selectedZombie = newZombie;
        selectedZombie.transform.localScale = selectedSize;

        selectedZombiePosition = zombies.IndexOf(selectedZombie);
    }

    // =========================
    // PUSH UP
    // =========================
    public void PushUp()
    {
        if (selectedZombie != null)
        {
            Rigidbody rb = selectedZombie.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(0, 5, 5, ForceMode.Impulse);
        }
    }

    // =========================
    // SCORE
    // =========================
    public void AddScore()
    {
        score += 1;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    // =========================
    // LOSE STATE
    // =========================
    void CheckLose()
    {
        if (!timerRunning)
            return;

        bool allFallen = true;

        foreach (GameObject z in zombies)
        {
            if (z != null && z.transform.position.y > -5f) 
            {
                allFallen = false;
                break;
            }
        }

        if (allFallen)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        timerRunning = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "FinalScore: " + score;
    }

    // =========================
    // RESTART
    // =========================
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        timer = 0f;
        timerRunning = true;
    }

    // =========================
    // SPAWN COLLECTIBLES PA ZOMBIJU LĪNIJĀM
    // =========================
    void SpawnBrain()
    {
        for (int i = 0; i < xPositions.Length; i++)
        {
            // Ja līnija jau aizņemta, nepievieno
            if (activeBrains[i] != null) continue;

            float spawnX = xPositions[i];
            float spawnZ = Random.Range(zMin, zMax);

            // Raycast no augšas, lai Y koordināta vienmēr uz platformas
            Vector3 rayOrigin = new Vector3(spawnX, 20f, spawnZ);
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 40f))
            {
                Vector3 spawnPos = hit.point + new Vector3(0f, 0.2f, 0f); // nedaudz virs platformas
                GameObject brain = Instantiate(brainPrefab, spawnPos, Quaternion.identity);

                // Saglabā masīvā, lai zinātu, ka līnija aizņemta
                activeBrains[i] = brain;

                // Pievieno collectible scriptam callback, lai atbrīvotu līniju, kad collectible tiek savākts
                Brain brainScript = brain.GetComponent<Brain>();
                if (brainScript != null)
                {
                    brainScript.SetLineIndex(i, this);
                }
            }
        }
    }

    // =========================
    // Atbrīvo līniju, kad collectible savākts
    // =========================
    public void FreeLine(int lineIndex)
    {
        if (lineIndex >= 0 && lineIndex < activeBrains.Length)
        {
            activeBrains[lineIndex] = null;
        }
    }
}
 