using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DaisyPlacer : MonoBehaviour
{
    [Header("Prefab to Place")]
    public GameObject targetDaisyPrefab;

    [Header("Placement Settings")]
    public LayerMask placementLayer;

    [Header("Cooldown Settings")]
    public float placementCooldown = 30f;
    private float nextPlacementTime = 0f;

    [Header("Sound")]
    public AudioClip placeSound;
    private AudioSource audioSource;

    private EnduranceManager enduranceManager;

    private bool aButtonWasPressed = false; // tracks Button South press

    private void Start()
    {
        enduranceManager = FindObjectOfType<EnduranceManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Mouse input
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceDaisy();
        }

#if ENABLE_INPUT_SYSTEM
        // Gamepad input
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.buttonSouth.isPressed && !aButtonWasPressed)
            {
                TryPlaceDaisy();
                aButtonWasPressed = true;
            }

            if (!gamepad.buttonSouth.isPressed)
            {
                aButtonWasPressed = false;
            }
        }
#endif
    }

    private void TryPlaceDaisy()
    {
        if (Time.time < nextPlacementTime) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
        {
            Instantiate(targetDaisyPrefab, hit.point, Quaternion.identity);

            if (enduranceManager != null)
                enduranceManager.AddGardenHealth(10);

            if (audioSource != null && placeSound != null)
                audioSource.PlayOneShot(placeSound);

            nextPlacementTime = Time.time + placementCooldown;
        }
    }
}
