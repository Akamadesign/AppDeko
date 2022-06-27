using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemCatalog", order = 4)]
public class SItem : ScriptableObject
{
    public string variantTitle, vID, pID;
    public List<string> variantOptions;
    public List<AssetReference> toDownload;
}
