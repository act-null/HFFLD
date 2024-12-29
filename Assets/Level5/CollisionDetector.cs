using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour {

    [Tooltip("需要检测的目标物体的标签列表（可添加多个标签）")]
    public string[] targetTags = new string[] { "Player" };

    [Tooltip("是否已经触发过")]
    private bool isTriggered = false;

    void OnCollisionEnter(Collision collision) {
        // 如果已经触发过，则返回
        if (isTriggered) {
            return;
        }

        // 检查碰撞物体的标签是否在目标标签列表中
        bool isTargetTag = false;
        foreach (string tag in targetTags) {
            if (collision.gameObject.CompareTag(tag)) {
                isTargetTag = true;
                break;
            }
        }

        if (!isTargetTag) {
            return;
        }

        // 获取并移除BoxCollider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null) {
            boxCollider.enabled = false;
        }

        // 获取所有子物体的Rigidbody组件
        Rigidbody[] childrenRigidbodies = GetComponentsInChildren<Rigidbody>();

        // 检查是否有子物体
        if (childrenRigidbodies.Length == 0) {
            Debug.LogWarning("没有找到带有Rigidbody组件的子物体！");
            return;
        }

        // 取消所有子物体的isKinematic属性
        foreach (Rigidbody rb in childrenRigidbodies) {
            rb.isKinematic = false;
        }

        // 标记已触发
        isTriggered = true;
    }

    void OnValidate() {
        // 检查所有标签是否存在
        if (targetTags != null) {
            foreach (string tag in targetTags) {
                if (!string.IsNullOrEmpty(tag)) {
                    bool tagExists = false;
                    foreach (string existingTag in UnityEditorInternal.InternalEditorUtility.tags) {
                        if (existingTag == tag) {
                            tagExists = true;
                            break;
                        }
                    }
                    if (!tagExists) {
                        Debug.LogError($"标签 '{tag}' 不存在！请在Tags设置中添加此标签。");
                    }
                }
            }
        }
    }
}
