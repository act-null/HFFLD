using UnityEngine;
using System.Collections.Generic;

public class WaterspoutGenerator : MonoBehaviour
{
    [Header("触发条件")]
    public bool Tridentfound = false;
    public bool Level5achieved = false;

    [Header("水龙卷基本参数")]
    public float topRadius = 10f;
    public float bottomRadius = 2f;
    public float height = 20f;
    public int spiralCount = 12;        // 每层螺旋数量
    public int layerCount = 3;          // 水龙卷层数
    public float layerSpacing = 1f;     // 层间距
    public int pointsPerSpiral = 100;   // 每条螺旋的点数
    public float spiralTurns = 3f;      // 每条螺旋线的旋转圈数
    
    [Header("视觉效果参数")]
    public Color bottomColor = Color.white;
    public Color topColor = Color.blue;
    public float rotationSpeed = 30f;    // 旋转速度
    public float curveOutwardBend = 2f;  // 曲线向外弯曲程度
    public float lineWidth = 0.2f;       // 线条宽度
    public float randomness = 0.5f;      // 随机变化程度
    
    [Header("物理效果")]
    public float attractionForce = 10f;
    public float rotationForce = 5f;
    public float liftForce = 15f;
    public float liftRadius = 5f;

    [Header("渐变可见性参数")]
    public float fadeDuration = 5f; // 渐变可见性的持续时间

    private List<LineRenderer> spirals = new List<LineRenderer>();
    private GameObject liftArea;
    private float[] randomOffsets;
    private float fadeStartTime;

    void OnEnable()
    {
        // 订阅事件
        TridentBehavior.OnTridentFound += HandleTridentFound;
    }

    void OnDisable()
    {
        // 取消订阅事件
        TridentBehavior.OnTridentFound -= HandleTridentFound;
    }

    void HandleTridentFound()
    {
        Tridentfound = true; // 设置 Tridentfound 为 true
        fadeStartTime = Time.time; // 记录渐变开始时间
    }

    void Start()
    {
        InitializeRandomOffsets();
    }

    void InitializeRandomOffsets()
    {
        // 为每层的每个螺旋线生成随机偏移
        randomOffsets = new float[spiralCount * layerCount];
        for (int i = 0; i < randomOffsets.Length; i++)
        {
            randomOffsets[i] = Random.Range(0f, 2f * Mathf.PI);
        }
    }

    void Update()
    {
        if (Tridentfound)
        {
            GenerateWaterspout();
            ApplyPhysics();
        }
        if(Level5achieved)
        {
            DestroyWaterspout();
        }
    }

    void GenerateWaterspout()
    {
        // 确保螺旋线数量正确（总数 = 每层数量 × 层数）
        int totalSpirals = spiralCount * layerCount;
        while (spirals.Count < totalSpirals)
        {
            GameObject spiralObj = new GameObject("Spiral_" + spirals.Count);
            spiralObj.transform.SetParent(transform);
            LineRenderer line = spiralObj.AddComponent<LineRenderer>();
            SetupLineRenderer(line);
            spirals.Add(line);
        }

        // 移除多余的螺旋线
        while (spirals.Count > totalSpirals)
        {
            LineRenderer lastSpiral = spirals[spirals.Count - 1];
            DestroyImmediate(lastSpiral.gameObject);
            spirals.RemoveAt(spirals.Count - 1);
        }

        UpdateSpirals();

        if (liftArea == null)
        {
            CreateLiftArea();
        }
    }

    void SetupLineRenderer(LineRenderer line)
    {
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        line.material = lineMaterial;
        line.widthMultiplier = lineWidth;
        line.positionCount = pointsPerSpiral;
        line.useWorldSpace = true;
    }

    void UpdateSpirals()
    {
        float currentRotation = Time.time * rotationSpeed;
        float fadeProgress = Mathf.Clamp01((Time.time - fadeStartTime) / fadeDuration); // 计算渐变进度

        for (int layer = 0; layer < layerCount; layer++)
        {
            float layerOffset = layer * layerSpacing; // 层偏移

            for (int i = 0; i < spiralCount; i++)
            {
                int spiralIndex = layer * spiralCount + i;
                LineRenderer line = spirals[spiralIndex];
                float spiralOffset = (360f / spiralCount) * i + currentRotation;
                
                Vector3[] points = new Vector3[pointsPerSpiral];
                
                for (int j = 0; j < pointsPerSpiral; j++)
                {
                    float t = j / (float)(pointsPerSpiral - 1);
                    float height_t = t * height;
                    
                    // 计算当前半径
                    float baseRadius = Mathf.Lerp(bottomRadius, topRadius, t);
                    float radius = baseRadius + layerOffset; // 添加层偏移
                    
                    // 添加随机变化
                    float timeOffset = Time.time + randomOffsets[spiralIndex];
                    float radiusVariation = Mathf.Sin(timeOffset * 2f + t * 5f) * randomness;
                    radius += radiusVariation;

                    // 计算螺旋角度
                    float angle = (spiralOffset + t * spiralTurns * 360f) * Mathf.Deg2Rad;
                    
                    // 添加向外弯曲效果
                    float bendOffset = Mathf.Sin(t * Mathf.PI) * curveOutwardBend;
                    radius += bendOffset;

                    // 计算点的位置
                    float x = Mathf.Cos(angle) * radius;
                    float z = Mathf.Sin(angle) * radius;
                    
                    points[j] = transform.position + new Vector3(x, height_t, z);
                }

                // 更新线条位置
                line.SetPositions(points);

                // 设置颜色渐变
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { 
                        new GradientColorKey(bottomColor, 0.0f), 
                        new GradientColorKey(topColor, 1.0f) 
                    },
                    new GradientAlphaKey[] { 
                        new GradientAlphaKey(fadeProgress, 0.0f),  // 从下到上逐渐透明
                        new GradientAlphaKey(1.0f, 1.0f)          // 顶部完全不透明
                    }
                );
                line.colorGradient = gradient;
            }
        }
    }

    void CreateLiftArea()
    {
        liftArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        liftArea.transform.SetParent(transform);
        liftArea.transform.localPosition = Vector3.zero;
        liftArea.transform.localScale = new Vector3(liftRadius * 2, 0.1f, liftRadius * 2);
        
        Material liftAreaMaterial = new Material(Shader.Find("Standard"));
        liftAreaMaterial.color = new Color(1f, 1f, 0f, 0.3f);
        liftArea.GetComponent<Renderer>().material = liftAreaMaterial;
    }

    void ApplyPhysics()
    {
        Collider[] affectedObjects = Physics.OverlapCapsule(
            transform.position,
            transform.position + Vector3.up * height,
            topRadius,
            LayerMask.GetMask("WindAffectedObject", "Player")
        );

        foreach (Collider col in affectedObjects)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 应用吸引力
                Vector3 attractionDirection = (transform.position - col.transform.position).normalized;
                rb.AddForce(attractionDirection * attractionForce);

                // 应用旋转力
                Vector3 tangent = Vector3.Cross(Vector3.up, attractionDirection).normalized;
                rb.AddForce(tangent * rotationForce);

                // 应用上升力
                float distanceToCenter = Vector3.Distance(
                    new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z),
                    transform.position
                );
                if (distanceToCenter <= liftRadius)
                {
                    rb.AddForce(Vector3.up * liftForce);
                }
            }
        }
    }

    void DestroyWaterspout()
    {
        foreach (LineRenderer spiral in spirals)
        {
            if (spiral != null)
            {
                DestroyImmediate(spiral.gameObject);
            }
        }
        spirals.Clear();

        if (liftArea != null)
        {
            DestroyImmediate(liftArea);
            liftArea = null;
        }
    }

    void OnDrawGizmos()
    {
        // 绘制水龙卷轮廓
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, bottomRadius);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * height, topRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * height);

        // 绘制上升力作用区域
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, liftRadius);
    }
}