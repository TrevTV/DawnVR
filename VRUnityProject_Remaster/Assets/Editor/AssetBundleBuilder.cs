using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleBuilder : EditorWindow
{
    [MenuItem("Window/AssetBundles")]
    public static void BuildBundleWindow()
    {
        GetWindow<AssetBundleBuilder>("AssetBundles");
    }

    private void OnGUI()
    {
        GUILayout.Label("AssetBundles Available", EditorStyles.centeredGreyMiniLabel);

        string[] bundles = AssetDatabase.GetAllAssetBundleNames();
        foreach (string bundle in bundles)
        {
            GUILayout.Space(20);

            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, stretchWidth = true };
            GUILayout.Label(bundle, style);
            if (GUILayout.Button("Build", EditorStyles.miniButton))
            {
                string savePath = EditorUtility.SaveFilePanel("Save AssetBundle", "", bundle, "");
                if (string.IsNullOrEmpty(savePath)) return;

                string fileName = Path.GetFileName(savePath);
                string folderPath = Path.GetDirectoryName(savePath);

                AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);
                assetBundleBuild.assetBundleName = fileName;

                if (assetBundleBuild.assetNames.Length == 0)
                {
                    EditorUtility.DisplayDialog("Error", "There are no assets in the selected bundle, aborting", "OK");
                    continue;
                }

                BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, EditorUserBuildSettings.activeBuildTarget);
                EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);
                EditorUserBuildSettings.SwitchActiveBuildTarget(EditorUserBuildSettings.activeBuildTarget);

                if (File.Exists(savePath))
                    File.Delete(savePath);

                File.Move(Application.temporaryCachePath + "/" + fileName, savePath);
                EditorUtility.DisplayDialog("", "Export Successful!", "OK");
            }
        }
    }
}
