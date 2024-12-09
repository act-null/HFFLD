using UnityEngine;

public class ThornInteraction : MonoBehaviour
{
    // 用于标记是否已经触发过效果，确保只执行一次
    private bool hasTriggeredEffect = false;
    // 记录初始的缩放比例，用于后续还原计算
    private Vector3 initialScale;
    // 记录初始的颜色，用于后续变色过程中的过渡计算
    private Color initialColor;
    // 用于记录效果开始的时间
    private float startTime;

    void Start()
    {
        initialScale = transform.localScale;
        initialColor = GetComponent<Renderer>().material.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasTriggeredEffect && collision.gameObject.CompareTag("Torch"))
        {
            hasTriggeredEffect = true;
            startTime = Time.time;
        }
    }

    void Update()
    {
        if (hasTriggeredEffect && Time.time - startTime < 5f)
        {
            float elapsedTime = Time.time - startTime;

            // 计算缩放比例的插值，逐渐缩小100倍
            float scaleFactor = Mathf.Lerp(1f, 0.01f, elapsedTime / 5f);
            transform.localScale = initialScale * scaleFactor;

            // 计算颜色的插值，逐渐变为橙色
            Color targetColor = new Color(1f, 0.5f, 0f);
            GetComponent<Renderer>().material.color = Color.Lerp(initialColor, targetColor, elapsedTime / 5f);

            // 修改材质的自发光属性，实现自发光橙光效果
            SetEmissionColor(Color.Lerp(initialColor, targetColor, elapsedTime / 5f));
        }
        else if (hasTriggeredEffect && Time.time - startTime >= 5f)
        {
            // 5秒后销毁物体，使其消失
            Destroy(gameObject);
        }
    }

    // 辅助方法，用于设置材质的自发光颜色
    private void SetEmissionColor(Color color)
    {
        // 获取物体的渲染器组件
        Renderer renderer = GetComponent<Renderer>();
        if (renderer!= null)
        {
            // 获取材质实例
            Material mat = renderer.material;
            // 设置自发光颜色，这里假设使用的是Standard材质，其自发光属性名为"_EmissionColor"，如果是其他材质可能需要调整属性名
            mat.SetColor("_EmissionColor", color);
            // 启用自发光，将自发光强度设置为1，不同材质可能设置方式略有不同，需根据实际情况调整
            mat.EnableKeyword("_EMISSION");
            mat.SetFloat("_EmissionIntensity", 1f);
        }
    }
}