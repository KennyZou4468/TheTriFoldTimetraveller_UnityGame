using UnityEngine;

public class StaminaRechargeZone : MonoBehaviour
{
    [Tooltip("体力加速恢复的倍数。例如设为 3，则恢复速度是原来的 3 倍。")]
    public float rechargeMultiplier = 2.5f;

    private const string PlayerTag = "Player"; // 确保你的玩家对象标签是 "Player"

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            Player1Controller player = other.GetComponent<Player1Controller>();
            if (player != null)
            {
                // 通知玩家控制器，进入了加速区域
                player.SetRechargeMultiplier(rechargeMultiplier);
                Debug.Log("进入加速区域，体力恢复速度提高。");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            Player1Controller player = other.GetComponent<Player1Controller>();
            if (player != null)
            {
                // 通知玩家控制器，离开了加速区域，恢复倍数为 1 (正常值)
                player.SetRechargeMultiplier(1f);
                Debug.Log("离开加速区域，体力恢复速度恢复正常。");
            }
        }
    }
}