using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycastTest : MonoBehaviour
{
    public CameraOrbit cam;
    float moveInput;
    public float rayDistance = 100f;
    public float pushForce = 500f; 
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
        cam.moveInput = moveInput;
    }

    public void OnClick(InputValue value)
    {
        if (GameManager.Instance.isGameOver || GameManager.Instance.isBallMoving) return;
        if (!value.isPressed) return;

        Debug.Log("1단계: 마우스 클릭 인식됨!"); // <--- 추가

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Debug.Log($"2단계: 레이캐스트가 {hit.collider.name}에 부딪힘!"); // <--- 추가

            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                BallCollision ball = rb.GetComponent<BallCollision>();
                if (ball != null)
                {
                    Debug.Log($"3단계: 부딪힌 공의 주인은 {ball.ballOwner} / 현재 턴은 {GameManager.Instance.currentTurn}"); // <--- 추가
                    if (ball.ballOwner == GameManager.Instance.currentTurn)
                    {
                        Debug.Log("4단계: 조건 일치! 힘을 가합니다!"); // <--- 추가
                        Vector3 force = ray.direction * pushForce;
                        rb.AddForce(force , ForceMode.Impulse);
                        GameManager.Instance.OnBallFired();
                    }
                }
            }
        }
    }
}