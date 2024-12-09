using UnityEngine;

public class ZAxisLoopMovement : MonoBehaviour
{
    // 定义运动速度，控制循环运动的快慢
    public float speed = 1f;
    // 定义在Z轴上运动的范围边界，这里设置为 +-3
    public float zAxisRange = 3f;
    // 用于记录物体初始的Z轴坐标（原始坐标）
    private float initialZPosition;
    // 用于记录当前物体在Z轴上的位置
    private float currentZPosition;

    void Start()
    {
        initialZPosition = transform.position.z;
        currentZPosition = initialZPosition;
    }

    void Update()
    {
        // 根据时间和速度更新物体在Z轴上的位置
        currentZPosition += speed * Time.deltaTime;

        // 当超过正向边界（原始坐标 + 3）时，改变运动方向
        if (currentZPosition > initialZPosition + zAxisRange)
        {
            speed = -Mathf.Abs(speed);
        }
        // 当超过负向边界（原始坐标 - 3）时，改变运动方向
        else if (currentZPosition < initialZPosition - zAxisRange)
        {
            speed = Mathf.Abs(speed);
        }

        // 更新物体的Z轴位置
        Vector3 newPosition = transform.position;
        newPosition.z = currentZPosition;
        transform.position = newPosition;
    }
}