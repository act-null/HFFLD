using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalSpin : MonoBehaviour
{
    public Transform centerObject;    // 公转中心物体
    public float rotationSpeed = 15f; // 自转速度
    public float revolutionSpeed = 20f;  // 公转速度
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }
    public RotationAxis axis = RotationAxis.X; // 自转轴

    private Vector3 startPosition;
    private float revolutionAngle = 0f;
    private float orbitRadius;      // 存储计算得到的公转半径
    private float heightOffset;     // 存储Y轴高度差值
    private Rigidbody rb;           // Rigidbody 组件
    private float initialAngle;    // 存储初始角度

    void Start()
    {
        // 获取 Rigidbody 组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
            return;
        }

        // 记录初始位置
        startPosition = transform.position;

        // 计算公转半径和高度差值
        if (centerObject != null)
        {
            Vector3 radiusVector = transform.position - centerObject.position;
            orbitRadius = new Vector2(radiusVector.x, radiusVector.z).magnitude;
            heightOffset = radiusVector.y;  // 保存Y轴高度差值
            
            // 计算初始角度
            initialAngle = Mathf.Atan2(radiusVector.z, radiusVector.x) * Mathf.Rad2Deg;
            revolutionAngle = initialAngle; // 设置初始公转角度
        }
    }

    void FixedUpdate()
    {
        // 处理自转
        Vector3 rotationVector = Vector3.zero;
        switch (axis)
        {
            case RotationAxis.X:
                rotationVector = Vector3.right;
                break;
            case RotationAxis.Y:
                rotationVector = Vector3.up;
                break;
            case RotationAxis.Z:
                rotationVector = Vector3.forward;
                break;
        }

        // 使用 Rigidbody 的 MoveRotation 方法进行自转
        Quaternion deltaRotation = Quaternion.Euler(rotationVector * rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // 处理公转
        if (centerObject != null)
        {
            revolutionAngle += revolutionSpeed * Time.fixedDeltaTime;
            float currentAngle = revolutionAngle;
            float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitRadius;
            float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitRadius;
            Vector3 offset = new Vector3(x, heightOffset, z);  // 使用存储的高度差值

            // 使用 Rigidbody 的 MovePosition 方法进行公转
            rb.MovePosition(centerObject.position + offset);
        }
    }
}