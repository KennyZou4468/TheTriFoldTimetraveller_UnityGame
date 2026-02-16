using UnityEngine;
using UnityEngine.SceneManagement; // 用于退出或加载主菜单

public class PauseMenu : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject pauseMenuUI; // 把那个 PauseMenu_Panel 拖进来

    // 一个全局变量，让别的脚本（比如开枪脚本）知道现在暂停了
    public static bool GameIsPaused = false;

    void Update()
    {
        // 监听 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume(); // 如果已经是暂停，就继续
            }
            else
            {
                Pause(); // 否则就暂停
            }
        }
    }

    // --- 继续游戏 ---
    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false); // 隐藏菜单
        Time.timeScale = 1f; // 🔴 恢复时间流动 (物理和动画恢复)
        GameIsPaused = false;
    }

    // --- 暂停游戏 ---
    void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true); // 显示菜单
        Time.timeScale = 0f; // 🔴 停止时间流动 (静止世界)
        GameIsPaused = true;
    }

    // --- 退出游戏 (绑在按钮上) ---
    public void QuitGame()
    {
        Debug.Log("退出游戏！");
        Application.Quit(); // 打包后才生效
    }
}
