using UnityEngine;
using System.Collections;
using System;

public class TridentBehavior : MonoBehaviour
{
    public float detectionRadius = 5f; // 用户自定义的检测半径
    public Color lightColor = new Color(0, 1, 1, 1); // 青蓝色
    public float lightIntensity = 10f; // 点光源的最大强度
    public float glowDuration = 5f; // 发光持续时间

    public GameObject targetObject; // 用户指定的需要上升的物体
    public float riseHeight = 60f; // 上升的高度
    public float riseDuration = 5f; // 上升的持续时间

    public float fallDistance = 20f; // 下落的距离
    public float fallDuration = 5f; // 下落的持续时间

    private bool isGlowing = false;
    private float glowStartTime;
    private Light pointLight; // 点光源组件
    private bool isFalling = false;

    // 定义事件
    public static event Action OnTridentFound;

    void Start()
    {
        // 初始化点光源
        pointLight = CreatePointLight();
        pointLight.enabled = false; // 默认关闭点光源
    }

    void Update()
    {
        // 检测是否有标签为“Player”的物体进入检测范围
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        bool playerDetected = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                playerDetected = true;
                break;
            }
        }

        // 如果检测到玩家并且还没有开始发光
        if (playerDetected && !isGlowing)
        {
            StartGlow();
            OnTridentFound?.Invoke(); // 触发事件
            StartCoroutine(RiseObject()); // 启动物体上升的协程
            StartCoroutine(FallTrident()); // 启动三叉戟下落的协程
        }

        // 如果正在发光，则更新发光效果
        if (isGlowing)
        {
            UpdateGlow();
        }
    }

    void StartGlow()
    {
        isGlowing = true;
        glowStartTime = Time.time;
        pointLight.enabled = true; // 启用点光源
    }

    void UpdateGlow()
    {
        float elapsedTime = Time.time - glowStartTime;

        if (elapsedTime >= glowDuration)
        {
            // 发光结束，销毁三叉戟物体和点光源
            Destroy(pointLight.gameObject);
            Destroy(gameObject);
        }
        else
        {
            // 计算当前的光强度
            float lightIntensityProgress = Mathf.Lerp(0, lightIntensity, elapsedTime / glowDuration);
            pointLight.intensity = lightIntensityProgress;
        }
    }

    IEnumerator RiseObject()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("No target object assigned for rising!");
            yield break;
        }

        Vector3 startPosition = targetObject.transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, riseHeight, 0);
        float elapsedTime = 0f;

        while (elapsedTime < riseDuration)
        {
            elapsedTime += Time.deltaTime;

            // 使用 Mathf.SmoothStep 实现缓出效果
            float t = Mathf.SmoothStep(0, 1, elapsedTime / riseDuration);
            targetObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // 确保物体最终到达目标位置
        targetObject.transform.position = targetPosition;
    }

    IEnumerator FallTrident()
    {
        isFalling = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition - new Vector3(0, fallDistance, 0);
        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            // 使用平滑插值实现仿重力下落
            float t = Mathf.SmoothStep(0, 1, elapsedTime / fallDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // 确保三叉戟最终到达目标位置
        transform.position = targetPosition;
        isFalling = false;
    }

    Light CreatePointLight()
    {
        // 创建一个新的点光源
        GameObject lightObject = new GameObject("Trident Point Light");
        lightObject.transform.position = transform.position;
        lightObject.transform.parent = transform; // 将点光源附加到三叉戟物体上

        Light lightComponent = lightObject.AddComponent<Light>();
        lightComponent.type = LightType.Point;
        lightComponent.color = lightColor;
        lightComponent.intensity = 0f; // 初始强度为 0
        lightComponent.range = 10f; // 设置点光源的范围

        return lightComponent;
    }

    void OnDrawGizmosSelected()
    {
        // 在编辑器中绘制检测范围的球体
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}