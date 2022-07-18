using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AkaCartManager : MonoBehaviour
{
    [SerializeField] GameObject globalQuantity, cartItemPrefab;
    public ScrollRect cartPlace;
    [SerializeField] TMP_Text subtotal;
    List<GameObject> productsOnCart;

    private void OnEnable()
    {
        AkaCart.UpdateGlobalQuantity += UpdateGlobalQuantity;
    }
    private void OnDisable()
    {
        AkaCart.UpdateGlobalQuantity -= UpdateGlobalQuantity;
    }
    void UpdateGlobalQuantity()
    {
        print("UpdateGlobalQuantity()");
        globalQuantity.SetActive(AkaCart.myCart.LineItems.All().Count > 0 ? true : false);
        float finalQuantity = 0;

        foreach (var item in AkaCart.productsCache)
        {
            finalQuantity += item.Value.quantity;
        }
        globalQuantity.GetComponentInChildren<TMP_Text>().text = finalQuantity + "";
            DrawAllCart();
    }
    public void DrawAllCart()
    {
        string currCodeSymbol = "$";
        if (productsOnCart != null)
        {
            if (productsOnCart.Count > 0)
                foreach (var item in productsOnCart)
                {
                    Destroy(item);
                }
        }
        productsOnCart = new List<GameObject>();
        foreach (var item in AkaCart.productsCache)
        {
            print(item.Key + " out of " + AkaCart.productsCache.Count + " Products//is " + item.Value.product.title() + " Product// " + item.Value.variant.title() + " Variant// " + item.Value.quantity + " Quantity ");
            AkaCartProduct itemProduct = Instantiate(cartItemPrefab, cartPlace.content).GetComponent<AkaCartProduct>();
            productsOnCart.Add(itemProduct.gameObject);
            itemProduct.FillChartProduct(item.Value);
            currCodeSymbol = CurrencySynbol.GetCurrencySimbol(item.Value.variant.priceV2().currencyCode());
        }
        subtotal.text = currCodeSymbol + " " + AkaCart.myCart.Subtotal();
    }
    public void SetViewToCart()
    {
        AkaStore akaStore = FindObjectOfType<AkaStore>();
        if (akaStore.onScreen == AkaStore.View.Cart)
        {
            akaStore.GoBack();
        }
        else
        {
            akaStore.SetNewView(AkaStore.View.Cart);
        }
    }
    public void CheckOutLink()
    {
        AkaCart.myCart.GetWebCheckoutLink(
            success: (link) =>
            {
                Application.OpenURL(link);
            },
            failure: (checkoutError) =>
            {
                Debug.Log(checkoutError.Description);
            }
            );
    }
}
