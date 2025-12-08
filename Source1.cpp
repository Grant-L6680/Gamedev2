CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Assign your player here in the Inspector
    public Vector3 offset = new Vector3(0, 5, -10);

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}


PlayerMovement.cs
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        Vector3 move = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}