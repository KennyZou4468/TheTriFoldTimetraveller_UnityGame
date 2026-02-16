using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dream2Timer : MonoBehaviour
{
    private string blueColorHex = "#4FF6FF";
    private string redColorHex = "#FF5651";
    public float startTime = 600f;
    public float timeLeft;
    private bool hasEnded = false;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    void Start()
    {
        timeLeft = startTime;
    }

    void Update()
    {
        if (timeLeft - Time.deltaTime >= 0)
        {
            timeLeft -= Time.deltaTime;
        }

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        if (seconds < 10)
            textMeshProUGUI.text = $"<color={blueColorHex}>{"Time left: "}</color>" + $"<color={redColorHex}>{minutes + ":0" + seconds}</color>";
        else
            textMeshProUGUI.text = $"<color={blueColorHex}>{"Time left: "}</color>" + $"<color={redColorHex}>{minutes + ":" + seconds}</color>";

        if (minutes == 0 && seconds == 0 && !hasEnded)
        {
            Dream2Manager.Instance.EndDream2();
            hasEnded = true;
        }

    }
}
