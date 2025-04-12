using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private GameObject vehiclePrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterStartAndEnd(Transform start, Transform end)
    {
        startPoint = start;
        endPoint = end;
    }

    public void RunBridgeTest()
    {
        StartCoroutine(SendVehicleAcross());
    }

    private IEnumerator SendVehicleAcross()
    {
        GameObject vehicle = Instantiate(vehiclePrefab, startPoint.position, Quaternion.identity);

        float duration = 5f;
        float elapsed = 0f;

        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vehicle.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        float distanceToEnd = Vector3.Distance(vehicle.transform.position, endPoint.position);
        if (distanceToEnd < 0.3f)
        {
            Debug.Log("Bridge Passed!");
            // Pass msg goes here
        }
        else
        {
            Debug.Log("Bridge Failed.");
            // Fail msg goes here
        }
    }
}
