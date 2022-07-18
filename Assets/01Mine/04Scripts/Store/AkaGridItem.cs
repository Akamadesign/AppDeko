using UnityEngine;
using TMPro;
using Shopify.Unity;
using System.Collections.Generic;
public class AkaGridItem : MonoBehaviour
{
    public TMP_Text title;
    public UnityEngine.UI.Image image, brokenImage;
    public GameObject loading;
    public Collection collection;
    public Product product;

    List<ProductVariant> variantes;
    public ProductVariant currentVariant;

    AkaStore akaStore;
    public void ButtonAction()
    {
        if(akaStore == null)
            akaStore = FindObjectOfType<AkaStore>();
        if(collection != null)
        {
            print(collection.title() + " Colection Button");
            akaStore.QueryAndDrawSomeProducts(collection);
        }
        else if(product != null)
        {
            variantes = (List<ProductVariant>)product.variants();
            ProductVariant choosen = currentVariant != null ? currentVariant : variantes[0];
            print(product.title() + "Product/" + choosen.title() + "variant Button");
            akaStore.SetNewView(AkaStore.View.Details);
            akaStore.akaDetailsManager.FillWhitDetails(product, choosen);
            if (akaStore.akaARManager.selectedObject != null)
            {
                akaStore.akaDetailsManager.GetComponent<LoadAssets>().WatchThisFittmentOnAR(product.id(), currentVariant.id(), false);
            }
        }
    }
}
