using UnityEngine;

public class VehicleTester : MonoBehaviour
{
    public Transform startPoint;
    public Transform finishPoint;
    public float speed = 1.0f;

    bool hasReachedFinish = false;
    float _spawnY;

    void Start()
    {
        // Record the Y height at spawn, and snap to it
        _spawnY = transform.position.y;
        transform.position = new Vector3(transform.position.x, _spawnY, transform.position.z);
    }

    void Update()
    {
        if (hasReachedFinish || startPoint == null || finishPoint == null)
            return;

        // Build a target that has finishPoint's X/Z but stays at the spawn Y
        Vector3 target = new Vector3(finishPoint.position.x, _spawnY, finishPoint.position.z);

        // Move horizontally at constant Y
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        Vector2 currentXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetXZ  = new Vector2(target.x, target.z);
        if (Vector2.Distance(currentXZ, targetXZ) < 0.01f)
        {
            hasReachedFinish = true;
            Debug.Log("Vehicle has reached the finish point.");
            LevelManager.Instance.CompleteLevel();
            Destroy(gameObject);
        }
    }
}
