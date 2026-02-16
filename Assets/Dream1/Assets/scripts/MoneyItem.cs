using UnityEngine;

public class MoneyItem : MonoBehaviour
{
    [Header("金钱设置")]
    public int amount = 100; // 这堆钱值多少
    public GameObject tipUI; // 拖入一个子物体（比如文字"按E拾取"），默认隐藏

    private bool canPickup = false; // 标记玩家是否在范围内

    void Start()
    {
        // 游戏开始时，确保提示文字是隐藏的
        if (tipUI != null) tipUI.SetActive(false);
    }

    void Update()
    {
        // 只有当玩家在范围内，且按下 E 键时触发
        if (canPickup && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    // 当玩家走进金钱的检测范围
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = true;
            // 显示提示文字
            if (tipUI != null) tipUI.SetActive(true);
        }
    }

    // 当玩家离开金钱范围
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = false;
            // 隐藏提示文字
            if (tipUI != null) tipUI.SetActive(false);
        }
    }

    void PickUp()
    {
        // 1. 加钱逻辑
        // 假设你有一个 GameManager 存钱，这里调用它
        GameManager.Instance.AddMoney(amount);

        // 简单测试：打印日志
        Debug.Log($"捡到了 {amount} 块钱！梦境能量上升！");

        // 2. 播放音效 (可选)
        // AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // 3. 销毁金钱物体
        Destroy(gameObject);
    }
}
