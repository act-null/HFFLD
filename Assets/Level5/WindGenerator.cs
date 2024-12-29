using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 风场生成器组件
/// </summary>
public class WindGenerator : MonoBehaviour
{
    /// <summary>
    /// 风场基础设置
    /// </summary>
    [System.Serializable]
    public class WindFieldSettings
    {
        [Header("风场参数")]
        public float cylinderRadius = 5f;    // 风场圆柱体半径
        public float cylinderHeight = 10f;   // 风场圆柱体高度
        public float windForce = 10f;        // 风力大小
        public Color visualizationColor = new Color(0.5f, 0.7f, 1f, 0.3f); // 可视化颜色
    }

    /// <summary>
    /// 风线效果设置
    /// </summary>
    [System.Serializable]
    public class WindLineSettings
    {
        [Header("风线参数")]
        [Range(5, 15)]
        public int minLineCount = 5;                        // 最小风线数量
        [Range(5, 15)]
        public int maxLineCount = 15;                       // 最大风线数量
        public float minLength = 1f;                        // 最小长度
        public float maxLength = 5f;                        // 最大长度
        public float minSpeed = 1f;                         // 最小速度
        public float maxSpeed = 3f;                         // 最大速度
        public float resetHeight = 0f;                      // 重置高度
        public float maxHeight = 10f;                       // 最大高度
        public float lineWidth = 0.1f;                      // 风线宽度
        public Color lineColor = new Color(0.5f, 0.7f, 1f, 0.5f); // 风线颜色
    }


    [Header("Event Settings")]
    public AudioTriggerZone[] triggerObjects; // 触发对象数组
    private int triggeredCount = 0; // 记录触发的事件数量

    [SerializeField] 
    private WindFieldSettings windFieldSettings;
    [SerializeField] 
    private WindLineSettings windLineSettings;

    private bool isActive;
    private List<GameObject> activeWindLines;
    private Material windLineMaterial;

    #region Unity生命周期方法

    private void Awake()
    {
        activeWindLines = new List<GameObject>();
        CreateWindLineMaterial();
    }

    private void Start()
    {
        // 注册事件监听器
        if (triggerObjects != null)
        {
            foreach (var triggerObject in triggerObjects)
            {
                if (triggerObject != null)
                {
                    triggerObject.onTriggerEvent.AddListener(OnTriggerActivated);
                }
            }
        }
    }

    /// <summary>
    /// 触发事件的回调方法
    /// </summary>
    private void OnTriggerActivated()
    {
        triggeredCount++;
        CheckAndActivateWindField();
    }

    /// <summary>
    /// 检查是否所有触发器都已激活，如果是则激活风场
    /// </summary>
    private void CheckAndActivateWindField()
    {
        if (triggerObjects != null && triggeredCount >= triggerObjects.Length)
        {
            ActivateWindField();
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 激活风场
    /// </summary>
    public void ActivateWindField()
    {
        if (!isActive)
        {
            isActive = true;
            GenerateWindLines();
        }
    }

    /// <summary>
    /// 停用风场
    /// </summary>
    public void DeactivateWindField()
    {
        if (isActive)
        {
            isActive = false;
            CleanupWindLines();
        }
    }

    /// <summary>
    /// 创建风线材质
    /// </summary>
    private void CreateWindLineMaterial()
    {
        // 创建一个新的材质
        windLineMaterial = new Material(Shader.Find("Standard"));
        windLineMaterial.SetFloat("_Mode", 3); // 设置为透明模式
        windLineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        windLineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        windLineMaterial.SetInt("_ZWrite", 0);
        windLineMaterial.DisableKeyword("_ALPHATEST_ON");
        windLineMaterial.EnableKeyword("_ALPHABLEND_ON");
        windLineMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        windLineMaterial.renderQueue = 3000;
        
        // 设置发光
        windLineMaterial.EnableKeyword("_EMISSION");
        windLineMaterial.SetColor("_EmissionColor", windLineSettings.lineColor * 0.5f);
        
        // 设置基础颜色
        windLineMaterial.SetColor("_Color", windLineSettings.lineColor);
    }

    /// <summary>
    /// 生成风线效果
    /// </summary>
    private void GenerateWindLines()
    {
        CleanupWindLines();

        // 随机生成风线数量
        int windLineCount = Random.Range(windLineSettings.minLineCount, windLineSettings.maxLineCount + 1);

        for (int i = 0; i < windLineCount; i++)
        {
            CreateWindLine();
        }
    }

    /// <summary>
    /// 创建风线模型
    /// </summary>
    private GameObject CreateWindLineModel(float length)
    {
        // 创建游戏对象
        GameObject windLine = new GameObject("WindLine");
        
        // 创建圆柱体网格
        MeshFilter meshFilter = windLine.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = windLine.AddComponent<MeshRenderer>();
        
        // 生成圆柱体网格
        Mesh mesh = CreateCylinderMesh(length, windLineSettings.lineWidth);
        meshFilter.mesh = mesh;
        
        // 设置材质
        meshRenderer.material = windLineMaterial;
        
        return windLine;
    }

    /// <summary>
    /// 创建圆柱体网格
    /// </summary>
    private Mesh CreateCylinderMesh(float length, float width)
    {
        Mesh mesh = new Mesh();
        
        int segments = 8; // 圆柱体段数
        float radius = width * 0.5f;
        
        // 计算顶点数量
        int numVertices = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];
        
        // 生成顶点
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            
            vertices[i] = new Vector3(x, 0, z);
            vertices[i + segments + 1] = new Vector3(x, length, z);
            
            uv[i] = new Vector2((float)i / segments, 0);
            uv[i + segments + 1] = new Vector2((float)i / segments, 1);
        }
        
        // 计算三角形数量
        int numTriangles = segments * 6;
        int[] triangles = new int[numTriangles];
        
        // 生成三角形
        int index = 0;
        for (int i = 0; i < segments; i++)
        {
            triangles[index++] = i;
            triangles[index++] = i + segments + 1;
            triangles[index++] = i + 1;
            
            triangles[index++] = i + 1;
            triangles[index++] = i + segments + 1;
            triangles[index++] = i + segments + 2;
        }
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    /// <summary>
    /// 创建单个风线
    /// </summary>
    private void CreateWindLine()
    {
        // 在圆形范围内随机生成位置
        Vector2 randomCircle = Random.insideUnitCircle * windFieldSettings.cylinderRadius;
        Vector3 position = transform.position + new Vector3(randomCircle.x, windLineSettings.resetHeight, randomCircle.y);

        // 随机设置风线参数
        float length = Random.Range(windLineSettings.minLength, windLineSettings.maxLength);
        float speed = Random.Range(windLineSettings.minSpeed, windLineSettings.maxSpeed);

        // 创建风线模型
        GameObject windLine = CreateWindLineModel(length);
        windLine.transform.position = position;
        windLine.transform.SetParent(transform);
        
        // 添加风线组件
        WindLine windLineComponent = windLine.AddComponent<WindLine>();
        windLineComponent.Initialize(length, speed, windLineSettings.resetHeight, windLineSettings.maxHeight);

        activeWindLines.Add(windLine);
    }

    private void Update()
    {
        if (isActive)
        {
            UpdateWindLines();
            ApplyWindForceToPlayers();
        }
    }

    private void UpdateWindLines()
    {
        if (activeWindLines == null || activeWindLines.Count == 0)
            return;

        for (int i = activeWindLines.Count - 1; i >= 0; i--)
        {
            if (activeWindLines[i] == null)
            {
                activeWindLines.RemoveAt(i);
                continue;
            }

            WindLine windLine = activeWindLines[i].GetComponent<WindLine>();
            if (windLine != null)
            {
                windLine.UpdatePosition();
            }
        }
    }

    private void ApplyWindForceToPlayers()
    {
        // 检测风场范围内的玩家
        Collider[] hitColliders = Physics.OverlapCapsule(
            transform.position,
            transform.position + Vector3.up * windFieldSettings.cylinderHeight,
            windFieldSettings.cylinderRadius,
            LayerMask.GetMask("Player")
        );

        // 输出检测到的碰撞器数量，用于调试
        if (hitColliders.Length > 0)
        {
            Debug.Log($"检测到 {hitColliders.Length} 个玩家在风场范围内");
        }

        // 对每个玩家施加向上的力
        foreach (Collider hitCollider in hitColliders)
        {
            Rigidbody playerRb = hitCollider.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // 添加调试日志
                Debug.Log($"对玩家 {hitCollider.name} 施加向上力: {windFieldSettings.windForce}");
                playerRb.AddForce(Vector3.up * windFieldSettings.windForce, ForceMode.Force);
            }
        }
    }

    /// <summary>
    /// 清理风线效果
    /// </summary>
    private void CleanupWindLines()
    {
        if (activeWindLines != null)
        {
            foreach (GameObject line in activeWindLines)
            {
                if (line != null)
                {
                    Destroy(line);
                }
            }
            activeWindLines.Clear();
        }
    }

    /// <summary>
    /// 绘制风场可视化效果
    /// </summary>
    private void DrawWindFieldGizmos()
    {
        Gizmos.color = windFieldSettings.visualizationColor;
        
        // 绘制圆柱体
        float radius = windFieldSettings.cylinderRadius;
        float height = windFieldSettings.cylinderHeight;
        Vector3 center = transform.position + new Vector3(0, height/2, 0);
        
        // 绘制顶部和底部圆形
        DrawCircle(transform.position, radius);
        DrawCircle(transform.position + Vector3.up * height, radius);
        
        // 绘制连接线
        for (int i = 0; i < 4; i++)
        {
            float angle = i * Mathf.PI / 2;
            Vector3 bottomPoint = transform.position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Vector3 topPoint = bottomPoint + Vector3.up * height;
            Gizmos.DrawLine(bottomPoint, topPoint);
        }
    }

    /// <summary>
    /// 绘制圆形
    /// </summary>
    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32;
        float angle = 0f;
        float angleStep = 2f * Mathf.PI / segments;
        Vector3 previousPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

        for (int i = 0; i <= segments; i++)
        {
            angle += angleStep;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

    #endregion
}

/// <summary>
/// 风线效果组件
/// </summary>
public class WindLine : MonoBehaviour
{
    private float length;           // 风线长度
    private float speed;            // 上升速度
    private float resetHeight;      // 重置高度
    private float maxHeight;        // 最大高度

    /// <summary>
    /// 初始化风线参数
    /// </summary>
    public void Initialize(float lineLength, float moveSpeed, float minHeight, float heightLimit)
    {
        length = lineLength;
        speed = moveSpeed;
        resetHeight = minHeight;
        maxHeight = heightLimit;

        // 随机旋转
        transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

    /// <summary>
    /// 更新风线位置
    /// </summary>
    public void UpdatePosition()
    {
        // 向上移动
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);

        // 检查是否超过最大高度
        if (transform.position.y >= maxHeight)
        {
            // 重置到底部
            Vector3 position = transform.position;
            position.y = resetHeight;
            transform.position = position;
        }
    }
}