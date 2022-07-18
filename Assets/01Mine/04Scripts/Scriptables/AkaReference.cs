using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(fileName = "NewProductReference", menuName = "ScriptableObjects/ProductReference", order = 1)]
public class AkaReference : ScriptableObject
{
    public string productTitle, pID;
    //public List<string> variantsIDs;
    public List<AssetReference> variantPrefabs;
    public List<AssetReference> referenceAssets;
}
