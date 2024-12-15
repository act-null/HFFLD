using UnityEngine;

public class RotateAroundY : MonoBehaviour
{
    // 定义旋转速度，可以在Unity编辑器中调整这个值来改变旋转的快慢
    public float rotationSpeed = -15f; 

    void Update()
    {
        // 绕自身的y轴进行旋转，Time.deltaTime用于让旋转速度不受帧率影响，保持相对匀速
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}