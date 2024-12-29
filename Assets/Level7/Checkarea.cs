using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkarea : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;        // 需要被移动的物体
    [SerializeField]
    private float checkRadius = 1.0f;      // 检测区域半径
    
    private bool hasTriggered = false;     // 确保只触发一次

    private void Update()
    {
        if (!hasTriggered)
        {
            // 检测球形区域内的所有碰撞体
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);
            
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    if (targetObject != null)
                    {
                        targetObject.transform.position = transform.position;
                        Debug.Log("物体已移动到指定位置");
                        hasTriggered = true;
                        break;
                    }
                    else
                    {
                        Debug.LogWarning("目标物体未设置！");
                    }
                }
            }
        }
    }

    // 在场景视图中绘制检测区域
    private void OnDrawGizmos()
    {
        // 设置Gizmos颜色
        Gizmos.color = hasTriggered ? Color.gray : Color.green;
        // 绘制球形检测区域
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
