using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Paneller")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject gameOverPanel;

    [Header("UI Elemanları")]
    public TextMeshProUGUI scoreText;
    public Image nextFruitImage;

    [Header("Oyun Ayarları")]
    public Transform spawnPoint;
    public float xLimit = 2.5f;
    public GameObject[] fruitPrefabs;

    [Header("Çizgi Ayarı")]
    public SpriteRenderer aimLine;

    private GameObject currentFruit;
    private int nextFruitIndex;
    private int score;
    private bool gameActive = false;
    private Camera cam;

    [SerializeField] AudioManager audioManager;
    [SerializeField] TextMeshProUGUI gameOverScore;

    void Start()
    {
        cam = Camera.main;

        if (PlayerPrefs.HasKey("Restarted"))
        {
            PlayerPrefs.DeleteKey("Restarted");
            Time.timeScale = 1f;
            gameActive = true;

            mainMenuPanel.SetActive(false);
            gameUIPanel.SetActive(true);
            gameOverPanel.SetActive(false);

            score = 0;
            nextFruitIndex = Random.Range(0, 4);
            UpdateScoreUI();
            UpdateNextFruitUI();
            SpawnNewFruit();
        }
        else
        {
            ShowMainMenu();
        }
    }

    void Update()
    {
        if (!gameActive) return;

        if (currentFruit != null)
        {
            FollowMouseX();
            UpdateAimLine();

            if (Input.GetMouseButtonDown(0))
            {
                DropFruit();
            }
        }
        else
        {
            if (aimLine != null && aimLine.enabled)
                aimLine.enabled = false;
        }
    }

    // ---------------- MENU ----------------
    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
        mainMenuPanel.SetActive(true);
        gameUIPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameActive = false;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("Restarted", 1);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        aimLine.enabled = true;
    }

    void FollowMouseX()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        float clampedX = Mathf.Clamp(mousePos.x, -xLimit, xLimit);
        currentFruit.transform.position = new Vector3(clampedX, spawnPoint.position.y, 0f);
    }

    void DropFruit()
    {
        currentFruit.GetComponent<Rigidbody2D>().simulated = true;
        audioManager.PlaySFX();
        currentFruit.GetComponent<Fruit>().audioManager = this.audioManager;
        currentFruit = null;

        if (aimLine != null)
            aimLine.enabled = false;

        Invoke(nameof(SpawnNewFruit), 0.5f);
    }

    public void SpawnNewFruit()
    {
        if (!gameActive) return;

        int index = nextFruitIndex;
        currentFruit = Instantiate(fruitPrefabs[index], spawnPoint.position, Quaternion.identity);
        currentFruit.GetComponent<Rigidbody2D>().simulated = false;

        nextFruitIndex = Random.Range(0, 4);
        UpdateNextFruitUI();

        if (aimLine != null)
            aimLine.enabled = true;
    }

    void UpdateAimLine()
    {
        if (aimLine == null || currentFruit == null) return;

        Vector3 linePos = aimLine.transform.position;
        linePos.x = currentFruit.transform.position.x;
        aimLine.transform.position = linePos;
    }

    public void AddScore(float addedMass)
    {
        int points = Mathf.RoundToInt(addedMass * 100f);
        score += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    void UpdateNextFruitUI()
    {
        if (nextFruitImage != null && nextFruitIndex < fruitPrefabs.Length)
        {
            SpriteRenderer sr = fruitPrefabs[nextFruitIndex].GetComponent<SpriteRenderer>();
            if (sr != null)
                nextFruitImage.sprite = sr.sprite;
        }
    }
    public void GameOver()
    {
        if (!gameActive) return;

        aimLine.enabled = false;
        gameActive = false;
        Time.timeScale = 0f;

        gameOverPanel.SetActive(true);
        gameOverScore.text = score.ToString();
        gameUIPanel.SetActive(false);
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("Restarted", 1); // işaret bırak
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        aimLine.enabled = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 0f;
        gameActive = false;

        if (currentFruit != null)
        {
            Destroy(currentFruit);
            currentFruit = null;
        }

        mainMenuPanel.SetActive(true);
        gameUIPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Build edilmiş oyunda çalışıyorsa uygulamayı kapat
        Application.Quit();
#endif
    }
}
