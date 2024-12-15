using UnityEngine;

public class SpecialThornInteraction : MonoBehaviour
{
    // 用于标记是否已经触发过效果，确保只执行一次
    private bool hasTriggeredEffect = false;
    // 记录初始的缩放比例，用于后续还原计算
    private Vector3 initialScale;
    // 记录初始的颜色，用于后续变色过程中的过渡计算
    private Color initialColor;
    // 用于记录效果开始的时间
    private float startTime;
    // 引用触发效果的火炬
    private GameObject triggeredTorch;

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
            triggeredTorch = collision.gameObject;
        }
    }

    void Update()
    {
        if (hasTriggeredEffect)
        {
            float elapsedTime = Time.time - startTime;

            if (elapsedTime < 5f)
            {
                // 荆棘效果
                UpdateThornEffect(elapsedTime);

                // 火炬效果
                UpdateTorchEffect(elapsedTime);
            }
            else
            {
                // 5秒后销毁荆棘和火炬
                Destroy(gameObject);
                if (triggeredTorch != null)
                {
                    Destroy(triggeredTorch);
                }
            }
        }
    }

    void UpdateThornEffect(float elapsedTime)
    {
        // 计算缩放比例的插值，逐渐缩小100倍
        float scaleFactor = Mathf.Lerp(1f, 0.01f, elapsedTime / 5f);
        transform.localScale = initialScale * scaleFactor;

        // 计算颜色的插值，逐渐变为橙色
        Color targetColor = new Color(1f, 0.5f, 0f);
        GetComponent<Renderer>().material.color = Color.Lerp(initialColor, targetColor, elapsedTime / 5f);

        // 修改材质的自发光属性，实现自发光橙光效果
        SetEmissionColor(Color.Lerp(initialColor, targetColor, elapsedTime / 5f));
    }

    void UpdateTorchEffect(float elapsedTime)
    {
        if (triggeredTorch != null)
        {
            Renderer torchRenderer = triggeredTorch.GetComponent<Renderer>();
            if (torchRenderer != null)
            {
                // 火炬颜色逐渐变灰
                Color initialTorchColor = Color.white; // 假设火炬初始颜色为白色
                Color grayColor = Color.gray;
                torchRenderer.material.color = Color.Lerp(initialTorchColor, grayColor, elapsedTime / 5f);

                // 火炬逐渐变透明
                Color currentColor = torchRenderer.material.color;
                currentColor.a = 1 - (elapsedTime / 5f);
                torchRenderer.material.color = currentColor;
            }
        }
    }

    // 辅助方法，用于设置材质的自发光颜色
    private void SetEmissionColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            mat.SetColor("_EmissionColor", color);
            mat.EnableKeyword("_EMISSION");
            mat.SetFloat("_EmissionIntensity", 1f);
        }
    }
}
