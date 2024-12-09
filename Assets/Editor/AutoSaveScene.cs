using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AutoSaveScene : MonoBehaviour
{
    // 定义保存的时间间隔，单位为秒，这里设置为15分钟，即15 * 60秒
    private const int SaveInterval = 15 * 60;
    // 用于记录上次保存的时间
    private static float lastSaveTime = 0f;

    [MenuItem("Tools/Auto Save Scene")]
    static void EnableAutoSave()
    {
        EditorApplication.update += AutoSave;
    }

    static void AutoSave()
    {
        if (EditorApplication.timeSinceStartup - lastSaveTime >= SaveInterval)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            lastSaveTime = (float)EditorApplication.timeSinceStartup;
            Debug.Log("Scene has been auto saved.");
        }
    }
}