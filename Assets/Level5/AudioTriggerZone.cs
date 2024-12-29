using UnityEngine;
using UnityEngine.Events;

public class AudioTriggerZone : MonoBehaviour
{
    [Header("音频设置")]
    public AudioClip triggerSound;
    
    [Header("触发区域设置")]
    public Vector3 triggerAreaSize = new Vector3(5f, 5f, 5f);
    public Vector3 triggerAreaOffset = Vector3.zero;

    [Header("触发标签")]
    public string[] triggerTags = { "Player" };

    [Header("事件")]
    public UnityEvent onTriggerEvent;

    [Header("音频设置")]
    public float volume = 1f;

    private bool hasTriggered = false;
    private AudioSource audioSource;

    void Awake()
    {
        // 初始化音频源
        InitializeAudioSource();
    }

    void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = triggerSound;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    void Update()
    {
        // 如果尚未触发，检查触发区域
        if (!hasTriggered)
        {
            CheckTriggerZone();
        }
    }

    void CheckTriggerZone()
    {
        // 使用 OverlapBox 检测区域内的碰撞体
        Collider[] colliders = Physics.OverlapBox(
            transform.position + triggerAreaOffset, 
            triggerAreaSize / 2
        );

        foreach (Collider col in colliders)
        {
            // 检查碰撞体的标签是否匹配
            if (IsValidTag(col.gameObject.tag))
            {
                TriggerEffects();
                break;
            }
        }
    }

    bool IsValidTag(string tag)
    {
        // 检查标签是否在允许的标签列表中
        foreach (string validTag in triggerTags)
        {
            if (tag == validTag)
            {
                return true;
            }
        }
        return false;
    }

    void TriggerEffects()
    {
        // 确保只触发一次
        if (!hasTriggered)
        {
            // 播放音频
            if (triggerSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(triggerSound, volume);
            }

            // 触发事件
            onTriggerEvent?.Invoke();

            // 标记已触发
            hasTriggered = true;

            // 脚本，禁用防止进一步检查
            this.enabled = false;

            // 调试日志
            Debug.Log($"[{gameObject.name}] 触发区域生效，已禁用脚本");
        }
    }

    // 在场景视图中绘制触发区域
    void OnDrawGizmos()
    {
        Gizmos.color = hasTriggered ? Color.red : Color.yellow;
        Gizmos.DrawWireCube(transform.position + triggerAreaOffset, triggerAreaSize);
    }
}
