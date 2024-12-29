using UnityEngine;
using UnityEngine.Events;

public class DisableKinematicOnCollision : MonoBehaviour
{
    public GameObject[] pairObjects; // 一对物体，包含所有需要取消 isKinematic 的物体
    public GameObject targetObject;  // 指定的物体，只有碰撞到它时才会触发效果
    public UnityEvent onLockBroken;  // 自定义事件：lockbroken

    public bool hasTriggered = false; // 标记是否已经触发过

    private void OnCollisionEnter(Collision collision)
    {
        // 检查是否已经触发过，并且碰撞到的物体是否是目标物体
        if (!hasTriggered && collision.gameObject == targetObject)
        {
            // 遍历所有物体，检查它们的 isKinematic 属性是否为 true
            foreach (GameObject obj in pairObjects)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && rb.isKinematic) // 仅当 isKinematic 为 true 时才取消
                {
                    rb.isKinematic = false;
                }
            }

            // 触发 lockbroken 事件
            onLockBroken.Invoke();

            // 标记已经触发过
            hasTriggered = true;
        }
    }
}