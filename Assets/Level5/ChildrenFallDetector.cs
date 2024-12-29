using UnityEngine;
using System.Collections.Generic;

public class ChildrenFallDetector : MonoBehaviour {
    
    [Header("检测区域设置")]
    [Tooltip("检测区域的中心位置（相对于物体）")]
    public Vector3 detectionAreaCenter = Vector3.zero;
    
    [Tooltip("检测区域的大小")]
    public Vector3 detectionAreaSize = new Vector3(10f, 2f, 10f);

    [Header("可视化设置")]
    [Tooltip("是否显示检测区域")]
    public bool showDebugArea = true;

    [Tooltip("检测区域的颜色")]
    public Color debugAreaColor = new Color(1f, 0f, 0f, 0.3f);

    private Dictionary<Transform, float> childInitialHeights = new Dictionary<Transform, float>();

    void Start() {
        // 记录所有子物体的初始高度
        foreach(Transform child in transform) {
            childInitialHeights[child] = child.position.y;
        }
    }

    void Update() {
        // 获取检测区域的世界空间边界
        Vector3 worldCenter = transform.TransformPoint(detectionAreaCenter);
        Vector3 min = worldCenter - detectionAreaSize * 0.5f;
        Vector3 max = worldCenter + detectionAreaSize * 0.5f;

        // 检查每个子物体
        foreach(Transform child in transform) {
            if (childInitialHeights.ContainsKey(child)) {
                Vector3 childPos = child.position;
               
                // 检查是否在检测区域内
                if (childPos.x >= min.x && childPos.x <= max.x &&
                    childPos.y >= min.y && childPos.y <= max.y &&
                    childPos.z >= min.z && childPos.z <= max.z) {
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    void OnDrawGizmos() {
        if (showDebugArea) {
            // 保存当前Gizmos颜色
            Color originalColor = Gizmos.color;
            Matrix4x4 originalMatrix = Gizmos.matrix;

            // 设置新颜色并绘制区域
            Gizmos.color = debugAreaColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            // 绘制检测区域
            Gizmos.DrawCube(detectionAreaCenter, detectionAreaSize);
            
            // 绘制线框
            Gizmos.color = new Color(debugAreaColor.r, debugAreaColor.g, debugAreaColor.b, 1f);
            Gizmos.DrawWireCube(detectionAreaCenter, detectionAreaSize);

            // 恢复原始设置
            Gizmos.color = originalColor;
            Gizmos.matrix = originalMatrix;
        }
    }
}
