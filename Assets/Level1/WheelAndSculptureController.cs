using UnityEngine;

public class WheelAndSculptureController : MonoBehaviour
{
    // 定义四个轮盘
    public Transform ClockRing1;
    public Transform ClockRing2;
    public Transform ClockRing3;
    public Transform ClockRing4;

    // 定义四个雕塑
    public Transform sculpture1;
    public Transform sculpture2;
    public Transform sculpture3;
    public Transform sculpture4;

    // 定义雕塑上下移动的边界值，可以在Unity编辑器中调整
    public float upperLimit = 5.0f;
    public float lowerLimit = -5.0f;

    // 记录雕塑当前移动方向，1表示向上， -1表示向下
    private int[] sculptureDirections = { 1, 1, 1, 1 };

    // 用于记录上一帧轮盘的角度
    private float[] lastWheelAngles = { 0f, 0f, 0f, 0f };

    void Update()
    {
        // 处理ClockRing1与sculpture1的逻辑
        ProcessWheelAndSculpture(ClockRing1, sculpture1, 0);

        // 处理ClockRing2与sculpture2的逻辑
        ProcessWheelAndSculpture(ClockRing2, sculpture2, 1);

        // 处理ClockRing3与sculpture3的逻辑
        ProcessWheelAndSculpture(ClockRing3, sculpture3, 2);

        // 处理ClockRing4与sculpture4的逻辑
        ProcessWheelAndSculpture(ClockRing4, sculpture4, 3);
    }

    void ProcessWheelAndSculpture(Transform wheel, Transform sculpture, int index)
    {
        // 获取当前轮盘绕Y轴的角度
        float currentAngle = wheel.localEulerAngles.y;

        // 判断轮盘是否有转动（对比当前角度和上一帧角度，考虑角度循环情况，进行差值计算）
        bool isWheelRotating = IsWheelRotating(currentAngle, lastWheelAngles[index]);

        if (isWheelRotating)
        {
            // 获取轮盘绕Y轴的角速度来作为雕塑的移动速度（这里取绝对值，只关注速度大小）
            float moveSpeed = Mathf.Abs(wheel.GetComponent<Rigidbody>().angularVelocity.y);
            if (moveSpeed == 0)
            {
                moveSpeed = 0;  // 如果角速度为0，设置一个极小的默认速度，避免雕塑不动，可按需调整
            }

            // 根据当前方向移动雕塑
            float newPositionY = sculpture.position.y + (moveSpeed * Time.deltaTime * sculptureDirections[index]);

            // 检查是否到达边界，如果到达则改变方向
            if (newPositionY >= upperLimit)
            {
                sculptureDirections[index] = -1;
            }
            else if (newPositionY <= lowerLimit)
            {
                sculptureDirections[index] = 1;
            }

            // 更新雕塑的位置
            sculpture.position = new Vector3(sculpture.position.x, newPositionY, sculpture.position.z);

            // 更新上一帧轮盘的角度记录
            lastWheelAngles[index] = currentAngle;
        }
    }

    bool IsWheelRotating(float currentAngle, float lastAngle)
    {
        // 考虑角度循环情况（0 - 360度），计算角度差值
        float angleDiff = Mathf.Abs(currentAngle - lastAngle);
        if (angleDiff > 180f)
        {
            angleDiff = 360f - angleDiff;
        }

        return angleDiff > 0.01f;  // 设置一个极小的阈值，避免因浮点数精度问题误判，可按需调整
    }
}