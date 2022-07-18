using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Shopify.Unity;
using Image = UnityEngine.UI.Image;
public class AkaDetailsManager : MonoBehaviour
{
    RectTransform contentRecT;
    [SerializeField] Image iconPlace, loadingBar, defImageIcon;
    [SerializeField] GameObject loadign;
    [SerializeField] TMP_Text namePlace, pricePlace;
    public RectTransform optionsPlace;
    [SerializeField] RectTransform rabutton;
    [SerializeField] Image raButtonIcon, dowloadButtonIcon;
    [SerializeField] RectTransform cartOptionsPlace, descriptionTitle;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_InputField quantityPlace;
    [Header("Prefabs")]
    [SerializeField] GameObject productOptionPrefab;
    [SerializeField] GameObject variantItemPrefab;
    Product product;
    ProductVariant variant;
    List<ProductOption> pOptions;
    int quantity = 1;

    public void FillWhitDetails(Product prdct, ProductVariant varnt)
    {
        variant = varnt;
        product = prdct;
        List<AkaProductOptions> optiones = new List<AkaProductOptions>();
        optiones.AddRange(optionsPlace.GetComponentsInChildren<AkaProductOptions>());
        foreach (var item in optiones)
        {
            Destroy(item.gameObject);
        }
        quantity = 1;
        contentRecT = GetComponent<ScrollRect>().content;
        namePlace.text = product.title();
        pricePlace.text = CurrencySynbol.GetCurrencySimbol(variant.priceV2().currencyCode()) + (float)variant.priceV2().amount();
        StartCoroutine(AkaImageHelper.FillIconImage(iconPlace, variant.image().transformedSrc(), defImageIcon, loadign));
        dowloadButtonIcon.gameObject.SetActive(!AkaCache.LoadedPrefabs.ContainsKey(variant.id()));

        raButtonIcon.gameObject.SetActive(AkaCache.LoadedPrefabs.ContainsKey(variant.id()));
        loadingBar.fillAmount = 0;
        pOptions = product.options();
        if (quantityPlace != null)
            quantityPlace.text = "" + quantity;
        descriptionText.text = product.description();
        for (int i = 0; i < pOptions.Count; i++)
        {
            GameObject optionGO = Instantiate(productOptionPrefab, optionsPlace);
            AkaProductOptions optionFill = optionGO.GetComponent<AkaProductOptions>();
            optionFill.PlaceNewSetOfOptions(product, i, variant, variantItemPrefab);
        }
        StartCoroutine(ReArrangeitmes());
    }
    IEnumerator ReArrangeitmes()
    {
        yield return new WaitForSeconds(0.1f);
        rabutton.anchoredPosition = new Vector2(0, optionsPlace.anchoredPosition.y - optionsPlace.sizeDelta.y - 100f);
        cartOptionsPlace.anchoredPosition = new Vector2(0, rabutton.anchoredPosition.y - rabutton.sizeDelta.y);
        descriptionTitle.anchoredPosition = new Vector2(0, cartOptionsPlace.anchoredPosition.y - cartOptionsPlace.sizeDelta.y);
        descriptionText.rectTransform.anchoredPosition = new Vector2(0, descriptionTitle.anchoredPosition.y - descriptionTitle.sizeDelta.y);
        contentRecT.sizeDelta = new Vector2(0, Mathf.Abs(descriptionText.rectTransform.anchoredPosition.y - descriptionText.rectTransform.sizeDelta.y));
    }
    public void Changequantity(bool plus)
    {
        int inCart = AkaCart.AllreadyInCart(variant.id());
        if (plus)
        {
            if (quantity + inCart < variant.quantityAvailable())
                quantity++;
        }
        else
        {
            if (quantity > 1)
                quantity--;
        }
        quantityPlace.text = "" + quantity;
    }
    public void Changequantity(string number)
    {
        int inCart = AkaCart.AllreadyInCart(variant.id());
        if (number != "")
            quantity = int.Parse(number);
        if (quantity < 0)
            quantity = 0;
        if (quantity + inCart > variant.quantityAvailable())
            quantity = variant.quantityAvailable() - inCart;
        quantityPlace.text = "" + quantity;
    }
    public void AddToChart()
    {
        int inCart = AkaCart.AllreadyInCart(variant.id());
        if (quantity > 0)
        {
            if (inCart + quantity < variant.quantityAvailable())
                AkaCart.AddOrUpdate(product, variant, quantity, true);
            else
                AkaCart.AddOrUpdate(product, variant, variant.quantityAvailable(), false);
        }
        ///Hace falta enviar un mensaje al usuario por si no hay mas de estos para añadir al carrito
    }
    public void ThisVariantToRA()
    {
        GetComponent<LoadAssets>().WatchThisFittmentOnAR(product.id(), variant.id(), true);
    }
#if UNITY_EDITOR
    public List<string> GetProductInfo()
    {
        List<string> productData = new List<string>();
        productData.Add(product.id());
        productData.Add(product.title());
        //List<ProductVariant> variants = (List < ProductVariant > )product.variants();
        //foreach (var item in variants)
        //{
        //    productData.Add(item.title());
        //}
        return productData;
    }
#endif

}
