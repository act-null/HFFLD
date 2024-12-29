using UnityEngine;

public class ReflectiveLaser : MonoBehaviour
{
    public float maxDistance = 300f;  // 激光的最大长度
    public LayerMask layerMask;       // 射线检测的层
    public int maxReflections = 5;    // 最大反射次数
    public LineRenderer lineRenderer; // Line Renderer 组件（通过 Inspector 赋值）

    private void Start()
    {
        // 设置 Line Renderer 使用世界坐标
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        // 清空 Line Renderer 的顶点
        lineRenderer.positionCount = 0;

        // 绘制激光及其反射路径
        DrawLaser(transform.position, transform.forward, maxReflections);
    }

    private void DrawLaser(Vector3 startPosition, Vector3 direction, int reflectionsLeft)
    {
        if (reflectionsLeft <= 0) return;

        Ray ray = new Ray(startPosition, direction);
        RaycastHit hit;

        // 添加当前起点到 Line Renderer
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, startPosition);

        // 检测射线碰撞
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            // 添加碰撞点到 Line Renderer
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

            // 如果碰撞到可反射物体（如镜子），计算反射方向并继续绘制激光
            if (hit.collider.CompareTag("Mirror"))
            {
                Vector3 reflectionDirection = Vector3.Reflect(direction, hit.normal);
                reflectionDirection = RoundReflectionDirection(reflectionDirection); // 对反射方向进行 90° 取整
                DrawLaser(hit.point, reflectionDirection, reflectionsLeft - 1);
            }
            // 如果碰撞到目标物体，触发效果
            else if (hit.collider.CompareTag("Barrier"))
            {
                TargetEffect targetEffect = hit.collider.GetComponent<TargetEffect>();
                if (targetEffect != null)
                {
                    targetEffect.TriggerEffect();
                }
            }
        }
        else
        {
            // 如果没有碰撞到物体，添加终点到 Line Renderer
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, startPosition + direction * maxDistance);
        }
    }

    // 对反射方向进行 90° 取整
    private Vector3 RoundReflectionDirection(Vector3 direction)
    {
        // 计算反射方向的角度（以 Y 轴为旋转轴）
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        // 对角度进行 90° 取整
        float roundedAngle = Mathf.Round(angle / 90) * 90;

        // 将角度转换回方向向量
        Vector3 roundedDirection = new Vector3(
            Mathf.Cos(roundedAngle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(roundedAngle * Mathf.Deg2Rad)
        ).normalized;

        return roundedDirection;
    }
}