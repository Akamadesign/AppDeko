using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AddresableCreator : MonoBehaviour
{
#if UNITY_EDITOR
    public void CreateAssetReferenceList()
    {
        List<string> variantData = FindObjectOfType<AkaDetailsManager>().GetProductInfo();
        AkaReference newAssetList = new AkaReference();
        newAssetList.variantPrefabs = new List<AssetReference>();
        newAssetList.referenceAssets = new List<AssetReference>();
        newAssetList.pID = variantData[0];
        newAssetList.productTitle = variantData[1];
        List<string> variantes = new List<string>();
        for (int i = 2; i < variantData.Count; i++)
        {
            variantes.Add(variantData[i]);
        }
        if (!AssetDatabase.IsValidFolder("Assets/01Mine"))
            AssetDatabase.CreateFolder("Assets", "01Mine");
        if (!AssetDatabase.IsValidFolder("Assets/01Mine/01InitialAssets"))
            AssetDatabase.CreateFolder("Assets/01Mine", "01InitialAssets");
        if (System.IO.File.Exists(Application.dataPath + "/01Mine/01InitialAssets/" + variantData[0] + ".asset"))
        {
            print("Este asset ya existe, se llama : " + variantData[0]);
        }
        else
        {
            AssetDatabase.CreateAsset(newAssetList, "Assets/01Mine/01InitialAssets/" + variantData[0] + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
