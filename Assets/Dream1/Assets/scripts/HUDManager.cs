using UnityEngine;
using UnityEngine.UI; // 引用 UI
using TMPro; // 如果你用的是 TextMeshPro，请取消注释这行，把下面的 Text 改成 TextMeshProUGUI

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("绑定 UI 组件")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI moneyText;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 更新血量
    public void UpdateHealth(int current, int max)
    {
        if (hpSlider != null) hpSlider.value = (float)current / max;
        if (hpText != null) hpText.text = $"HP: {current}/{max}";
    }

    // 更新攻击力
    public void UpdateAttack(int dmg)
    {
        if (atkText != null) atkText.text = $"ATK: {dmg}";
    }

    // 更新金钱
    public void UpdateMoney(int amount)
    {
        if (moneyText != null) moneyText.text = $"$ {amount}";
    }
}
