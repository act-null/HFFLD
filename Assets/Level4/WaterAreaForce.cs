using UnityEngine;
using System.Collections.Generic;

public class WaterAreaForce : MonoBehaviour
{
    public Vector2 forceIntervalRange = new Vector2(5f, 10f);
    public Vector2 forceDurationRange = new Vector2(5f, 10f);
    public Vector3 areaSize = new Vector3(32.5f, 25f, 32.5f);
    public float forceMagnitude = 10f;
    public int numberOfWindLines = 50;
    public Vector2 windLineLengthRange = new Vector2(3f, 6f); // 风线长度范围
    public Vector2 windLineSpeedRange = new Vector2(1f, 3f); // 风线速度范围
    public float windLineDisappearTime = 1f;
    private Material windLineMaterial;
    private float nextForceTime;
    private float forceEndTime;
    private Vector3 currentForceDirection;
    private Camera mainCamera;
    private Dictionary<Rigidbody, ForceInfo> affectedObjects = new Dictionary<Rigidbody, ForceInfo>();
    private List<WindLineInfo> windLines = new List<WindLineInfo>();

    private struct WindLineInfo
    {
        public GameObject lineObject;
        public float disappearTime;
        public float speed; // 新增：风线速度
        public float length; // 新增：风线长度
    }

    private struct ForceInfo
    {
        public Vector3 direction;
        public float endTime;
    }

    void Start()
    {
        nextForceTime = Time.time + Random.Range(forceIntervalRange.x, forceIntervalRange.y);
        mainCamera = FindObjectOfType<Camera>();
        CreateWindLineMaterial();
    }

    void Update()
    {
        if (Time.time >= nextForceTime)
        {
            // 随机确定一个力的方向
            int randomAxis = Random.Range(0, 2);
            int randomDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            currentForceDirection = Vector3.zero;
            if (randomAxis == 0)
            {
                currentForceDirection.x = randomDirection;
            }
            else
            {
                currentForceDirection.z = randomDirection;
            }

            // 随机确定力的持续时间
            float forceDuration = Random.Range(forceDurationRange.x, forceDurationRange.y);
            forceEndTime = Time.time + forceDuration;

            // 获取水域区域的边界
            Bounds waterAreaBounds = new Bounds(transform.position, areaSize * 2f);

            // 使用 Physics.OverlapBox 进行检测
            Collider[] colliders = Physics.OverlapBox(transform.position, areaSize, transform.rotation);
            foreach (Collider collider in colliders)
            {
                if ((collider.CompareTag("WindAffectedObject") || collider.CompareTag("Player")) && waterAreaBounds.Contains(collider.bounds.center))
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null && !affectedObjects.ContainsKey(rb)) // 添加检查，确保对象不在字典中
                    {
                        // 将新受到力的物体添加到字典中，并记录相关信息
                        affectedObjects.Add(rb, new ForceInfo { direction = currentForceDirection, endTime = forceEndTime });
                    }
                    // 如果对象已存在，可以选择更新它的信息
                    else if (rb != null)
                    {
                        affectedObjects[rb] = new ForceInfo { direction = currentForceDirection, endTime = forceEndTime };
                    }
                }
            }

            // 创建新的风线
            CreateWindLines();

            // 更新下一次施加力的时间
            nextForceTime = Time.time + Random.Range(forceIntervalRange.x, forceIntervalRange.y);
        }

        // 对所有受力物体施加力，直到力的结束时间
        List<Rigidbody> toRemove = new List<Rigidbody>();
        foreach (KeyValuePair<Rigidbody, ForceInfo> pair in affectedObjects)
        {
            if (Time.time < pair.Value.endTime)
            {
                pair.Key.AddForce(pair.Value.direction * forceMagnitude, ForceMode.Force);
            }
            else
            {
                toRemove.Add(pair.Key);
            }
        }

        foreach (Rigidbody rb in toRemove)
        {
            affectedObjects.Remove(rb);
        }

        // 更新风线位置和透明度
        UpdateWindLines();
    }

    // 创建风线并从相应轴向的区域边界开始
    void CreateWindLines()
    {
        for (int i = 0; i < numberOfWindLines; i++)
        {
            GameObject windLine = new GameObject("WindLine");
            LineRenderer lineRenderer = windLine.AddComponent<LineRenderer>();
            lineRenderer.material = windLineMaterial;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 2;

            // 随机生成风线长度和速度
            float lineLength = Random.Range(windLineLengthRange.x, windLineLengthRange.y);
            float lineSpeed = Random.Range(windLineSpeedRange.x, windLineSpeedRange.y);

            Vector3 startPosition;
            // 根据力的方向确定起始位置
            if (Mathf.Abs(currentForceDirection.x) > 0) // X 轴方向
            {
                startPosition = new Vector3(
                    transform.position.x + (currentForceDirection.x > 0 ? -areaSize.x : areaSize.x),
                    transform.position.y + Random.Range(-areaSize.y, areaSize.y),
                    transform.position.z + Random.Range(-areaSize.z, areaSize.z)
                );
            }
            else // Z 轴方向
            {
                startPosition = new Vector3(
                    transform.position.x + Random.Range(-areaSize.x, areaSize.x),
                    transform.position.y + Random.Range(-areaSize.y, areaSize.y),
                    transform.position.z + (currentForceDirection.z > 0 ? -areaSize.z : areaSize.z)
                );
            }

            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, startPosition + currentForceDirection * lineLength);

            // 将风线信息添加到列表中
            windLines.Add(new WindLineInfo { lineObject = windLine, disappearTime = Time.time + forceDurationRange.y, speed = lineSpeed, length = lineLength });
        }
    }

    // 更新风线位置和透明度
    void UpdateWindLines()
    {
        List<WindLineInfo> toRemove = new List<WindLineInfo>();
        foreach (WindLineInfo windLine in windLines)
        {
            LineRenderer lineRenderer = windLine.lineObject.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                Vector3 startPos = lineRenderer.GetPosition(0);
                Vector3 endPos = lineRenderer.GetPosition(1);

                // 沿力的方向移动风线
                startPos += currentForceDirection * windLine.speed * Time.deltaTime;
                endPos += currentForceDirection * windLine.speed * Time.deltaTime;

                // 如果风线超出区域，则标记为待移除
                if (!new Bounds(transform.position, areaSize * 2f).Contains(endPos))
                {
                    toRemove.Add(windLine);
                }
                else
                {
                  lineRenderer.SetPosition(0, startPos);
                  lineRenderer.SetPosition(1, endPos);

                  // 根据时间调整风线的透明度
                  float alpha = Mathf.Clamp01(1 - (Time.time - (windLine.disappearTime - forceDurationRange.y)) / windLineDisappearTime);
                  lineRenderer.startColor = new Color(1f, 1f, 1f, alpha);
                  lineRenderer.endColor = new Color(1f, 1f, 1f, alpha);
                }
            }
        }

        // 移除已消失的风线
        foreach (WindLineInfo windLine in toRemove)
        {
            windLines.Remove(windLine);
            Destroy(windLine.lineObject);
        }
    }

    // 绘制辅助线框，方便在 Scene 视图中查看区域大小
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, areaSize * 2f);
    }

    // 创建风线材质
    private void CreateWindLineMaterial()
    {
        windLineMaterial = new Material(Shader.Find("Standard"));
        windLineMaterial.color = new Color(1f, 1f, 1f, 0.5f);

        windLineMaterial.SetFloat("_Mode", 2);
        windLineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        windLineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        windLineMaterial.SetInt("_ZWrite", 0);
        windLineMaterial.DisableKeyword("_ALPHATEST_ON");
        windLineMaterial.EnableKeyword("_ALPHABLEND_ON");
        windLineMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        windLineMaterial.renderQueue = 3000;
    }
}
