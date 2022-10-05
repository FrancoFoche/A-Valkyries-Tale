#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;

public class ReplaceGameObjects : ScriptableWizard
{
    public bool copyScale = true;
    public ReplaceObjects[] replace;

    [System.Serializable]
    public struct ReplaceObjects
    {
        public GameObject NewType;
        public GameObject[] OldObjects;
    }
    
    [MenuItem("Custom/Replace GameObjects")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(ReplaceGameObjects), "Replace");
    }
 
    void OnWizardCreate()
    {
        foreach (var action in replace)
        {
            foreach (GameObject go in action.OldObjects)
            {
                GameObject newObject;
                newObject = (GameObject)PrefabUtility.InstantiatePrefab(action.NewType);
                newObject.transform.position = go.transform.position;
                newObject.transform.rotation = go.transform.rotation;
                if (copyScale)
                {
                    newObject.transform.localScale = go.transform.localScale;
                }
                newObject.transform.parent = go.transform.parent;
 
                DestroyImmediate(go);
 
            }
        }
        UpdatePrefab();
    }
    
    public static void UpdatePrefab()
    {
        #if UNITY_EDITOR
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
        #endif
    }
}
#endif
