using UnityEngine;

public class ToolPicker : MonoBehaviour
{
    public Transform holdObject; // Assign your HOLD object in the inspector
    public float pickupRange = 2f; // Adjust as needed

    private GameObject heldTool;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press E to pick up
        {
            if (heldTool == null)
                TryPickupTool();
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to drop
        {
            if (heldTool != null)
                DropTool();
        }
    }

    void TryPickupTool()
    {
        // Find all colliders within pickupRange
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("TOOL"))
            {
                Pickup(collider.gameObject);
                break;
            }
        }
    }

    void Pickup(GameObject tool)
    {   
        heldTool = tool;

        // Move tool to HOLD position in world space before parenting
        tool.transform.position = holdObject.position;
        tool.transform.rotation = holdObject.rotation;

        // Attach tool to HOLD object
        tool.transform.SetParent(holdObject, worldPositionStays: true);

        // Optional: Reset local scale if needed
        tool.transform.localScale = Vector3.one;

        // Optional: Disable physics
        var rb = tool.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        // Ensure renderer is enabled
        var renderer = tool.GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.enabled = true;
    }

    void DropTool()
    {
        if (heldTool == null)
            return;

        heldTool.transform.SetParent(null);
        heldTool.transform.position = holdObject.position + holdObject.forward * 0.5f; // Drop in front of hand

        var rb = heldTool.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        heldTool = null;
    }
}