using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Make sure you have TextMeshPro imported

public class CameraFlyover : MonoBehaviour
{
    [Header("Flyover Settings")]
    public Transform[] waypoints;       // Assign start, middle, end points in inspector
    public float flySpeed = 10f;         // Camera movement speed
    public float rotationSpeed = 2f;    // Smooth rotation speed
    public float waitTimeAtWaypoint = 10f; // Seconds to pause at each waypoint

    [Header("Waypoint Messages")]
    public string[] messages;           // Messages for each waypoint
    public TMP_Text messageText;        // Assign your UI TextMeshPro object here

    private int currentWaypoint = 0;
    private bool isWaiting = false;

    void Update()
    {
        if (waypoints.Length == 0 || isWaiting) return;

        Transform target = waypoints[currentWaypoint];

        // Move towards current waypoint
        transform.position = Vector3.MoveTowards(transform.position, target.position, flySpeed * Time.deltaTime);

        // Rotate smoothly towards waypoint
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;

        // Display message for current waypoint
        if (messages != null && messages.Length > currentWaypoint && messageText != null)
        {
            messageText.text = messages[currentWaypoint];
            messageText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(waitTimeAtWaypoint);

        // Hide message after waiting
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        currentWaypoint++;
        isWaiting = false;

        if (currentWaypoint >= waypoints.Length)
        {
            ReturnToStartScene();
        }
    }

    private void ReturnToStartScene()
    {
        SceneManager.LoadScene("Start"); // Replace with your actual start scene name
    }
}
