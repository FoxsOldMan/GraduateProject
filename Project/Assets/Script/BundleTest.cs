using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleTest : MonoBehaviour
{
    void Start()
    {
        AssetBundle ab = AssetBundle.LoadFromFile("C:/Users/XPS/Desktop/MyAssetBundle/charactors.abc");
        GameObject enemy = ab.LoadAsset<GameObject>("Enemy");
        Instantiate(enemy);
    }

}
