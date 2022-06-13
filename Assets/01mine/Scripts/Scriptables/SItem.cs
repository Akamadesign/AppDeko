using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemCatalog", order = 4)]
public class SItem : ScriptableObject
{
    public List<AssetReference> toDownload;
}
