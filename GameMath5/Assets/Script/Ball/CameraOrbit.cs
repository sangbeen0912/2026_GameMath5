using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    private float yaw = 0f;
    public float moveInput = 0f;

    public float rotateSpeed = 100f;
    public Vector3 offset = new Vector3(0f, 4f, -7f);

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    void Update()
    {
        yaw += moveInput * rotateSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 rotatedOffset = rotation * offset;
        transform.position = target.position + rotatedOffset;
        transform.LookAt(target);
    }
}