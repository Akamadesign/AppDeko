using UnityEngine;
using TMPro;
using Shopify.Unity;
using System.Collections.Generic;
using UnityEngine.UI;
public class AkaProductsOption : MonoBehaviour
{
    [SerializeField] TMP_Text tItle;
    [SerializeField] ScrollRect rectView;
    public void PlaceNewSetOfOptions(Product rootProduct, int optionIndex, ProductVariant currVar, GameObject gridPrefab)
    {
        List<ProductVariant> rootProductVariants = (List<ProductVariant>)rootProduct.variants();
        ProductOption selectedOption = rootProduct.options()[optionIndex];
        List<string> selectedOptionValues = selectedOption.values();
        tItle.text = selectedOption.name();
        foreach (var optionValue in selectedOptionValues)
        {
            GameObject griditemGO = Instantiate(gridPrefab, rectView.content);
            AkaGridItem gridItem = griditemGO.GetComponent<AkaGridItem>();
            Button itemButton = griditemGO.GetComponent<Button>();
            itemButton.interactable = false;
            List<ProductVariant> similarVariants = new List<ProductVariant>();
            foreach (var productVariant in rootProductVariants)
            {
                bool almostSameVariant = true;
                for (int i = 0; i < productVariant.selectedOptions().Count; i++)
                {
                    if (i != optionIndex)
                    {
                        if (productVariant.selectedOptions()[i].value() != currVar.selectedOptions()[i].value())
                            almostSameVariant = false;
                    }
                }
                if (almostSameVariant)
                    similarVariants.Add(productVariant);
            }
            foreach (var item in similarVariants)
            {
                if (item.selectedOptions()[optionIndex].value() == optionValue)
                    StartCoroutine(AkaImageHelper.FillProductVariant(gridItem, rootProduct, item, optionValue));
            }
            if (currVar.selectedOptions()[optionIndex].value() != optionValue && gridItem.currentVariant.availableForSale())
                itemButton.interactable = true;
        }
    }
}
