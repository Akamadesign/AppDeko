using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Shopify.Unity;
using Image = UnityEngine.UI.Image;

public class RAFittmentDetails : MonoBehaviour
{
    RectTransform contentRecT;
    [SerializeField] Image iconPlace, defImageIcon;
    [SerializeField] TMP_Text namePlace, pricePlace;
    [SerializeField] RectTransform optionsPlace, cartOptionsPlace, descriptionTitle;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_InputField quantityPlace;
    [Header("Prefabs")]
    [SerializeField] GameObject productOptionPrefab, gridItemPrefab;
    Product product;
    ProductVariant variant;
    List<ProductOption> pOptions;
    int quantity = 1;

    public void FillWithDetails(Product prdct, ProductVariant vrnt)
    {
        print(prdct.title() + " // " + vrnt.title() + " tiene (" + vrnt.quantityAvailable() + ") elemntos disponibles");
        variant = vrnt;
        List<AkaProductsOption> optiones = new List<AkaProductsOption>();
        optiones.AddRange(optionsPlace.GetComponentsInChildren<AkaProductsOption>());
        foreach (var item in optiones)
        {
            Destroy(item.gameObject);
        }
        quantity = 1;
        contentRecT = GetComponent<ScrollRect>().content;
        product = prdct;
        namePlace.text = product.title();
        pricePlace.text = CurrencySynbol.GetCurrencySimbol(variant.priceV2().currencyCode()) + (float)vrnt.priceV2().amount();
        defImageIcon.gameObject.SetActive(true);
        StartCoroutine(AkaImageHelper.FillIconImage(iconPlace, vrnt.image().transformedSrc(), defImageIcon));
        pOptions = product.options();
        if (quantityPlace != null)
            quantityPlace.text = "" + quantity;
        descriptionText.text = product.description();
        for (int i = 0; i < pOptions.Count; i++)
        {
            GameObject optionGO = Instantiate(productOptionPrefab, optionsPlace);
            AkaProductsOption optionFill = optionGO.GetComponent<AkaProductsOption>();
            optionFill.PlaceNewSetOfOptions(product, i, vrnt, gridItemPrefab);
        }
        StartCoroutine(ReArrangeitmes());
    }
    IEnumerator ReArrangeitmes()
    {
        yield return new WaitForSeconds(0.1f);
        cartOptionsPlace.anchoredPosition = new Vector2(0, optionsPlace.anchoredPosition.y - optionsPlace.sizeDelta.y);
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
        if (quantity > 0)
        {
            AkaCart.AddOrUpdate(product, variant, quantity);
            FindObjectOfType<AkaCartManager>().UpdateGlobalQuantity();
        }
    }
}
