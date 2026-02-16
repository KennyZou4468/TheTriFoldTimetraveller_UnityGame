using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;       // 要跟随的目标（把Player拖进来）
    public float smoothSpeed = 0.125f; // 跟随的平滑度 (0~1之间，越小越滞后，越大越紧)
    public Vector3 offset;         // 偏移量 (保持相机在Z轴的距离)

    void LateUpdate()
    {
        // 如果没有目标，就不执行，防止报错
        if (target == null) return;

        // 1. 计算目标位置：玩家的位置 + 我们设定的偏移量
        Vector3 desiredPosition = target.position + offset;

        // 2. 平滑移动：使用 Lerp 插值算法，让相机慢慢移动到目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. 应用位置更新
        transform.position = smoothedPosition;
    }
}
