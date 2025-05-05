using UnityEngine;

public class VehicleTester : MonoBehaviour
{
    public Transform startPoint;
    public Transform finishPoint;
    public float speed = 1.0f;

    private bool hasReachedFinish = false;

    void Start()
    {
        // Setting vehicle starting pos
        if (startPoint != null)
            transform.position = startPoint.position;
    }

    void Update()
    {
        if (hasReachedFinish || startPoint == null || finishPoint == null)
            return;

        // Simple movement from start to end pos
        transform.position = Vector3.MoveTowards(transform.position, finishPoint.position, speed * Time.deltaTime);

        // End point detection
        if (Vector3.Distance(transform.position, finishPoint.position) < 0.1f)
        {
            hasReachedFinish = true;
            Debug.Log("Vehicle has reached the finish point.");
            LevelManager.Instance.CompleteLevel();

            // Remove vehicle to start next level
            Destroy(gameObject);
        }
    }
}
