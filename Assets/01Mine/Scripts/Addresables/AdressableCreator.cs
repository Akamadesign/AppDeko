using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AdressableCreator : MonoBehaviour
{
    public void CreateAssetReferenceList()
    {
        List<string> variantData = FindObjectOfType<AkaDetailsManager>().GetProductVariantInfo();
        SItem newAssetList = new SItem();
        newAssetList.toDownload = new List<AssetReference>();
        newAssetList.variantTitle = variantData[1];
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
        AssetDatabase.CreateAsset(newAssetList, "Assets/01Mine/ShopifyScriptables/" + variantData[0]+ ".asset");
        AssetDatabase.SaveAssets();
    }
}
