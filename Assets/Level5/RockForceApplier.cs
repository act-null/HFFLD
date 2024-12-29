using UnityEngine;

public class RockForceApplier : MonoBehaviour
{
    // 用户可以在Inspector中设置此值来调整施加给岩石的力的比例。
    public float forceMultiplier = 1.0f;
    // 用户可以选择是否根据距离的倒数施加力或使用固定数值。
    public bool useInverseDistanceForce = true;
    // 如果选择了固定数值，则这是用户设定的力大小。
    public float fixedForceMagnitude = 5.0f;
    // 球形区域的半径。
    public float sphereRadius = 5.0f;

    // 玩家标签。
    private const string PLAYER_TAG = "Player";

    // 可视化球形区域的组件。
    private SphereCollider sphereCollider;
    // 标记玩家是否在球形区域内。
    private bool playerIsInSphere = false;
    // 缓存玩家对象以减少每帧查找的开销。
    private Transform playerTransform;

    void Start()
    {
        // 创建并配置SphereCollider用于可视化和检测玩家进入。
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = sphereRadius;
        sphereCollider.isTrigger = true; // 设置为触发器。
        sphereCollider.center = Vector3.zero; // 中心在岩石中心。
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查碰撞对象是否为标记为"Player"的对象。
        if (other.CompareTag(PLAYER_TAG))
        {
            playerIsInSphere = true;
            playerTransform = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 检查离开的碰撞对象是否为标记为"Player"的对象。
        if (other.CompareTag(PLAYER_TAG))
        {
            playerIsInSphere = false;
            playerTransform = null;
        }
    }

    void FixedUpdate()
    {
        if (playerIsInSphere && playerTransform != null)
        {
            ApplyForceToRock(playerTransform);
        }
    }

    void ApplyForceToRock(Transform playerTransform)
    {
        // 获取从玩家指向石头的向量（修改后的方向）
        Vector3 directionFromPlayer = transform.position - playerTransform.position;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (useInverseDistanceForce)
            {
                float distance = directionFromPlayer.magnitude;
                float minDistance = 0.1f;
                if (distance < minDistance)
                {
                    distance = minDistance;
                }

                // 根据距离的倒数计算力的大小，方向为从玩家指向石头
                float forceMagnitude = forceMultiplier / distance;
                Vector3 forceToApply = directionFromPlayer.normalized * forceMagnitude;
                rb.AddForce(forceToApply, ForceMode.Force);
            }
            else
            {
                // 使用固定力，方向从玩家指向石头
                Vector3 forceToApply = directionFromPlayer.normalized * fixedForceMagnitude;
                rb.AddForce(forceToApply, ForceMode.Force);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 在编辑器中可视化球形区域。
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}