using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public GameManager.Turn ballOwner; // 이 공이 1P 공인지 2P 공인지 인스펙터에서 설정

    private void OnCollisionEnter(Collision collision)
    {
        // 게임이 끝났거나, 이번 턴에 아직 치지 않았다면 충돌 연산 패스
        if (GameManager.Instance.isGameOver || !GameManager.Instance.isBallMoving) return;

        // 현재 공격 주도권을 쥔 플레이어의 공이 움직이다가 부딪힌 경우만 계산
        if (GameManager.Instance.currentTurn != ballOwner) return;

        // 요구사항 5: 타겟 공을 맞췄을 때
        if (collision.gameObject.CompareTag("Target"))
        {
            // 타겟 공은 여러 번 충돌할 수 있으므로, 예시로 충돌 시 점수 획득 처리
            // (주의: 완벽한 규칙을 위해선 한 턴에 맞춘 타겟들을 리스트로 체크해야 하지만 우선 기본 충돌로 구현)
            GameManager.Instance.AddScore(ballOwner, 1);

            // 점수를 얻은 타겟은 파괴하거나 비활성화 처리 (원치 않으면 아래 줄 삭제)
            Destroy(collision.gameObject);
        }
        // 요구사항 6: 상대방 플레이어 공을 맞췄을 때
        else if (collision.gameObject.CompareTag("PlayerBall"))
        {
            BallCollision otherBall = collision.gameObject.GetComponent<BallCollision>();
            if (otherBall != null && otherBall.ballOwner != ballOwner)
            {
                // 상대 공을 맞췄으므로 감점 (-1)
                GameManager.Instance.AddScore(ballOwner, -1);
            }
        }
    }
}