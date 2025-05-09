using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

[System.Serializable]
public class LevelConfig
{
    public string levelName;
    public int startingBudget;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public LevelConfig[] levels;
    private int currentLevelIndex = 0;

    [Header("Vehicle Testing")]
    public GameObject vehiclePrefab;
    public Transform vehicleStartPoint;
    public Transform vehicleFinishPoint;

    [Header("UI")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteText;
    public Button nextButton;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Button restartButton;

    [Header("AR Template")]
    public ARTemplateMenuManager arMenuManager;

    // State flags
    private bool levelStarted = false;
    private bool testTriggered = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Hide all panels/buttons initially
        levelCompletePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // Wire UI callbacks
        nextButton.onClick.AddListener(AdvanceLevel);
        restartButton.onClick.AddListener(RestartGame);
    }

    /// <summary>
    /// Called by “Start Game” UI button.
    /// </summary>
    public void OnGameStartButtonClicked()
    {
        if (!levelStarted)
        {
            levelStarted = true;
            StartLevel();
        }
    }

    /// <summary>
    /// Called by “Test Bridge” UI button.
    /// </summary>
    public void OnTestButtonClicked()
    {
        if (!levelStarted)
        {
            Debug.LogWarning("Please click 'Start Game' first.");
            return;
        }
        if (!testTriggered)
        {
            testTriggered = true;
            SpawnVehicle();
        }
    }

    /// <summary>
    /// Sets up the next level, including initializing the budget.
    /// </summary>
    void StartLevel()
    {
        if (currentLevelIndex >= levels.Length)
        {
            // No more levels → Game Over
            ShowGameOverUI();
            return;
        }

        var lvl = levels[currentLevelIndex];
        Debug.Log("Starting Level: " + lvl.levelName);

        // initialize budget for this level
        if (BudgetManager.Instance != null)
            BudgetManager.Instance.InitBudget(lvl.startingBudget);

        // Reset per‐level state
        testTriggered = false;
    }

    void SpawnVehicle()
    {
        if (vehiclePrefab == null || vehicleStartPoint == null || vehicleFinishPoint == null)
        {
            Debug.LogError("Missing vehicle prefab or start/finish points");
            return;
        }

        var vehicle = Instantiate(vehiclePrefab, vehicleStartPoint.position, Quaternion.identity);
        var tester = vehicle.GetComponent<VehicleTester>();
        if (tester != null)
        {
            tester.startPoint = vehicleStartPoint;
            tester.finishPoint = vehicleFinishPoint;
        }
        else
        {
            Debug.LogError("Vehicle prefab missing VehicleTester component");
        }
    }

    /// <summary>
    /// Called by VehicleTester when the vehicle reaches the finish point.
    /// </summary>
    public void CompleteLevel()
    {
        Debug.Log("Level completed: " + levels[currentLevelIndex].levelName);
        ShowLevelCompleteUI();
    }

    void ShowLevelCompleteUI()
    {
        levelCompleteText.text = $"Level {currentLevelIndex + 1} Complete!";
        levelCompletePanel.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called by the Next Level button to proceed.
    /// </summary>
    public void AdvanceLevel()
    {
        // Hide Level Complete UI
        levelCompletePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        // Advance the index
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            ShowGameOverUI();
        }
        else
        {
            // Clear all placed objects via the AR template’s built-in method
            arMenuManager?.ClearAllObjects();

            testTriggered = false;
            StartLevel();
        }
    }

    void ShowGameOverUI()
    {
        Debug.Log("All levels finished. Game Over!");
        gameOverPanel.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called by the Restart button to reset the game.
    /// Immediately starts Level 1 so Test Bridge works right away.
    /// </summary>
    public void RestartGame()
    {
        // Hide Game Over UI
        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // Reset state
        currentLevelIndex = 0;
        levelStarted = false;
        testTriggered = false;

        // Clear any remaining objects via the AR template
        arMenuManager?.ClearAllObjects();

        // Start first level automatically
        levelStarted = true;
        StartLevel();
    }
}
