using UnityEngine;
using UnityEngine.UI;
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
    public Transform vehicleStartPoint;   // Inspector fallback
    public Transform vehicleFinishPoint;  // Inspector fallback
    public float spawnHeightOffset = 0.5f;

    [Header("AR Template")]
    [Tooltip("Assign the ARTemplateMenuManager from your scene")]
    public ARTemplateMenuManager arMenuManager;

    [Header("UI")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteText;
    public Button nextButton;

    [Header("Test Bridge UI")]
    public Button testBridgeButton;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Button restartButton;

    // Tracks the most recently placed bridge instance
    private BridgeExtendable currentBridge;

    // State flags
    private bool levelStarted  = false;
    private bool testTriggered = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        // Subscribe to AR object spawns so we can detect bridge prefabs
        if (arMenuManager != null && arMenuManager.objectSpawner != null)
            arMenuManager.objectSpawner.objectSpawned += OnObjectSpawned;
    }

    void OnDisable()
    {
        if (arMenuManager != null && arMenuManager.objectSpawner != null)
            arMenuManager.objectSpawner.objectSpawned -= OnObjectSpawned;
    }

    /// <summary>
    /// Called whenever the AR spawner creates an object. If it's a bridge root,
    /// we keep a reference for later vehicle spawning.
    /// </summary>
    private void OnObjectSpawned(GameObject spawned)
    {
        if (spawned.TryGetComponent<BridgeExtendable>(out var bridge))
        {
            currentBridge = bridge;
            Debug.Log("LevelManager: Registered new bridge instance.");
        }
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

        testBridgeButton.gameObject.SetActive(false);  
    }

    void Update()
    {
        // Only show Test Bridge once the level’s started, 
        // we haven’t already triggered a test, 
        // and the current bridge has ≥1 segment:
        bool canTest = levelStarted 
                       && !testTriggered 
                       && currentBridge != null 
                       && currentBridge.HasSegments();
        testBridgeButton.gameObject.SetActive(canTest);
    }
    
    /// <summary>
    /// Called by the “Start Game” UI button.
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
    /// Called by the “Test Bridge” UI button.
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
    /// Initializes the next level: sets budget, resets flags.
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

        // Initialize budget
        if (BudgetManager.Instance != null)
            BudgetManager.Instance.InitBudget(lvl.startingBudget);

        testTriggered = false;
    }

    /// <summary>
    /// Spawns the vehicle at the bridge’s actual start/end, or fallbacks.
    /// </summary>
    void SpawnVehicle()
    {
        if (vehiclePrefab == null)
        {
            Debug.LogError("Missing vehicle prefab");
            return;
        }

        // Prefer the dynamic bridge’s endpoints if available
        Transform startT = currentBridge != null ? currentBridge.StartPoint : vehicleStartPoint;
        Transform   endT = currentBridge != null ? currentBridge.EndPoint   : vehicleFinishPoint;

        if (startT == null || endT == null)
        {
            Debug.LogError("Missing start/finish points");
            return;
        }

        Vector3 spawnPos = startT.position + startT.up * spawnHeightOffset;
        Vector3 direction = (endT.position - startT.position).normalized;

        Quaternion lookRot = Quaternion.LookRotation(direction);
        Quaternion flipped = lookRot * Quaternion.Euler(0, 180f, 0);

        var vehicle = Instantiate(vehiclePrefab, spawnPos, flipped);

        
        float desiredScaleFactor = 0.2f; 
        vehicle.transform.localScale = Vector3.one * desiredScaleFactor;
        var tester  = vehicle.GetComponent<VehicleTester>();
        if (tester != null)
        {
            tester.startPoint  = startT;
            tester.finishPoint = endT;
        }
        else
        {
            Debug.LogError("Vehicle prefab missing VehicleTester component");
        }
    }

    /// <summary>
    /// Called by the VehicleTester when the vehicle crosses the finish.
    /// </summary>
    public void CompleteLevel()
    {
        Debug.Log("Level completed: " + levels[currentLevelIndex].levelName);
        ShowLevelCompleteUI();
    }

    void ShowLevelCompleteUI()
    {
        levelCompleteText.text        = $"Level {currentLevelIndex + 1} Complete!";
        levelCompletePanel.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Advances to the next level, clears placed objects, and resets state.
    /// </summary>
    public void AdvanceLevel()
    {
        levelCompletePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            ShowGameOverUI();
        }
        else
        {
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
    /// Resets the game back to level 1 and immediately starts it.
    /// </summary>
    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);

        currentLevelIndex = 0;
        levelStarted     = false;
        testTriggered    = false;

        arMenuManager?.ClearAllObjects();

        levelStarted = true;
        StartLevel();
    }
}
