using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelConfig {
    public string levelName;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    // Inspector
    [Header("Level Settings")]
    public LevelConfig[] levels;       
    private int currentLevelIndex = 0;

    [Header("Vehicle Testing")]
    public GameObject vehiclePrefab;   
    public Transform vehicleStartPoint; 
    public Transform vehicleFinishPoint; 

    // State flags for button clicks
    private bool levelStarted = false;
    private bool testTriggered = false;

    void Awake()
    {   
        // Level start trigger
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

  
    void Start()
    {
        // Waiting for Start Game button click...
    }

   
    /// Called by the Start Game button to begin first level
    public void OnGameStartButtonClicked()
    {
        if (!levelStarted)
        {
            levelStarted = true;
            Debug.Log("Game started!");
            StartLevel();
        }
    }

    
    /// Called by the Test Bridge button to send vehicle
    public void OnTestButtonClicked()
    {
        if (levelStarted && !testTriggered)
        {
            testTriggered = true;
            // Spawn vehicle for testing
            SpawnVehicle();
        }
        else if (!levelStarted)
        {
            Debug.LogWarning("The game hasn't started yet. Click 'Start Game' first!");
        }
    }

    void StartLevel()
    {
        if (currentLevelIndex >= levels.Length)
        {
            Debug.Log("All levels completed!");
            return;
        }

        LevelConfig currentLevel = levels[currentLevelIndex];
        Debug.Log("Starting Level: " + currentLevel.levelName);

        // Reset or update logic goes here
        // Reset the test trigger for the new level
        testTriggered = false;

        // Vehicle will be spawned by clicking the Test Bridge button
    }

    void SpawnVehicle()
    {
        if (vehiclePrefab == null || vehicleStartPoint == null || vehicleFinishPoint == null)
        {
            Debug.LogError("Missing vehicle prefab or start/finish points!");
            return;
        }

        // Instantiate the vehicle at the start point
        GameObject vehicleInstance = Instantiate(vehiclePrefab, vehicleStartPoint.position, Quaternion.identity);

        // Set up VehicleTester on the spawned vehicle
        VehicleTester tester = vehicleInstance.GetComponent<VehicleTester>();
        if (tester != null)
        {
            tester.startPoint = vehicleStartPoint;
            tester.finishPoint = vehicleFinishPoint;
        }
        else
        {
            Debug.LogError("Vehicle prefab missing VehicleTester component!");
        }
    }

    // Called by VehicleTester when the vehicle reaches finish point
    public void CompleteLevel()
    {
        Debug.Log("Level completed: " + levels[currentLevelIndex].levelName);
        StartCoroutine(TransitionToNextLevel());
    }

    IEnumerator TransitionToNextLevel()
    {
        // Delay for transition effects (e.g. animations, UI feedback)
        yield return new WaitForSeconds(2.0f);
        currentLevelIndex++;
        StartLevel();
    }
}
