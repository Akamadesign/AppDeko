using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shopify.Unity;
using TMPro;
public class AkamaruStore : MonoBehaviour
{
    [Header("Paginas para activar")]
    [SerializeField] GameObject initialPage, raScreen;
    [SerializeField] ScrollRect storeGrid;
    public GameObject screenCart;
    public AkaDetailsManager detailsManager;
    public enum View { Init, Collections, Furniture, Details, Cart, RA }
    [Header("Vista actual")]
    public View onScreen;
    [Header("Prefabricados")]
    [SerializeField] GameObject gridItemPrefab;
    [Header("UIToCange")]
    [SerializeField] UnityEngine.UI.Image backIcon;
    [SerializeField] TMP_Text backText;
    [SerializeField] Sprite minor, exit;


    MovingSidePannel storegridPanel, cartPanel, detailsPAnel;
    View scrrenBeforeCart;
    List<GameObject> gridItems;
    Collection lastCollection;
    UnityEngine.UI.Image backGround;
    void Start()
    {
        backGround = GetComponent<UnityEngine.UI.Image>();
        gridItems = new List<GameObject>();
        storegridPanel = storeGrid.GetComponent<MovingSidePannel>();
        cartPanel = screenCart.GetComponent<MovingSidePannel>();
        detailsPAnel = detailsManager.GetComponent<MovingSidePannel>();
        InitiallizeShopifyStore();
    }

    /// <summary>
    /// Inicia la aplicaion de la tienda
    /// </summary>
    public void StartStore()
    {
        InitiallizeShopifyStore();
        SetNewView(View.Collections);
        if(gridItems != null)
            CleanGridList();
        gridItems = new List<GameObject>();
    }

    /// <summary>
    /// Inicializa la tienda de shopify
    /// </summary>
    void InitiallizeShopifyStore()
    {
        ShopifyBuy.Init("c0aab686e583c894c50ca1b82b025137", "gyanezfeliu.myshopify.com");
        AkaCart.IniTializeCart();
    }

    /// <summary>
    /// Dibuja una lista de colecciones en cuadricula
    /// </summary>
    public void QueryAndDrawCollections()
    {
        ShopifyBuy.Client().collections((collections, error, after) =>
        {
            if (error != null)
            {
                Debug.Log(error.Description);
                switch (error.Type)
                {
                    // An HTTP error is actually Unity's WWW.error
                    case ShopifyError.ErrorType.HTTP:
                        break;
                    // It's unlikely but it may be that an invalid GraphQL query was sent.
                    // Report an issue to https://github.com/shopify/unity-buy-sdk/issues
                    case ShopifyError.ErrorType.GraphQL:
                        break;
                };
            }
            else
            {
                CleanGridList();
                FillGridList(collections);
            }
        });
    }

    /// <summary>
    /// Dibuja una lista de Productos en cuadricula
    /// </summary>
    /// <param name="collection">Coleción de çproductos a dibujar</param>
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

    /// <summary>
    /// Llena la cuadricula con la lista de colecciones
    /// </summary>
    /// <param name="collections">Lista de Coleciones</param>
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

    /// <summary>
    /// Llena la cuadricula con la lista de Productos
    /// </summary>
    /// <param name="products4">Lista de Productos</param>
    void FillGridList(List<Product> products)
    {
        foreach (Product item in products)
        {
            GameObject griditemGO = Instantiate(gridItemPrefab, storeGrid.content);
            AkaGridItem gridItem = griditemGO.GetComponent<AkaGridItem>();
            gridItems.Add(griditemGO);
            StartCoroutine(AkaImageHelper.FillGridItem(gridItem, item.title(), null, item));
        }
    }

    /// <summary>
    /// Limpia lista de items en la cuadricula
    /// </summary>
    public void CleanGridList()
    {
        foreach (GameObject item in gridItems)
        {
            Destroy(item);
        }
        gridItems = new List<GameObject>();
    }

    /// <summary>
    /// Configura la nueva vista
    /// </summary>
    /// <param name="view">Vista a configurar</param>
    public void SetNewView(View newView)
    {
        if (backGround == null)
            backGround = GetComponent<UnityEngine.UI.Image>();
        print("settingnew view to " + newView);
        if (newView == View.Cart)
            scrrenBeforeCart = onScreen;
        onScreen = newView;
        switch (onScreen)
        {
            case View.Init:
                backIcon.sprite = exit;
                backText.text = "Exit";
                backGround.color = Color.white;
                initialPage.SetActive(true);
                storegridPanel.toShow = false;
                cartPanel.toShow = false;
                detailsPAnel.toShow = false;
                raScreen.SetActive(false);
                break;
            case View.Collections:
                backIcon.sprite = minor;
                backText.text = "Back";
                backGround.color = Color.white;
                initialPage.SetActive(false);
                storegridPanel.toShow = true;
                cartPanel.toShow = false;
                detailsPAnel.toShow = false;
                QueryAndDrawCollections();
                raScreen.SetActive(false);
                break;
            case View.Furniture:
                backGround.color = Color.white;
                initialPage.SetActive(false);
                storegridPanel.toShow = true;
                cartPanel.toShow = false;
                detailsPAnel.toShow = false;
                raScreen.SetActive(false);
                break;
            case View.Details:
                backGround.color = Color.white;
                initialPage.SetActive(false);
                storegridPanel.toShow = false;
                cartPanel.toShow = false;
                detailsPAnel.toShow = true;
                raScreen.SetActive(false);
                break;
            case View.Cart:
                backGround.color = Color.white;
                initialPage.SetActive(false);
                storegridPanel.toShow = false;
                cartPanel.toShow = true;
                detailsPAnel.toShow = false;
                raScreen.SetActive(false);
                break;
            case View.RA:
                backGround.color = Color.clear;
                initialPage.SetActive(false);
                storegridPanel.toShow = false;
                cartPanel.toShow = false;
                detailsPAnel.toShow = false;
                raScreen.SetActive(true);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Retocede a la vosta anterior
    /// </summary>
    public void GoBack()
    {
        switch (onScreen)
        {
            case View.Init:
                Application.Quit();
                break;
            case View.Collections:
                SetNewView(View.Init);
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
