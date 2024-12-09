using UnityEngine;

public class ApplyForceToSelf : MonoBehaviour
{
    // 定义在z轴负向施加力的大小
    public float forceZ = 10f;
    // 定义在y轴负向施加力的大小
    public float forceY = 10f;

    private Rigidbody rb;

    void Start()
    {
        // 获取物体上挂载的刚体组件，如果没有则添加
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // 在每一帧都施加力（你也可以根据具体需求放在FixedUpdate等其他合适的地方）
        ApplyForces();
    }

    void ApplyForces()
    {
        // 计算z轴负向的力向量
        Vector3 forceZVector = new Vector3(0, 0, -forceZ);
        // 计算y轴负向的力向量
        Vector3 forceYVector = new Vector3(0, -forceY, 0);

        // 将两个方向的力向量相加得到总力向量
        Vector3 totalForce = forceZVector + forceYVector;

        // 对物体自身的刚体施加力，使用ForceMode.Force表示以力的形式施加（还有其他模式可按需选择）
        rb.AddForce(totalForce, ForceMode.Force);
    }
}