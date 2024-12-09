using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    // 用于指定需要移动的物体
    public GameObject movingObject;
    // 用于指定自转的物体
    public GameObject spinningObject;
    // 记录移动物体的初始Z轴坐标
    private float initialZPositionOfMovingObject;
    // 记录自转物体的初始旋转角度（主要用于记录初始状态，目前暂未在核心逻辑中使用）
    private float initialRotationZOfSpinningObject;
    // 定义自转物体的旋转速度，初始为负数，控制物体绕Z轴自旋的快慢，单位是度/秒
    public float rotationSpeed = -30f;
    // 用于记录达到反转条件的时间
    private float reverseConditionTime;
    // 用于标记是否处于停止旋转的5秒时间段内
    private bool isStoppedRotating;

    void Start()
    {
        if (movingObject!= null)
        {
            initialZPositionOfMovingObject = movingObject.transform.position.y;
        }

        if (spinningObject!= null)
        {
            initialRotationZOfSpinningObject = spinningObject.transform.eulerAngles.z;
        }
    }

    void Update()
    {
        if (movingObject!= null && spinningObject!= null)
        {
            float currentZPositionOfMovingObject = movingObject.transform.position.y;

            // 判断是否处于停止旋转的5秒时间段内，如果是，则不进行旋转相关逻辑，直接返回
            if (isStoppedRotating && Time.time - reverseConditionTime < 5f)
            {
                return;
            }
            else
            {
                isStoppedRotating = false;
            }

            // 判断移动物体是否达到上限 +13，若是则自转物体反向旋转，并记录达到反转条件的时间，设置停止旋转标记
            if (rotationSpeed <= 0 && currentZPositionOfMovingObject >= initialZPositionOfMovingObject + 13f)
            {
                rotationSpeed = -rotationSpeed;
                reverseConditionTime = Time.time;
                isStoppedRotating = true;
            }
            // 判断移动物体是否达到下限 +1，若是则自转物体再次反向旋转，并记录达到反转条件的时间，设置停止旋转标记
            else if (rotationSpeed >= 0 && currentZPositionOfMovingObject <= initialZPositionOfMovingObject + 1f)
            {
                rotationSpeed = -rotationSpeed;
                reverseConditionTime = Time.time;
                isStoppedRotating = true;
            }

            // 让自转物体绕Z轴旋转，前提是不在停止旋转的5秒时间段内
            if (!isStoppedRotating)
            {
                spinningObject.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            }
        }
    }
}