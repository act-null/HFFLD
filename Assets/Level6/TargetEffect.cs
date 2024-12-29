using UnityEngine;

public class TargetEffect : MonoBehaviour
{
    public float effectDuration = 5f; // 效果持续时间
    private Material material;        // 物体的材质
    private Color originalColor;      // 物体的原始颜色
    private bool isEffectActive = false; // 是否正在播放效果
    private float effectTimer = 0f;   // 效果计时器

    private void Start()
    {
        // 获取物体的材质
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            originalColor = material.color;
        }
    }

    private void Update()
    {
        if (isEffectActive)
        {
            Debug.Log("isEffectActive");
            // 更新计时器
            effectTimer += Time.deltaTime;

            // 计算颜色插值
            float t = Mathf.Clamp01(effectTimer / effectDuration);
            Color targetColor = Color.Lerp(originalColor, new Color(1.0f, 0.5f, 0.0f), t); // 橙色
            material.color = targetColor;

            // 如果效果结束，销毁物体
            if (effectTimer >= effectDuration)
            {
                Destroy(gameObject);
            }
        }
    }

    // 触发效果
    public void TriggerEffect()
    {
        if (!isEffectActive)
        {
            isEffectActive = true;
            effectTimer = 0f;
        }
    }
}