using UnityEngine;
using TMPro;
using Shopify.Unity;
using System.Collections.Generic;
public class AkaGridItem : MonoBehaviour
{
    public TMP_Text title;
    public UnityEngine.UI.Image image, brokenImage;
    public Collection collection;
    public Product product;

    List<ProductVariant> variantes;
    public ProductVariant currentVariant;
    public void ButtonAction()
    {
        if (collection != null)
        {
            print("CollectionButton");
            FindObjectOfType<AkamaruStore>().QueryAndDrawSomeProducts(collection);
        }
        else if (product != null)
        {
            variantes = (List<ProductVariant>)product.variants();
            AkamaruStore AkaStore = FindObjectOfType<AkamaruStore>();
            AkaStore.SetNewView(AkamaruStore.View.Details);
            AkaStore.detailsManager.FillWithDetails(product, currentVariant != null ? currentVariant : variantes[0]);
            print("ProductButton " + product.title());
        }
    }
}
