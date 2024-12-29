using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoldenAppleEventListener : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToActivate;    // 需要激活的目标物体
    [SerializeField]
    private GameObject objectToForce;       // 需要施加力的物体
    [SerializeField]
    private GoldenAppleCollection appleCollection;    // 金苹果收集器引用
    
    [Header("Force Parameters")]
    [SerializeField]
    private int forceCount = 3;        // 施加力的次数
    [SerializeField]
    private float forceInterval = 0.5f; // 每次施加力的时间间隔
    [SerializeField]
    private float forceMagnitude = 5f;  // 力的大小
    [SerializeField]
    private Vector3 forceDirection = Vector3.up; // 力的方向
    
    private bool hasActivated = false;
    private Rigidbody targetRigidbody;

    private void Start()
    {
        // 确保引用不为空
        if (appleCollection != null)
        {
            // 注册事件监听
            appleCollection.onCollectionComplete.AddListener(OnAppleCollectionComplete);
        }
        else
        {
            Debug.LogWarning("GoldenAppleCollection reference not set!");
        }

        // 确保激活物体一开始是隐藏的
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }

        // 获取受力物体的Rigidbody组件
        if (objectToForce != null)
        {
            targetRigidbody = objectToForce.GetComponent<Rigidbody>();
            if (targetRigidbody == null)
            {
                Debug.LogWarning("Force target object does not have a Rigidbody component!");
            }
        }
    }

    private void OnAppleCollectionComplete()
    {
        if (!hasActivated)
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            if (objectToForce != null && targetRigidbody != null)
            {
                StartCoroutine(ApplyForceRoutine());
                Debug.Log("开始施加力！");
            }
            
            hasActivated = true;
        }
    }

    private System.Collections.IEnumerator ApplyForceRoutine()
    {
        int appliedForces = 0;
        while (appliedForces < forceCount)
        {
            targetRigidbody.AddForce(forceDirection.normalized * forceMagnitude, ForceMode.Impulse);
            appliedForces++;
            yield return new WaitForSeconds(forceInterval);
        }
    }

    private void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        if (appleCollection != null)
        {
            appleCollection.onCollectionComplete.RemoveListener(OnAppleCollectionComplete);
        }
    }
}
