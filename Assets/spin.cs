using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour {
    public Transform centerObject;    // 公转中心物体
    public float rotationSpeed = 15f;
    public float revolutionSpeed = 20f;  // 公转速度
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }
    public RotationAxis axis = RotationAxis.X;

    private Vector3 startPosition;
    private float revolutionAngle = 0f;
    private float orbitRadius;      // 存储计算得到的公转半径
    private float heightOffset;     // 存储Y轴高度差值
    private float initialAngle;    // 存储初始角度

    void Start() {
        startPosition = transform.position;
        if (centerObject != null)
        {
            // 计算初始半径
            Vector3 radiusVector = transform.position - centerObject.position;
            orbitRadius = new Vector2(radiusVector.x, radiusVector.z).magnitude;
            heightOffset = radiusVector.y;  // 保存Y轴高度差值
            
            // 计算初始角度
            initialAngle = Mathf.Atan2(radiusVector.z, radiusVector.x) * Mathf.Rad2Deg;
            revolutionAngle = initialAngle; // 设置初始公转角度
        }
    }
    
    void Update() {
        // 处理自转
        Vector3 rotationVector = Vector3.zero;
        switch(axis)
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
        transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);

        // 处理公转
        if (centerObject != null)
        {
            revolutionAngle += revolutionSpeed * Time.deltaTime;
            float currentAngle = revolutionAngle;
            float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitRadius;
            float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitRadius;
            Vector3 offset = new Vector3(x, heightOffset, z);  // 使用存储的高度差值
            transform.position = centerObject.position + offset;
        }
    }
}
