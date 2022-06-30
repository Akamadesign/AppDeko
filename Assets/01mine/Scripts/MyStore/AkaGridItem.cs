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

    AkamaruStore akamaruStore;
    AkaStore akaStore;
    public void ButtonAction()
    {
        akamaruStore = FindObjectOfType<AkamaruStore>();
        akaStore = FindObjectOfType<AkaStore>();
        if (collection != null)
        {
            print("CollectionButton");
            if (akamaruStore != null)
                akamaruStore.QueryAndDrawSomeProducts(collection);
            else
                akaStore.QueryAndDrawSomeProducts(collection);
        }
        else if (product != null)
        {
            MyARManager arManager = FindObjectOfType<MyARManager>();

            if (arManager != null && arManager.gameObject.activeInHierarchy && arManager.selectedObject != null)
            {
                print("ChangeModel");
                arManager.fittmentDetails.GetComponent<RAFittmentDetails>().FillWithDetails(product, currentVariant);
                FindObjectOfType<RAFittmentDetails>().GetComponent<LoadAssets>().WatchThisFittmentOnAR(currentVariant.id(), false);
            }
            else
            {
                variantes = (List<ProductVariant>)product.variants();
                if (akamaruStore != null)
                {
                    akamaruStore.SetNewView(AkamaruStore.View.Details);
                    akamaruStore.detailsManager.RADetailsFill(product, currentVariant != null ? currentVariant : variantes[0]);
                }
                if (akaStore != null)
                {
                    akaStore.SetNewView(AkaStore.View.Details);
                    akaStore.detailsManager.RADetailsFill(product, currentVariant != null ? currentVariant : variantes[0]);
                }
                print("ProductButton " + product.title());
            }
        }
    }
}
