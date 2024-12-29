using UnityEngine;

public class Evelator : MonoBehaviour
{
    [SerializeField] private float forceMultiplier = 10f; // 施加的力的大小
    [SerializeField] private float startDelay = 5f;       // 启动时间（默认5秒）
    [SerializeField] private float cooldownTime = 5f;     // 缓和时间（默认5秒）
    [SerializeField] private float maxY = 50f;            // 最高点

    private bool isPlayerInside = false;    // 玩家是否在电梯内
    private bool isForceActive = false;     // 是否正在施力
    private float startTimer = 0f;         // 启动计时器
    private float cooldownTimer = 0f;      // 缓和计时器
    private Rigidbody rb;                  // 刚体组件

    private void Start()
    {
        // 初始化 Rigidbody 组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the GameObject.");
        }
    }

    private void FixedUpdate()
    {
        if (isPlayerInside && !isForceActive)
        {
            // 启动计时器
            startTimer += Time.fixedDeltaTime;
            if (startTimer >= startDelay)
            {
                isForceActive = true; // 启动施力
                startTimer = 0f;      // 重置计时器
            }
        }
        else if (!isPlayerInside && isForceActive)
        {
            // 缓和计时器
            cooldownTimer += Time.fixedDeltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                isForceActive = false; // 停止施力
                cooldownTimer = 0f;    // 重置计时器
            }
        }

        // 施力逻辑
        if (isForceActive)
        {
            // 如果电梯未达到最高点，继续施力
            if (rb.position.y < maxY)
            {
                // 使用全局向上的方向（Y轴）
                Vector3 globalUp = Vector3.up;

                // 向电梯施加全局向上的力
                rb.AddForce(globalUp * forceMultiplier, ForceMode.Acceleration);
            }
            else
            {
                // 达到最高点，停止施力
                isForceActive = false;

            }
        }
        else
        {

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerInside = true;
            cooldownTimer = 0f; // 重置缓和计时器
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerInside = false;
            startTimer = 0f; // 重置启动计时器
        }
    }
}