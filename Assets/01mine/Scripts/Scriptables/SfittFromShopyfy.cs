using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewShopifyItem", menuName = "ScriptableObjects/ShopifyItem", order = 3)]

public class SfittFromShopyfy : SGridItem
{
    [Header("Item-Only Properties")]
    public List<string> categories;
    public string storeID;
    public string company;
    public float price;
    public string color;
    public string size;
    public string description;
    public string tecnicFeatures;
    public List<string> tags;
    public bool aviable;
    public GameObject finalPrefab;


    public SfittFromShopyfy()
    {
        //get he information from shpppify and fill the fields before find the actual prefab
    }
    public void LoadThePrefab()
    {
        //FindObjectOfType<LoadAssetsManager>().LoadSomeAsset(this);
    }
}
