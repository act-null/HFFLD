using UnityEngine;

public class RotateAroundX : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotationSpeed = 15f; // 旋转速度（度/秒）

    void Update()
    {
        // 绕自身 X 轴旋转
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}