using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shopify.Unity;
using TMPro;
public class AkaStore : MonoBehaviour
{

    public enum View { Init, Collections, Furniture, Details, Cart }
    [Header("Vista actual")]
    public View onScreen;
    View scrrenBeforeCart;

    [Header("Paginas para activar")]
    [SerializeField] MovingSidePannel thisMovingPanel;
    [SerializeField] ScrollRect storeGrid;
    public AkaDetailsManager akaDetailsManager;
    public AkaCartManager akaCartManager;
    public AkaARManager akaARManager;
    public GameObject initPage;
    public MovingSidePannel screenCart;
    [Header("Texto de mensaje")]
    public TMP_Text messageText;
    [Header("Prefabricados")]
    [SerializeField] GameObject gridItemPrefab;

    public bool realidadAumentadaEjecutandose;


    string message;
    List<GameObject> gridItems;
    Collection lastCollection;


    void Start()
    {
        ShopifyBuy.Init("c0aab686e583c894c50ca1b82b025137", "gyanezfeliu.myshopify.com");
        AkaCart.IniTializeCart();
        messageText.text = "Bienvenido! Vamos a ver muebles?";
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
        messageText.text = "Escoge una Coleccion";
    }
    void FillGridList(List<Collection> collections)
    {
        foreach (Collection item in collections)
        {
            GameObject griditemGO = Instantiate(gridItemPrefab, storeGrid.content);
            AkaGridItem gridItem = griditemGO.GetComponent<AkaGridItem>();
            gridItems.Add(griditemGO);
            gridItem.collection = item;
            gridItem.product = null;
            gridItem.currentVariant = null;
            gridItem.title.text = item.title();
            string imageURL = item.image().transformedSrc();
            StartCoroutine(AkaImageHelper.FillIconImage(gridItem.image, imageURL, gridItem.brokenImage, gridItem.loading));
        }
    }
    void FillGridList(List<Product> products)
    {
        foreach (Product item in products)
        {
            if (!AkaCache.productsOnStore.ContainsKey(item.id()))
            {
                AkaCache.productsOnStore.Add(item.id(), item);
            }
            GameObject griditemGO = Instantiate(gridItemPrefab, storeGrid.content);
            AkaGridItem gridItem = griditemGO.GetComponent<AkaGridItem>();
            gridItems.Add(griditemGO);

            gridItem.collection = null;
            gridItem.product = item;
            gridItem.currentVariant = null;
            gridItem.title.text = item.title();
            List<Shopify.Unity.Image> shImages = (List<Shopify.Unity.Image>)item.images();
            string imageURL = shImages[0].transformedSrc();
            StartCoroutine(AkaImageHelper.FillIconImage(gridItem.image, imageURL, gridItem.brokenImage, gridItem.loading));
        }
    }
    public void StartStore()
    {
        //thisMovingPanel.SetShowRect(new Vector2(0, 0), new Vector2(1, 1));
        //storeGrid.content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(300, 300);
        //akaDetailsManager.optionsPlace.GetComponent<GridLayoutGroup>().cellSize = new Vector2(900, 220);
        initPage.SetActive(false);
        ShoUpStore();
    }
    public void ShoUpStore()
    {
        SetNewView(View.Collections);
        if (gridItems != null)
            CleanGridList();
        thisMovingPanel.HideOrShowPanel(true);
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
        screenCart.HideOrShowPanel( newView == View.Cart);

        if (newView == View.Cart)
        {
            print("CartView");
            scrrenBeforeCart = onScreen;
        }
        else
        {
            print("no, cart view");
        }
        onScreen = newView;
        akaDetailsManager.gameObject.SetActive(onScreen == View.Details);
        storeGrid.viewport.gameObject.SetActive(onScreen != View.Cart);


        if (onScreen == View.Collections)
            QueryAndDrawCollections();
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