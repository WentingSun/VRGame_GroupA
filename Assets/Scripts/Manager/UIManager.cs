using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image resurrectionImage;
    [SerializeField] private TextMeshProUGUI ballCountText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Sprite Resources")]
    [SerializeField] private Sprite shieldSprite;
    [SerializeField] private Sprite resurrectionSprite;

    [Header("Colors")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;

    private void Start()
    {
        // 初始化护盾和复活状态
        shieldImage.sprite = shieldSprite;
        resurrectionImage.sprite = resurrectionSprite;

        UpdateShieldStatus(GameManager.Instance.ProtectShell);
        UpdateResurrectionStatus(GameManager.Instance.isResurrection);
        UpdateBallCount(GameManager.Instance.remainingSmallBallNum);
        UpdateHealth(GameManager.Instance.CurrentPlayerHealth);
        UpdateScore(GameManager.Instance.Score);

        // 订阅事件
        GameManager.OnGameEventSent += OnGameEventSent;
        GameManager.OnPlayerStateChange += OnPlayerStateChanged;
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        GameManager.OnGameEventSent -= OnGameEventSent;
        GameManager.OnPlayerStateChange -= OnPlayerStateChanged;
    }

    private void OnGameEventSent(GameEvent gameEvent)
    {
        // 根据游戏事件更新护盾和复活状态
        switch (gameEvent)
        {
            case GameEvent.ProtectShellBreak: // 护盾被破坏
                UpdateShieldStatus(false);
                break;

            case GameEvent.GetProtectShell: // 获得护盾
                UpdateShieldStatus(true);
                break;

            case GameEvent.GetResurrection: // 获得复活
                UpdateResurrectionStatus(true);
                break;

            case GameEvent.ResurrectionUsed: // 使用复活
                UpdateResurrectionStatus(false);
                break;

            case GameEvent.ScoreUpdated:
                UpdateScore(GameManager.Instance.Score);
                break;
            
        }
    }

    private void OnPlayerStateChanged(PlayerState newState)
    {
        // 根据玩家状态更新血量和小球数量
        UpdateHealth(GameManager.Instance.CurrentPlayerHealth);
        UpdateBallCount(GameManager.Instance.remainingSmallBallNum);
        UpdateScore(GameManager.Instance.Score);
    }

    private void UpdateShieldStatus(bool hasShield)
    {
        shieldImage.color = hasShield ? activeColor : inactiveColor;
    }

    private void UpdateResurrectionStatus(bool hasResurrection)
    {
        resurrectionImage.color = hasResurrection ? activeColor : inactiveColor;
    }

    private void UpdateBallCount(int ballCount)
    {
        ballCountText.text = $"x {ballCount}";
    }

    private void UpdateHealth(int health)
    {
        healthText.text = $"x {health}";
    }

    private void UpdateScore(float score)
    {
        scoreText.text = $"x {score:F1}"; // 保留一位小数
    }
}