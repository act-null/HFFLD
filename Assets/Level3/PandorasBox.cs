using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PandorasBox : MonoBehaviour
{
    [Header("蜡烛配置")]
    public List<GameObject> candlesToCheck = new List<GameObject>(); // 用户指定的蜡烛列表

    [Header("光线配置")]
    public int numberOfRays = 20; // 光线数量
    public float rayMinLength = 5f; // 光线最小长度
    public float rayMaxLength = 15f; // 光线最大长度
    public float rayWidth = 0.1f; // 光线宽度
    public Color rayColor = Color.white; // 光线颜色
    public float rayLifeTime = 3f; // 光线生命周期
    public float raySpeed = 5f; // 光线运动速度

    [Header("特殊配置")]
    public string thornTag = "SpecialThorn"; // 特殊刺的标签
    public List<GameObject> objectsToActivate = new List<GameObject>(); // 需要激活的物体
    public List<GameObject> objectsToColorChange = new List<GameObject>(); // 需要改变颜色的物体
    public Color lightGreenColor = new Color(0.6f, 1f, 0.6f, 1f); // 浅绿色

    [Header("清除区域配置")]
    public Vector3 clearAreaSize = new Vector3(10f, 10f, 10f); // 清除区域的大小

    private Light pointLight; // 点光源
    private bool isTriggered = false; // 是否已触发

    void Start()
    {
        isTriggered = false;
        InitializePointLight(); // 初始化点光源

        // 重置所有蜡烛的状态
        foreach (GameObject candle in candlesToCheck)
        {
            if (candle != null)
            {
                candle.SetActive(false);
            }
        }
    }

    void Update()
    {
        // 检查是否所有蜡烛都被激活
        if (!isTriggered && AreAllCandlesActive())
        {
            TriggerPandorasBox(); // 触发潘多拉魔盒
        }
    }

    /// <summary>
    /// 初始化点光源
    /// </summary>
    void InitializePointLight()
    {
        pointLight = gameObject.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.color = Color.white;
        pointLight.intensity = 0;
        pointLight.range = 10f;
    }

    /// <summary>
    /// 检查是否所有蜡烛都被激活
    /// </summary>
    /// <returns>如果所有蜡烛都被激活，返回 true</returns>
    bool AreAllCandlesActive()
    {
        foreach (GameObject candle in candlesToCheck)
        {
            if (candle == null || !candle.activeInHierarchy)
            {
                return false;
            }
        }
        return candlesToCheck.Count > 0;
    }

    /// <summary>
    /// 触发潘多拉魔盒
    /// </summary>
    void TriggerPandorasBox()
    {
        isTriggered = true;
        StartCoroutine(PandorasBoxSequence()); // 启动潘多拉魔盒序列
    }

    /// <summary>
    /// 潘多拉魔盒序列
    /// </summary>
    IEnumerator PandorasBoxSequence()
    {
        StartCoroutine(FadeInLight()); // 发出白光
        CreateLightRays(); // 生成光线
        ClearSpecialThorns(); // 清除特殊刺
        ActivateSpecifiedObjects(); // 激活指定物体
        ChangeObjectsColor(); // 改变指定物体颜色

        yield return new WaitForSeconds(10f); // 等待 10 秒
        DestroyPandorasBox(); // 销毁潘多拉魔盒
    }

    /// <summary>
    /// 淡入白光
    /// </summary>
    IEnumerator FadeInLight()
    {
        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            pointLight.intensity = Mathf.Lerp(0, 5f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pointLight.intensity = 5f;
    }

    /// <summary>
    /// 生成光线
    /// </summary>
    void CreateLightRays()
    {
        for (int i = 0; i < numberOfRays; i++)
        {
            // 创建一个空物体作为光线容器
            GameObject rayObject = new GameObject("LightRay");
            rayObject.transform.position = transform.position;

            // 添加 LineRenderer 组件
            LineRenderer lineRenderer = rayObject.AddComponent<LineRenderer>();

            // 设置光线的起点和终点
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position); // 起点
            lineRenderer.SetPosition(1, transform.position); // 初始终点（与起点相同）

            // 设置光线的宽度
            lineRenderer.startWidth = rayWidth;
            lineRenderer.endWidth = rayWidth;

            // 设置光线的材质和自发光效果
            lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit")); // 使用自发光材质
            lineRenderer.material.color = rayColor;

            // 随机生成光线的长度
            float randomLength = Random.Range(rayMinLength, rayMaxLength);

            // 启动协程，让光线向外发射
            StartCoroutine(AnimateLightRay(lineRenderer, randomLength, rayLifeTime, raySpeed));
        }
    }

    /// <summary>
    /// 让光线向外发射的协程
    /// </summary>
    /// <param name="lineRenderer">光线的 LineRenderer</param>
    /// <param name="targetLength">光线的目标长度</param>
    /// <param name="lifeTime">光线的生命周期</param>
    /// <param name="speed">光线的运动速度</param>
    IEnumerator AnimateLightRay(LineRenderer lineRenderer, float targetLength, float lifeTime, float speed)
    {
        Vector3 startPosition = lineRenderer.GetPosition(0); // 起点
        Vector3 direction = Random.onUnitSphere; // 随机方向

        float elapsedTime = 0f;
        while (elapsedTime < lifeTime)
        {
            // 计算当前光线的终点位置
            float currentLength = Mathf.Lerp(0, targetLength, elapsedTime / lifeTime);
            Vector3 endPosition = startPosition + direction * currentLength;

            // 更新光线的终点
            lineRenderer.SetPosition(1, endPosition);

            elapsedTime += Time.deltaTime * speed; // 根据速度调整时间
            yield return null;
        }

        // 确保光线达到最大长度
        lineRenderer.SetPosition(1, startPosition + direction * targetLength);

        // 销毁光线对象
        Destroy(lineRenderer.gameObject, 0.1f);
    }

    /// <summary>
    /// 清除指定区域内的特殊刺
    /// </summary>
    void ClearSpecialThorns()
    {
        GameObject[] thorns = GameObject.FindGameObjectsWithTag(thornTag);
        Bounds clearBounds = new Bounds(transform.position, clearAreaSize); // 以潘多拉盒子为中心的清除区域

        foreach (GameObject thorn in thorns)
        {
            // 检查刺是否在指定区域内
            if (clearBounds.Contains(thorn.transform.position))
            {
                Destroy(thorn);
            }
        }
    }

    /// <summary>
    /// 激活指定物体
    /// </summary>
    void ActivateSpecifiedObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 改变指定物体的颜色
    /// </summary>
    void ChangeObjectsColor()
    {
        foreach (GameObject obj in objectsToColorChange)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = lightGreenColor;
                }
            }
        }
    }

    /// <summary>
    /// 销毁潘多拉魔盒
    /// </summary>
    void DestroyPandorasBox()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 在场景中绘制清除区域
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // 设置线框颜色为红色
        Gizmos.DrawWireCube(transform.position, clearAreaSize); // 以潘多拉盒子为中心绘制线框
    }
}