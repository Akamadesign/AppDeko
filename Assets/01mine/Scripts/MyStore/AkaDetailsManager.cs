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
    [SerializeField] TMP_Text namePlace, pricePlace;
    [SerializeField] RectTransform optionsPlace, rabutton2, cartOptionsPlace, descriptionTitle;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_InputField quantityPlace;
    [Header("Prefabs")]
    [SerializeField] GameObject productOptionPrefab, gridItemPrefab;
    Product product;
    ProductVariant variant;
    List<ProductOption> pOptions;
    int quantity = 1;

    public void RADetailsFill(Product prdct, ProductVariant vrnt)
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
        loadingBar.fillAmount = 0;
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
        rabutton2.anchoredPosition = new Vector2(0, optionsPlace.anchoredPosition.y - optionsPlace.sizeDelta.y - 100f);
        cartOptionsPlace.anchoredPosition = new Vector2(0, rabutton2.anchoredPosition.y - rabutton2.sizeDelta.y);
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
    public void ThisVariantToRA()
    {
        GetComponent<LoadAssets>().WatchThisFittmentOnAR(variant.id(),true);
    }
#if UNITY_EDITOR
    public List<string> GetProductVariantInfo()
    {
        List<string> fittmentData = new List<string>();
        fittmentData.Add(variant.id());
        fittmentData.Add(product.id());
        fittmentData.Add(product.title() + ">>" +variant.title());
        foreach (var item in variant.selectedOptions())
        {
            fittmentData.Add(item.name() + "//" + item.value());
        }
        return fittmentData;
    }
#endif
}
