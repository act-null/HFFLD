using UnityEngine;
using System.Collections.Generic;

public class EggCollectionAndPillarMovement : MonoBehaviour
{
    // 定义巢的Transform组件
    public Transform Nest;
    // 定义四个蛋的Transform组件数组
    public Transform[] Eggs = new Transform[4];
    // 定义判定蛋是否在巢上方空间的距离阈值，可在Unity编辑器中调整
    [SerializeField]
    private float collectionDistanceThreshold = 5f;

    // 定义柱子的集合，用于存储多个柱子的相关信息
    public List<PillarInfo> Pillars = new List<PillarInfo>();

    // 定义初始显示的自定义物品
    public GameObject initialObject;
    // 定义替换后的自定义物品
    public GameObject replacementObject;

    // 新增标志位，用于记录柱子机关是否已经被启动过
    private bool pillarsActivated = false;

    void Update()
    {
        CheckEggsCollected();
        MovePillars();
    }

    void CheckEggsCollected()
    {
        int collectedCount = 0;
        foreach (var egg in Eggs)
        {
            // 计算蛋与巢在xyz轴方向上的距离
            Vector3 distance = new Vector3(egg.position.x - Nest.position.x, egg.position.y - Nest.position.y, egg.position.z - Nest.position.z);
            if (Mathf.Abs(distance.x) <= collectionDistanceThreshold &&
                Mathf.Abs(distance.y) <= collectionDistanceThreshold &&
                Mathf.Abs(distance.z) <= collectionDistanceThreshold)
            {
                collectedCount++;
            }
            if (collectedCount == 4 &&!pillarsActivated)
            {
                ReplaceObject();
                foreach (var pillar in Pillars)
                {
                    pillar.isMoving = true;
                    // 记录柱子原始坐标信息
                    pillar.originalPosition = pillar.transform.position;
                }
                pillarsActivated = true;
            }
        }
    }

    void MovePillars()
    {
        foreach (var pillar in Pillars)
        {
            if (pillar.isMoving)
            {
                float distanceToMove = pillar.moveSpeed * Time.deltaTime * pillar.direction;
                Vector3 newPosition = pillar.transform.position + new Vector3(0, distanceToMove, 0);

                // 判断在Y轴方向上是否达到移动距离（以原柱子坐标 + MoveDistance为标准）
                if ((pillar.direction == 1 && newPosition.y >= pillar.originalPosition.y + pillar.moveDistance) ||
                    (pillar.direction == -1 && newPosition.y <= pillar.originalPosition.y - pillar.moveDistance))
                {
                    pillar.isMoving = false;
                }
                else
                {
                    pillar.transform.position = newPosition;
                }
            }
        }
    }

    void ReplaceObject()
    {
        if (initialObject!= null && replacementObject!= null)
        {
            initialObject.SetActive(false);
            replacementObject.SetActive(true);
        }
    }
}

// 将结构体改为类，并添加Serializable属性
[System.Serializable]
public class PillarInfo
{
    public Transform transform;
    public float moveDistance;
    public float moveSpeed;
    public bool isMoving;
    public int direction;
    // 新增字段，用于记录柱子的原始坐标
    public Vector3 originalPosition;
}