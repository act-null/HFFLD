using UnityEngine;
using System.Collections.Generic;

public class HornTrigger : MonoBehaviour
{
    public AudioClip hornSound;
    public List<GameObject> candlesToActivate;
    public Vector3 triggerAreaSize = new Vector3(5f, 5f, 5f);
    public Vector3 triggerAreaOffset = Vector3.zero;

    private bool hasTriggered = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = hornSound;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!hasTriggered)
        {
            Collider[] colliders = Physics.OverlapBox(transform.position + triggerAreaOffset, triggerAreaSize / 2);
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    TriggerHornAndCandles();
                    break;
                }
            }
        }
    }

    void TriggerHornAndCandles()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            audioSource.Play();

            foreach (GameObject candle in candlesToActivate)
            {
                if (candle != null)
                {
                    // 激活蜡烛游戏对象
                    candle.SetActive(true);

                    // 重启所有粒子系统
                    ParticleSystem[] particleSystems = candle.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (ParticleSystem ps in particleSystems)
                    {
                        ps.Clear();
                        ps.Play();
                    }

                    // 重启所有音频源
                    AudioSource[] audioSources = candle.GetComponentsInChildren<AudioSource>(true);
                    foreach (AudioSource audio in audioSources)
                    {
                        if (audio.playOnAwake)
                        {
                            audio.Play();
                        }
                    }

                    // 确保所有Light组件都被正确启用
                    Light[] lights = candle.GetComponentsInChildren<Light>(true);
                    foreach (Light light in lights)
                    {
                        light.enabled = true;
                    }

                    // 如果使用了动画器
                    Animator[] animators = candle.GetComponentsInChildren<Animator>(true);
                    foreach (Animator animator in animators)
                    {
                        animator.enabled = true;
                        animator.Rebind();
                        animator.Update(0f);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + triggerAreaOffset, triggerAreaSize);
    }
}
