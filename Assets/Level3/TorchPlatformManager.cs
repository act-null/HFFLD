using UnityEngine;
using System.Collections;

public class TorchPlatformManager : MonoBehaviour
{
    [Header("Torch References")]
    public GameObject originalTorch; // 初始指定的火把
    public GameObject torchPrefab; // 用于生成新火把的预制体

    [Header("Platform Settings")]
    public Transform platformTransform; // 平台的Transform
    public float torchSpawnRadius = 2f; // 火把生成区域半径

    [Header("Green Leaf Stage")]
    public GameObject greenLeafStage; // 绿叶台阶物体
    public float greenLeafMoveDistance = 2f; // 绿叶台阶在 Y 轴上移动的距离

    [Header("Spawn Settings")]
    public float spawnCheckInterval = 0.5f; // 检查火把位置的间隔时间

    private GameObject currentTorch; // 当前活跃的火把
    private bool isFirstSpawn = true; // 是否是第一次生成火把

    void Start()
    {
        // 初始化设置
        currentTorch = originalTorch;
        StartCoroutine(CheckTorchPosition());
    }

    IEnumerator CheckTorchPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnCheckInterval);

            // 检查当前火把是否在平台区域内
            if (!IsTorchInPlatformArea(currentTorch))
            {
                // 生成新的火把
                SpawnNewTorch();
            }
        }
    }

    bool IsTorchInPlatformArea(GameObject torch)
    {
        if (torch == null) return false;

        // 计算火把与平台中心的距离
        float distance = Vector3.Distance(torch.transform.position, platformTransform.position);
        return distance <= torchSpawnRadius;
    }

    void SpawnNewTorch()
    {
        // 如果当前火把不在区域内，生成新火把
        Vector3 spawnPosition = GetRandomPositionOnPlatform();
        GameObject newTorch = Instantiate(torchPrefab, spawnPosition, Quaternion.identity);

        // 如果是第一次生成火把，激活并移动绿叶台阶
        if (isFirstSpawn)
        {
            ActivateGreenLeafStage();
            isFirstSpawn = false;
        }

        // 更新当前火把
        currentTorch = newTorch;
    }

    Vector3 GetRandomPositionOnPlatform()
    {
        // 在平台区域内生成随机位置
        Vector2 randomCircle = Random.insideUnitCircle * torchSpawnRadius;
        Vector3 randomPosition = platformTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        randomPosition.y = platformTransform.position.y + 0.5f; // 稍微抬高生成高度
        return randomPosition;
    }

    void ActivateGreenLeafStage()
    {
        if (greenLeafStage != null)
        {
            greenLeafStage.SetActive(true);
            StartCoroutine(MoveGreenLeafStage());
        }
    }

    IEnumerator MoveGreenLeafStage()
    {
        if (greenLeafStage == null) yield break;

        // 获取初始位置
        Vector3 startPosition = greenLeafStage.transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, greenLeafMoveDistance, 0);

        // 5秒内沿 Y 轴移动
        float moveDuration = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / moveDuration;

            // 线性插值改变位置
            greenLeafStage.transform.position = Vector3.Lerp(startPosition, targetPosition, normalizedTime);

            yield return null;
        }

        // 确保最终到达目标位置
        greenLeafStage.transform.position = targetPosition;
    }

    // 可视化平台区域（仅在编辑器模式下）
    void OnDrawGizmosSelected()
    {
        if (platformTransform == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(platformTransform.position, torchSpawnRadius);
    }
}