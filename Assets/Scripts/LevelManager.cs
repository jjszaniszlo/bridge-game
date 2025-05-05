using UnityEngine;
using UnityEngine.UI;              
using TMPro;                   

[System.Serializable]
public class LevelConfig {
    public string levelName;
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
    // public Text levelCompleteText;           
    public Button nextButton;                
    public TextMeshProUGUI levelCompleteText; 

    // State flags
    private bool levelStarted  = false;
    private bool testTriggered = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // hide UI on launch
        levelCompletePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        // wire up the NextButton
        nextButton.onClick.AddListener(AdvanceLevel);
    }

    /// Called by “Start Game” button in the scene
    public void OnGameStartButtonClicked()
    {
        if (!levelStarted)
        {
            levelStarted = true;
            Debug.Log("Game started!");
            StartLevel();
        }
    }

    /// Called by “Test Bridge” button in the scene
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

    void StartLevel()
    {
        if (currentLevelIndex >= levels.Length)
        {
            Debug.Log("All levels completed!");
            return;
        }

        var lvl = levels[currentLevelIndex];
        Debug.Log("Starting Level: " + lvl.levelName);

        // reset the Test Bridge button for this level
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
        var tester  = vehicle.GetComponent<VehicleTester>();
        if (tester != null)
        {
            tester.startPoint  = vehicleStartPoint;
            tester.finishPoint = vehicleFinishPoint;
        }
        else Debug.LogError("Vehicle prefab missing VehicleTester component");
    }

    // Called by VehicleTester when the vehicle reaches the finish point
    public void CompleteLevel()
    {
        Debug.Log("Level completed: " + levels[currentLevelIndex].levelName);
        ShowLevelCompleteUI();
    }

    // Display the panel and NextLevel button
    void ShowLevelCompleteUI()
    {
        levelCompleteText.text        = $"Level {currentLevelIndex + 1} Complete!";
        levelCompletePanel.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called by the NextButton to proceed to the next level
    /// </summary>
    public void AdvanceLevel()
    {
        // hide the UI
        levelCompletePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        // increment and start the next level
        currentLevelIndex++;
        testTriggered = false;
        StartLevel();
    }
}
