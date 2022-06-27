using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AdressableCreator : MonoBehaviour
{
#if UNITY_EDITOR
    public void CreateAssetReferenceList()
    {
        List<string> variantData = FindObjectOfType<AkaDetailsManager>().GetProductVariantInfo();
        SItem newAssetList = new SItem();
        newAssetList.toDownload = new List<AssetReference>();
        newAssetList.vID = variantData[0];
        newAssetList.pID = variantData[1];
        newAssetList.variantTitle = variantData[2];
        List<string> opciones = new List<string>();
        for (int i = 2; i < variantData.Count; i++)
        {
            opciones.Add(variantData[i]);
        }
        newAssetList.variantOptions = opciones;
        if (!AssetDatabase.IsValidFolder("Assets/01Mine"))
            AssetDatabase.CreateFolder("Assets", "01Mine");
        if (!AssetDatabase.IsValidFolder("Assets/01Mine/ShopifyScriptables"))
            AssetDatabase.CreateFolder("Assets/01Mine", "ShopifyScriptables");
        if(System.IO.File.Exists(Application.dataPath +  "/01Mine/ShopifyScriptables/" + variantData[0] + ".asset"))
        {
            print("Este asset ya existe, se llama : " + variantData[0]);
        }
        else
        {
            AssetDatabase.CreateAsset(newAssetList, "Assets/01Mine/ShopifyScriptables/" + variantData[0] + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
