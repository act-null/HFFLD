using UnityEngine;

public class SingleWheelAndCageController : MonoBehaviour
{
    // 定义轮盘
    public Transform ClockRing;

    // 定义笼子
    public Transform Cage;

    // 定义笼子上下移动的边界值，可以在Unity编辑器中调整
    public float upperLimit = -24.0f;
    public float lowerLimit = -34.0f;

    // 用于记录上一帧轮盘的角度，用于判断轮盘是否转动以及转动方向
    private float lastWheelAngle = 0f;

    void Update()
    {
        ProcessWheelAndCage();
    }

    void ProcessWheelAndCage()
    {
        // 获取当前轮盘绕Y轴的角度
        float currentAngle = ClockRing.localEulerAngles.y;

        // 判断轮盘是否转动以及转动方向（通过角度变化来判断）
        bool isRotating = IsWheelRotating(currentAngle, lastWheelAngle);
        int rotationDirection = GetRotationDirection(currentAngle, lastWheelAngle);

        if (isRotating)
        {
            // 根据轮盘转动方向确定笼子移动方向
            int cageDirection = rotationDirection;

            // 获取轮盘绕Y轴的角速度来作为笼子的移动速度（取绝对值，只关注速度大小）
            float moveSpeed = Mathf.Abs(GetRotationSpeed(ClockRing, rotationDirection));

            // 根据当前方向移动笼子
            float newPositionY = Cage.position.y + (moveSpeed * Time.deltaTime * cageDirection);

            // 限制笼子移动在边界内
            newPositionY = Mathf.Clamp(newPositionY, lowerLimit, upperLimit);

            // 更新笼子的位置
            Cage.position = new Vector3(Cage.position.x, newPositionY, Cage.position.z);
        }

        // 更新上一帧轮盘的角度记录
        lastWheelAngle = currentAngle;
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

    int GetRotationDirection(float currentAngle, float lastAngle)
    {
        // 通过角度变化判断轮盘转动方向，顺时针为1，逆时针为 -1
        float angleDiff = currentAngle - lastAngle;
        if (angleDiff > 180f)
        {
            angleDiff -= 360f;
        }
        else if (angleDiff < -180f)
        {
            angleDiff += 360f;
        }
        return angleDiff > 0? 1 : -1;
    }

    float GetRotationSpeed(Transform wheel, int rotationDirection)
    {
        // 获取轮盘对应方向的旋转速度，这里通过角速度的对应分量来获取
        if (rotationDirection == 1)
        {
            return wheel.GetComponent<Rigidbody>().angularVelocity.y;
        }
        else
        {
            return -wheel.GetComponent<Rigidbody>().angularVelocity.y;
        }
    }
}