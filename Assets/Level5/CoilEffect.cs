using UnityEngine;

public class CoilEffect : MonoBehaviour
{
    // 用户指定的中心物体
    public Transform cylinderCenterObject;

    // 圆柱形区域的半径和高度
    public float cylinderRadius = 5f; // 圆柱形区域的半径
    public float cylinderHeight = 10f; // 圆柱形区域的高度

    // 用于存储上一次记录的位置
    private Vector3 lastRecordedPosition;

    // 用于存储生成的白线的材质
    private Material lineMaterial;

    void Start()
    {
        // 检查是否指定了中心物体
        if (cylinderCenterObject == null)
        {
            Debug.LogError("请在 Inspector 中指定一个中心物体！");
            enabled = false; // 禁用脚本
            return;
        }

        // 初始化上一次记录的位置为当前位置
        lastRecordedPosition = transform.position;

        // 创建一个新的材质，并设置其属性为白色自发光
        lineMaterial = new Material(Shader.Find("Standard"));
        lineMaterial.SetColor("_EmissionColor", Color.white);
        lineMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        // 获取中心物体的位置（固定位置，不再动态获取）
        Vector3 cylinderCenter = cylinderCenterObject.position;

        // 检查物体是否在圆柱形区域内
        if (IsInsideCylinder(transform.position, cylinderCenter))
        {
            // 计算当前位置与上一次记录位置之间的距离
            float distanceMoved = Vector3.Distance(transform.position, lastRecordedPosition);

            // 如果移动距离达到1米
            if (distanceMoved >= 1f)
            {
                // 记录当前位置
                Vector3 currentPosition = transform.position;

                // 在当前位置和上一次记录位置之间绘制白线
                SpawnWhiteLine(lastRecordedPosition, currentPosition);

                // 更新上一次记录的位置
                lastRecordedPosition = currentPosition;
            }
        }
    }

    // 检查点是否在圆柱形区域内
    bool IsInsideCylinder(Vector3 point, Vector3 cylinderCenter)
    {
        // 计算点到圆柱中心的水平距离
        float horizontalDistance = Vector2.Distance(new Vector2(point.x, point.z), new Vector2(cylinderCenter.x, cylinderCenter.z));

        // 检查水平距离是否在半径范围内，且高度是否在圆柱高度范围内
        return horizontalDistance <= cylinderRadius && Mathf.Abs(point.y - cylinderCenter.y) <= cylinderHeight / 2f;
    }

    // 在两个点之间绘制白线
    void SpawnWhiteLine(Vector3 startPoint, Vector3 endPoint)
    {
        // 创建一个新的游戏对象来表示白线
        GameObject lineObject = new GameObject("WhiteLine");
        lineObject.transform.position = startPoint;

        // 添加LineRenderer组件
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // 设置LineRenderer的材质
        lineRenderer.material = lineMaterial;

        // 设置LineRenderer的属性
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        // 设置LineRenderer的顶点
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    // 在 Unity 编辑器中绘制圆柱形区域的可视化
    private void OnDrawGizmos()
    {
        if (cylinderCenterObject == null)
            return;

        // 获取中心物体的位置
        Vector3 cylinderCenter = cylinderCenterObject.position;

        // 设置 Gizmos 的颜色为半透明蓝色
        Gizmos.color = new Color(0, 0.5f, 1f, 0.5f);

        // 绘制圆柱形的顶部和底部圆
        DrawCircle(cylinderCenter + Vector3.up * (cylinderHeight / 2), cylinderRadius);
        DrawCircle(cylinderCenter + Vector3.down * (cylinderHeight / 2), cylinderRadius);

        // 绘制圆柱形的侧面
        for (float y = 0; y <= cylinderHeight; y += cylinderHeight / 10)
        {
            Vector3 start = cylinderCenter + Vector3.up * (y - cylinderHeight / 2);
            Vector3 end = cylinderCenter + Vector3.up * (y + cylinderHeight / 10 - cylinderHeight / 2);
            DrawCircle(start, cylinderRadius);
            DrawCircle(end, cylinderRadius);
        }
    }

    // 绘制圆形
    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32; // 圆的线段数
        float angleStep = 360f / segments;
        Vector3 previousPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 point = center + new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
                0,
                Mathf.Sin(Mathf.Deg2Rad * angle) * radius
            );

            if (i > 0)
            {
                Gizmos.DrawLine(previousPoint, point);
            }

            previousPoint = point;
        }
    }
}