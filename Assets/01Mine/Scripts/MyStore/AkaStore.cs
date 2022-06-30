using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shopify.Unity;
public class AkaStore : MonoBehaviour
{
    public enum View {Init, Collections, Furniture, Details, Cart, RA}
    [Header("Paginas para activar")]
    [SerializeField] GameObject initialPage;
    [SerializeField] GameObject details, screenCart;
    [SerializeField] ScrollRect storeGrid;
    public AkaDetailsManager detailsManager;
    [SerializeField] MovingSidePannel thisMovingPanel;
    [Header("Vista actual")]
    public View onScreen;
    [Header("Prefabricados")]
    [SerializeField] GameObject gridItemPrefab;
    List<GameObject> gridItems;
    Collection lastCollection;
    View scrrenBeforeCart;
    void OnEnable()
    {
        ShopifyBuy.Init("c0aab686e583c894c50ca1b82b025137", "gyanezfeliu.myshopify.com");
        AkaCart.IniTializeCart();
    }
    void CleanGridList()
    {
        if (gridItems != null)
            foreach (GameObject item in gridItems)
                Destroy(item);
        gridItems = new List<GameObject>();
    }
    void QueryAndDrawCollections()
    {

        List<Collection> coll = null;
        ShopifyBuy.Client().collections((collections, error, after) =>
        {
            if (error == null)
            {
                CleanGridList();
                FillGridList(collections);
            }
        });
    }
    void FillGridList(List<Collection> collections)
    {
        foreach (Collection item in collections)
        {
            GameObject griditemGO = Instantiate(gridItemPrefab, storeGrid.content);
            AkaGridItem gridItem = griditemGO.GetComponent<AkaGridItem>();
            gridItems.Add(griditemGO);
            StartCoroutine(AkaImageHelper.FillGridItem(gridItem, item.title(), item, null));
        }
    }
    void FillGridList(List<Product> products)
    {
        foreach (Product item in products)
        {
            if (!AkaProductsList.productsOnStore.ContainsKey(item.id()))
            {
                AkaProductsList.productsOnStore.Add(item.id(), item);
            }
            GameObject griditemGO = Instantiate(gridItemPrefab, storeGrid.content);
            AkaGridItem gridItem = griditemGO.GetComponent<AkaGridItem>();
            gridItems.Add(griditemGO);
            StartCoroutine(AkaImageHelper.FillGridItem(gridItem, item.title(), null, item));
        }
    }
    public void StartStore()
    {
        SetNewView(View.Collections);
        if (gridItems != null)
            CleanGridList();
        thisMovingPanel.SetShowRect(new Vector2(0, 0), new Vector2(1, 1));
        storeGrid.content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(300, 300);
        thisMovingPanel.toShow = true;
        initialPage.SetActive(false);
    }
    public void QueryAndDrawSomeProducts(Collection collection)
    {
        SetNewView(View.Furniture);
        lastCollection = collection;
        CleanGridList();
        List<Product> proucts = (List<Product>)collection.products();
        List<string> pdctsids = new List<string>();
        foreach (var item in proucts)
        {
            pdctsids.Add(item.id());
        }
        ShopifyBuy.Client().products((products, error) =>
        {
            FillGridList(products);
        }, pdctsids);
    }
    public void SetNewView(View newView)
    {
        print("settingnew view to " + newView);
        if (newView == View.Cart)
            scrrenBeforeCart = onScreen;
        onScreen = newView;
        details.SetActive(onScreen == View.Details);
        screenCart.gameObject.SetActive(onScreen == View.Cart);
        if (onScreen == View.Collections)
            QueryAndDrawCollections();
        if (newView == View.RA)
            thisMovingPanel.SetShowRect(new Vector2(1 / 3f, 0), new Vector2(1, 1));
    }
    public void GoBack()
    {
        switch (onScreen)
        {
            case View.Collections:
                break;
            case View.Furniture:
                SetNewView(View.Collections);
                break;
            case View.Details:
                QueryAndDrawSomeProducts(lastCollection);
                break;
            case View.Cart:
                SetNewView(scrrenBeforeCart);
                break;
            default:
                break;
        }
    }
}