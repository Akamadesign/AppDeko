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

    AkaStore akaStore;
    public void ButtonAction()
    {
        akaStore = FindObjectOfType<AkaStore>();
        if (collection != null)
        {
            print("CollectionButton");
            akaStore.QueryAndDrawSomeProducts(collection);
        }
        else if (product != null)
        {
            AkaARManager arManager = FindObjectOfType<AkaARManager>();

            variantes = (List<ProductVariant>)product.variants();
            if (akaStore != null)
            {
                akaStore.SetNewView(AkaStore.View.Details);
                akaStore.detailsManager.FillWithDetails(product, currentVariant != null ? currentVariant : variantes[0]);
            }
            print("ProductButton " + product.title());
            if (arManager != null && arManager.gameObject.activeInHierarchy && arManager.selectedObject != null)
            {
                akaStore.detailsManager.GetComponent<LoadAssets>().WatchThisFittmentOnAR(currentVariant.id(), false);
            }
        }
    }
}
