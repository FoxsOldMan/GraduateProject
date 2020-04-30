using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class BuildBundle : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundle")]
    static void BuildAllAssetBundles()
    {
        string bundlePath = "C:/Users/XPS/Desktop/MyAssetBundle";
        if (!Directory.Exists(bundlePath)){
            Directory.CreateDirectory(bundlePath);
        }

        BuildPipeline.BuildAssetBundles(bundlePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
