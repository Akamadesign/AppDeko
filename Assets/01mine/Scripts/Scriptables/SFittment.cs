using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item", order = 2)]
public class SFittment : SGridItem
{
    [Header("Item-Only Properties")]
    public List<string> categories;
    public string storeID;
    public float price;
    public string color;
    public string size;
    public string description;
    public string tecnicFeatures;
    public List<string> tags;
    public GameObject prefab;
    public AssetReference assetReference;
    public bool aviable;
}
