using UnityEngine;
using UnityEngine.UI; // 레거시 UI 사용 시 (만약 TextMeshPro를 쓰시면 TMPro로 변경)

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Turn { Player1, Player2 }
    [Header("Game State")]
    public Turn currentTurn = Turn.Player1;
    public bool isBallMoving = false;
    public bool isGameOver = false;

    [Header("Scores")]
    public int p1Score = 0;
    public int p2Score = 0;

    [Header("References")]
    public Rigidbody p1Ball;
    public Rigidbody p2Ball;
    public Rigidbody[] targetBalls; // 맞춰야 하는 타겟 공들

    [Header("UI Text")]
    public Text turnText;  // 화면에 턴 표시할 Text 컴포넌트
    public Text scoreText; // 화면에 점수 표시할 Text 컴포넌트

    private float stopThreshold = 0.05f; // 이 속도 이하이면 멈춘 것으로 간주
    private bool hasFired = false; // 이번 턴에 공을 쳤는지 여부

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // 공을 친 후에만 움직임 체크를 시작함
        if (hasFired)
        {
            CheckBallsMoving();
        }
    }

    // 공을 발사했을 때 MouseRaycastTest에서 호출할 함수 (요구사항 3)
    public void OnBallFired()
    {
        isBallMoving = true;
        hasFired = true;
    }

    // 모든 공이 멈췄는지 확인 (요구사항 4)
    void CheckBallsMoving()
    {
        bool anyBallMoving = false;

        // 1P, 2P 공 체크
        if (p1Ball.linearVelocity.magnitude > stopThreshold || p2Ball.linearVelocity.magnitude > stopThreshold)
        {
            anyBallMoving = true;
        }

        // 타겟 공들 체크
        foreach (Rigidbody target in targetBalls)
        {
            if (target != null && target.linearVelocity.magnitude > stopThreshold)
            {
                anyBallMoving = true;
                break;
            }
        }

        // 모든 공이 멈췄다면 턴 전환
        if (!anyBallMoving && isBallMoving)
        {
            isBallMoving = false;
            hasFired = false;
            ChangeTurn();
        }
    }

    void ChangeTurn()
    {
        currentTurn = (currentTurn == Turn.Player1) ? Turn.Player2 : Turn.Player1;
        UpdateUI();
    }

    // 점수 추가/감점 로직 (요구사항 5, 6, 7)
    public void AddScore(Turn player, int amount)
    {
        if (isGameOver) return;

        if (player == Turn.Player1)
        {
            p1Score = Mathf.Max(0, p1Score + amount); // 0점 이하로 안 내려가게 방지
        }
        else
        {
            p2Score = Mathf.Max(0, p2Score + amount);
        }

        UpdateUI();
        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (p1Score >= 5)
        {
            isGameOver = true;
            turnText.text = "1P 승리! 게임 종료";
        }
        else if (p2Score >= 5)
        {
            isGameOver = true;
            turnText.text = "2P 승리! 게임 종료";
        }
    }

    void UpdateUI()
    {
        if (isGameOver) return;
        turnText.text = (currentTurn == Turn.Player1) ? "현재 턴: 1P" : "현재 턴: 2P";
        scoreText.text = $"1P: {p1Score}점 | 2P: {p2Score}점";
    }
}