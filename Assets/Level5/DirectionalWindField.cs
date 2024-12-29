using UnityEngine;
using System.Collections.Generic;

public class DirectionalWindField : MonoBehaviour {
    [System.Serializable]
    public class WindFieldSettings {
        [Header("风场参数")]
        public float fieldLength = 10f;      // 风场长度
        public float fieldWidth = 5f;        // 风场宽度
        public float fieldHeight = 5f;       // 风场高度
        public float windForce = 10f;        // 风力大小
        public string targetTag = "Player";   // 目标标签
        public Color visualizationColor = new Color(0.5f, 0.7f, 1f, 0.3f);
    }

    [System.Serializable]
    public class WindLineSettings {
        [Header("风线参数")]
        [Range(5, 20)]
        public int lineCount = 10;           // 风线数量
        public float minLength = 1f;         // 最小长度
        public float maxLength = 3f;         // 最大长度
        public float minSpeed = 2f;          // 最小速度
        public float maxSpeed = 5f;          // 最大速度
        public float lineWidth = 0.1f;       // 线条宽度
        public Color lineColor = new Color(0.5f, 0.7f, 1f, 0.5f);
    }

    [SerializeField] private WindFieldSettings fieldSettings;
    [SerializeField] private WindLineSettings lineSettings;

    private List<GameObject> windLines = new List<GameObject>();

    void Start() {
        GenerateWindLines();
        // 添加和配置BoxCollider
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(fieldSettings.fieldWidth, fieldSettings.fieldHeight, fieldSettings.fieldLength);
        boxCollider.center = Vector3.zero;
    }

    void Update() {
        UpdateWindLines();
        ApplyWindForce(); // 添加持续应用风力的方法
    }

    // 新增：持续应用风力的方法
    private void ApplyWindForce()
    {
        Collider[] colliders = Physics.OverlapBox(
            transform.position,
            new Vector3(fieldSettings.fieldWidth/2, fieldSettings.fieldHeight/2, fieldSettings.fieldLength/2),
            transform.rotation
        );

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(fieldSettings.targetTag))
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(transform.forward * fieldSettings.windForce, ForceMode.Force);
                }
            }
        }
    }

    private void GenerateWindLines() {
        for (int i = 0; i < lineSettings.lineCount; i++) {
            GameObject windLine = new GameObject("WindLine");
            windLine.transform.SetParent(transform);
            
            // 在局部空间中生成风线位置
            windLine.transform.localPosition = new Vector3(
                Random.Range(-fieldSettings.fieldWidth / 2, fieldSettings.fieldWidth / 2),
                Random.Range(-fieldSettings.fieldHeight / 2, fieldSettings.fieldHeight / 2),
                Random.Range(-fieldSettings.fieldLength / 2, fieldSettings.fieldLength / 2)
            );
            
            // 设置风线方向与物体前方一致
            windLine.transform.forward = transform.forward;
            
            LineRenderer lineRenderer = windLine.AddComponent<LineRenderer>();
            lineRenderer.startWidth = lineSettings.lineWidth * transform.lossyScale.magnitude;
            lineRenderer.endWidth = lineSettings.lineWidth * transform.lossyScale.magnitude;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = lineSettings.lineColor;
            lineRenderer.endColor = lineSettings.lineColor;
            windLines.Add(windLine);
        }
    }

    private void UpdateWindLines() {
        foreach (GameObject windLine in windLines) {
            LineRenderer lineRenderer = windLine.GetComponent<LineRenderer>();
            float length = Random.Range(lineSettings.minLength, lineSettings.maxLength) * transform.lossyScale.z;
            float speed = Random.Range(lineSettings.minSpeed, lineSettings.maxSpeed);
            
            // 使用物体的前方方向
            Vector3 windDirection = transform.forward;
            
            // 更新风线位置和方向
            windLine.transform.Translate(windDirection * speed * Time.deltaTime, Space.World);
            
            // 在世界空间中设置风线起点和终点
            Vector3 startPos = windLine.transform.position;
            Vector3 endPos = startPos + windDirection * length;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            // 检查是否超出范围并重置
            float localZ = transform.InverseTransformPoint(windLine.transform.position).z;
            if (localZ > fieldSettings.fieldLength / 2) {
                Vector3 resetPos = transform.TransformPoint(new Vector3(
                    Random.Range(-fieldSettings.fieldWidth / 2, fieldSettings.fieldWidth / 2),
                    Random.Range(-fieldSettings.fieldHeight / 2, fieldSettings.fieldHeight / 2),
                    -fieldSettings.fieldLength / 2
                ));
                windLine.transform.position = resetPos;
                windLine.transform.forward = transform.forward;
            }
        }
    }

    private void OnDrawGizmos() {
        // 使用物体的变换矩阵绘制Gizmos
        Gizmos.color = fieldSettings.visualizationColor;
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(
            fieldSettings.fieldWidth,
            fieldSettings.fieldHeight,
            fieldSettings.fieldLength
        ));
        Gizmos.matrix = originalMatrix;
    }
}
