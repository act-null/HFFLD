using UnityEngine;
using UnityEngine.Events;

public class EventListenerAndSpawner : MonoBehaviour
{
    public DisableKinematicOnCollision[] lockObjects; // 引用DisableKinematicOnCollision脚本的对象数组
    public GameObject prefabToSpawn;    // 用户指定的预制体

    private int triggeredCount = 0; // 记录触发的事件数量

    private void Start()
    {
        // 为每个DisableKinematicOnCollision对象注册事件监听器
        foreach (var lockObject in lockObjects)
        {
            if (lockObject != null)
            {
                lockObject.onLockBroken.AddListener(OnLockBroken);
            }
        }
    }

    public void OnLockBroken()
    {
        triggeredCount++;
        CheckAndSpawn();
    }

    private void CheckAndSpawn()
    {
        // 检查是否所有的锁都被触发了
        if (triggeredCount >= lockObjects.Length)
        {
            // 在空物体的中心生成预制体
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }
}
