using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoldenAppleCollection : MonoBehaviour
{
    // 定义收集目标点
    public Transform collectionPoint;
    // 定义需要收集的金苹果数组
    public Transform[] goldenApples;
    // 定义收集距离阈值
    [SerializeField]
    private float collectionDistanceThreshold = 5f;
    // 定义收集完成事件
    public UnityEvent onCollectionComplete;
    // 防止事件重复触发
    private bool isCollectionComplete = false;

    void Update()
    {
        if (!isCollectionComplete)
        {
            CheckApplesCollected();
        }
    }

    void CheckApplesCollected()
    {
        int collectedCount = 0;
        
        foreach (var apple in goldenApples)
        {
            if (apple != null)
            {
                // 计算苹果与收集点的距离
                Vector3 distance = apple.position - collectionPoint.position;
                if (Mathf.Abs(distance.x) <= collectionDistanceThreshold &&
                    Mathf.Abs(distance.y) <= collectionDistanceThreshold &&
                    Mathf.Abs(distance.z) <= collectionDistanceThreshold)
                {
                    collectedCount++;
                }
            }
        }

        // 检查是否所有金苹果都已收集
        if (collectedCount == goldenApples.Length && !isCollectionComplete)
        {
            isCollectionComplete = true;
            // 触发收集完成事件
            onCollectionComplete?.Invoke();
            Debug.Log("所有金苹果已收集完成！");
        }
    }
}
