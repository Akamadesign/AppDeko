using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Shopify.Unity;
using Image = UnityEngine.UI.Image;
public static class AkaImageHelper
{
    public static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

    /// <summary>
    /// Pone la imagen desde un imageURL en un image.Sprite o activa el brokenImage si esta disponible
    /// </summary>
    /// <param name="image">Imagen de UnityEngine.UI donde deberia aparecer la imagen</param>
    /// <param name="brokenImage">Imagen por defecto para mostrar en caso de no poder cragar la imagen desde URL</param>
    /// <param name="imageURL">URL de la imagen a cargar</param>
    /// <returns></returns>
    public static IEnumerator FillIconImage(Image image, string imageURL, Image brokenImage = null)
    {
        Debug.Log("Filling image");
        if (imageURL != null && imageURL != "" && !TextureCache.ContainsKey(imageURL))
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageURL))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Sprite not Founded at : " + imageURL);
                }
                else
                {
                    TextureCache[imageURL] = DownloadHandlerTexture.GetContent(uwr);
                }
            }
        }
        var texture = TextureCache[imageURL];
        image.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect
        );
        if (brokenImage != null)
            brokenImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pone datos en un GridItem de acuerdo a solicitud (Colección o Producto)
    /// </summary>
    /// <param name="gridItem">Prefabricado a llenar</param>
    /// <param name="title">Texto que llevara el GridItem</param>
    /// <param name="collection">Colección a mostrar</param>
    /// <param name="product">Producto a mostrar</param>
    /// <returns></returns>
    public static IEnumerator FillGridItem(AkaGridItem gridItem, string title, Collection collection = null, Product product = null)
    {
        //Debug.Log("Filling griItem");

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
        if (imageURL != null && imageURL != "" && !TextureCache.ContainsKey(imageURL))
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageURL))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Sprite not Founded at : " + imageURL);
                }
                else
                {
                    TextureCache[imageURL] = DownloadHandlerTexture.GetContent(uwr);
                }
            }
        }
        if (imageURL != "")
        {
            var texture = TextureCache[imageURL];
            gridItem.image.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect
            );

            gridItem.image.preserveAspect = true;
            gridItem.image.gameObject.SetActive(true);
            gridItem.image.color = Color.white;
            if (gridItem.brokenImage != null) gridItem.brokenImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Pone datos de posibles variantes en un tipo especial de GridItem
    /// </summary>
    /// <param name="gridItem">Prefabricado a llenar</param>
    /// <param name="prdct">Producto Raíz</param>
    /// <param name="variant">Variante a Dibujar</param>
    /// <param name="value">Titulo de la variante selecionada</param>
    /// <returns></returns>
    public static IEnumerator FillProductVariant(AkaGridItem gridItem, Product prdct, ProductVariant variant, string value)
    {
        Debug.Log("Filling productVariant");
        gridItem.collection = null;
        gridItem.product = prdct;
        gridItem.currentVariant = variant;
        gridItem.title.text = value;
        List<ProductVariant> variants = (List<ProductVariant>)prdct.variants();
        if (variant != null)
        {
            string imageURL = variant.image().transformedSrc();
            if (imageURL != null && imageURL != "" && !TextureCache.ContainsKey(imageURL))
            {
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageURL))
                {
                    yield return uwr.SendWebRequest();
                    if (uwr.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Sprite not Founded at : " + imageURL);
                    }
                    else
                    {
                        TextureCache[imageURL] = DownloadHandlerTexture.GetContent(uwr);
                    }
                }
            }
            var texture = TextureCache[imageURL];
            gridItem.image.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect
            );
            gridItem.image.preserveAspect = true;
            gridItem.image.gameObject.SetActive(true);
            gridItem.image.color = Color.white;
            if (gridItem.brokenImage != null) gridItem.brokenImage.gameObject.SetActive(false);
        }
        else
        {
            gridItem.image.gameObject.SetActive(false);
            if (gridItem.brokenImage != null) gridItem.brokenImage.gameObject.SetActive(true);
        }

    }

}

public static class AkaCart
{
    public static Cart myCart;
    public static Dictionary<string, AkaProductToPurchaseReference> cartProductCache { get; private set; }
    public delegate void UpdateQuantity();
    public static event UpdateQuantity UpdateGlobalQuantity;

    /// <summary>
    /// Inicializa el carrito de shopify
    /// </summary>
    public static void IniTializeCart()
    {
        myCart = ShopifyBuy.Client().Cart();
        cartProductCache = new Dictionary<string, AkaProductToPurchaseReference>();
    }

    /// <summary>
    /// Añade o actualiza un producto en el carrito d shopify y en el cache de productos del carrito
    /// </summary>
    /// <param name="producto"></param>
    /// <param name="variante"></param>
    /// <param name="cantidad"></param>
    public static void AddOrUpdate(Product producto, ProductVariant variante, int cantidad)
    {
        Debug.Log("productCart Updated to: " + producto.title() + "/" + variante.title() + "/" + cantidad + "Cantidad");
        if (!cartProductCache.ContainsKey(variante.id()))
        {
            cartProductCache.Add(variante.id(), new AkaProductToPurchaseReference(producto, variante, cantidad));
            myCart.LineItems.AddOrUpdate(variante, cantidad);
        }
        else
        {
            cartProductCache[variante.id()].quantity = cantidad;
            myCart.LineItems.AddOrUpdate(variante, cartProductCache[variante.id()].quantity);
        }
        UpdateGlobalQuantity();
    }

    /// <summary>
    /// Borra un producto del carrito de shopify y del cache de productos en el carrito
    /// </summary>
    /// <param name="varianteID"></param>
    public static void EraseFromCart(string varianteID)
    {
        cartProductCache.Remove(varianteID);
        myCart.LineItems.Delete(varianteID);
        UpdateGlobalQuantity();
    }
    public static int AllreadyInCart(string varianteID)
    {
        if (myCart.LineItems.Get(varianteID) != null)
            return (int)myCart.LineItems.Get(varianteID).Quantity;
        else
            return  0;
    }
}

public static class AkaPrefabs
{
    public static Dictionary<string, GameObject> LoadedPrefabs = new Dictionary<string, GameObject>();
}
/// <summary>
/// Estructura para definir un producto en el carrito de shopify
/// </summary>
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

