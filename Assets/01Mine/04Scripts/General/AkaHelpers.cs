using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Shopify.Unity;

public static class AkaCache
{
    public static Dictionary<string, Product> productsOnStore = new Dictionary<string, Product>();
    /// <summary>
    /// Loaded prefabsCache, key is the VariantID
    /// </summary>
    public static Dictionary<string, GameObject> LoadedPrefabs = new Dictionary<string, GameObject>();
}

public static class AkaImageHelper
{
    public static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    public static IEnumerator FillIconImage(UnityEngine.UI.Image image, string uRL, UnityEngine.UI.Image nullImage, GameObject loading)
    {
        Debug.Log("FillIconImage()");
        image.color = Color.clear;
        nullImage.gameObject.SetActive(true);
        loading.SetActive(true);
        if (uRL != null && uRL != "" && !textureCache.ContainsKey(uRL))
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(uRL))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    textureCache[uRL] = DownloadHandlerTexture.GetContent(uwr);
                }
            }
        }
        loading.SetActive(false);
        Texture2D texture = textureCache[uRL];
        image.color = Color.white;
        image.preserveAspect = true;
        image.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect
        );
        if (nullImage != null)
            nullImage.gameObject.SetActive(false);
    }
    public static void FillGridItem(AkaGridItem gridItem, string title, Collection collection = null, Product product = null)
    {
        Debug.Log("FillGridItem()");
        string imageURL = "";
        gridItem.collection = collection;
        gridItem.currentVariant = null;
        gridItem.product = product;
        gridItem.title.text = title;
        if (collection != null && collection.image() != null)
            imageURL = collection.image().transformedSrc();
        else if (product != null)
        {
            List<Shopify.Unity.Image> shImages = (List<Shopify.Unity.Image>)product.images();
            imageURL = shImages[0].transformedSrc();
        }
        
        FillIconImage(gridItem.image, imageURL, gridItem.brokenImage, gridItem.loading);
    }
    public static void FillProductVariant(AkaGridItem gridItem, Product product, ProductVariant variant, string value)
    {
        Debug.Log("FillProductVariant()");
        gridItem.collection = null;
        gridItem.product = product;
        gridItem.currentVariant = variant;
        gridItem.title.text = value;
        List<ProductVariant> variants = (List<ProductVariant>)product.variants();
        if(variant != null)
        {
            string imageURL = variant.image().transformedSrc();
            FillIconImage(gridItem.image, imageURL, gridItem.brokenImage, gridItem.loading);
        }
    }
}

[System.Serializable]
public class AkaProductToPurchaseReference
{
    public int quantity;
    public Product product { get; private set; }
    public ProductVariant variant { get; private set; }

    public AkaProductToPurchaseReference(Product product, ProductVariant variant, int quantity)
    {
        this.quantity = quantity;
        this.product = product;
        this.variant = variant;
    }
}

public static class AkaCart
{
    public static Cart myCart;
    public static Dictionary<string, AkaProductToPurchaseReference> productsCache { get; private set; }
    public delegate void UpdateQuantity();
    public static event UpdateQuantity UpdateGlobalQuantity;

    public static void IniTializeCart()
    {
        Debug.Log("IniTializeCart()");
        myCart = ShopifyBuy.Client().Cart();
        productsCache = new Dictionary<string, AkaProductToPurchaseReference>();
    }

    public static void AddOrUpdate(Product product, ProductVariant variant, int cantidad, bool addtocurrentQuantiitie)
    {
        Debug.Log("AddOrUpdte(" + product.title() + "/" + variant.title() + "/" + cantidad + ")");
        if(!productsCache.ContainsKey(variant.id()))
        {
            productsCache.Add(variant.id(), new AkaProductToPurchaseReference(product, variant, cantidad));
        }
        else
        {
            if (addtocurrentQuantiitie)
                productsCache[variant.id()].quantity += cantidad;
            else
                productsCache[variant.id()].quantity = cantidad;
        }
        myCart.LineItems.AddOrUpdate(variant, productsCache[variant.id()].quantity);
        if (UpdateGlobalQuantity != null)
            UpdateGlobalQuantity();
    }
    public static void EraseFromCart(string varianteID)
    {
        productsCache.Remove(varianteID);
        myCart.LineItems.Delete(varianteID);
        UpdateGlobalQuantity();
    }
    public static int AllreadyInCart(string varianteID)
    {
        if (myCart.LineItems.Get(varianteID) != null)
            return (int)myCart.LineItems.Get(varianteID).Quantity;
        else
            return 0;
    }

}
