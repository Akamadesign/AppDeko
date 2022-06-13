using UnityEngine;
using Image = UnityEngine.UI.Image;
using TMPro;
using Shopify.Unity;
public class AkaCartProduct : MonoBehaviour
{
    [SerializeField] GameObject variantTextPrefab;
    [SerializeField] Image iconPlacer, brokenImage;
    [SerializeField] TMP_Text productTitle;
    [SerializeField] RectTransform variantPlace;
    [SerializeField] TMP_Text unitPrice, totalPrice;
    [SerializeField] TMP_InputField quantityText;

    int quantity;
    ProductVariant variant;
    Product product;
    public void FillChartProduct(AkaProductToPurchaseReference reference)
    {
        string simbol = CurrencySynbol.GetCurrencySimbol(reference.variant.priceV2().currencyCode());
        quantity = reference.quantity;
        variant = reference.variant;
        product = reference.product;
        print("tratando de poner la imagen");
        StartCoroutine(AkaImageHelper.FillIconImage(iconPlacer, reference.variant.image().transformedSrc(), brokenImage));
        print("despues de poner la imagen");
        productTitle.text = reference.product.title();
        unitPrice.text = simbol + " " + reference.variant.priceV2().amount();
        string cantidad = "" + quantity;
        print(cantidad);
        print("" + quantityText.gameObject.activeInHierarchy);
        quantityText.text = cantidad;
        float finalValue = ((float)(reference.variant.priceV2().amount() * quantity));
        totalPrice.text = simbol + " " + finalValue + "";
        foreach (var item in reference.variant.selectedOptions())
        {
            TMP_Text variantOption = Instantiate(variantTextPrefab, variantPlace).GetComponent<TMP_Text>();
            variantOption.text = "" + item.value();
        }
    }
    public void Changequantity(bool plus)
    {
        print("Changing cart Product Quantity");
        if (plus)
        {
            print(quantity + " + 1");
            if (quantity < variant.quantityAvailable())
                quantity++;

            print(quantity);
        }
        else
        {
            if (quantity > 1)
                quantity--;
        }
        quantityText.text = "" + quantity;
        AkaCart.AddOrUpdate(product, variant, quantity);
    }
    public void Changequantity(string number)
    {
        print("Changing quantity to " + quantity);
        if (number != "")
            quantity = int.Parse(number);
        if (quantity < 1)
            quantity = 1;
        if (quantity > variant.quantityAvailable())
            quantity = variant.quantityAvailable();
        quantityText.text = "" + quantity;
        AkaCart.AddOrUpdate(product, variant, quantity);
    }
    public void DeleteFromCart()
    {
        AkaCart.EraseFromCart(variant.id());
    }
}
