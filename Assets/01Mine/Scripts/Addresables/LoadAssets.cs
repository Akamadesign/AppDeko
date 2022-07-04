using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadAssets : MonoBehaviour
{
    [Header("LoadStrand")]
    public Image filler;
    [Header("Dynamic")]
    public string fittmentID;
    public SItem item;
    public float parts, progress, partSection;
    public int section;
    public List<AssetReference> assetsToDownload;

    bool executingSomeAsync;
    bool newObject;
    /// <summary>
    /// load a tifftmet to RA 
    /// </summary>
    /// <param name="fittmentID"></param>
    /// <param name="newGameObject"></param>
    public void WatchThisFittmentOnAR(string fittmentID, bool newGameObject)
    {
        newObject = newGameObject;
        print("Load asset was invoked");
        this.fittmentID = fittmentID;
        item = null;
        if (AkaPrefabs.LoadedPrefabs.ContainsKey(this.fittmentID))
        {
            if (newObject)
                PrefabToAR();
            else
                ReplacePrefab();
        }
        else
            StartCoroutine(DownloadScriptableReference());
    }
    IEnumerator DownloadScriptableReference()
    {
        executingSomeAsync = true;
        print("Just started to download the scriptable object");
        string addres = "Assets/01Mine/ShopifyScriptables/" + fittmentID + ".asset";
        print("this will download the Scriptable asset at this addres: '" + addres + "'");
        yield return new WaitForSeconds(0.01f);
        AsyncOperationHandle<SItem> pref = Addressables.LoadAssetAsync<SItem>(addres);
        pref.Completed += LoadScriptable;
    }
    void LoadScriptable(AsyncOperationHandle<SItem> itm)
    {
        if (itm.Status == AsyncOperationStatus.Succeeded)
        {
            print(itm.Result.GetType() + " Es el tipo de archivo");
            print("ScriptableDowloaded");
            item = itm.Result;
            parts = item.toDownload.Count + 2;
            partSection = 1 / parts;
            progress = partSection;
            section = 0;
            filler.fillAmount = progress;
            Addressables.Release(itm);
            print("Handle Released");
            assetsToDownload = item.toDownload;
            executingSomeAsync = false;
            StartCoroutine(DowloadFolderContent());
        }
    }
    IEnumerator DowloadFolderContent()
    {
        executingSomeAsync = true;
        yield return new WaitForSeconds(0.01f);
        print("CourutineSuccesfull Lauched");
        foreach (AssetReference assetR in assetsToDownload)
        {
            AsyncOperationHandle asstR = Addressables.DownloadDependenciesAsync(assetR, true);
            asstR.Completed += AssetCompleted;
        }
    }
    void AssetCompleted(AsyncOperationHandle assetH)
    {
        if (assetH.Status == AsyncOperationStatus.Succeeded)
        {
            progress += partSection;
            section++;
            filler.fillAmount = progress;
            print(progress + " AssetDowloaded");
            if (section == assetsToDownload.Count)
            {
                print("Last Asset Downloaded");
                StartCoroutine(LoadPrefab());
            }
        }
    }
    IEnumerator LoadPrefab()
    {
        print("Starting to load the Prefab");
        executingSomeAsync = true;
        yield return new WaitForSeconds(0.01f);
        AsyncOperationHandle<GameObject> prefabHandle = Addressables.LoadAssetAsync<GameObject>(assetsToDownload[0]);
        prefabHandle.Completed += PrefabDownloadCompletted;
    }
    void PrefabDownloadCompletted(AsyncOperationHandle<GameObject> pref)
    {
        if (pref.Status == AsyncOperationStatus.Succeeded)
        {
            print("prefab Loaded");
            AkaPrefabs.LoadedPrefabs.Add(fittmentID, pref.Result);
            if (newObject)
                PrefabToAR();
            else
                ReplacePrefab();
        }
        executingSomeAsync = false;
    }
    void PrefabToAR()
    {
        AkamaruStore akamaruStore = FindObjectOfType<AkamaruStore>();
        AkaStore akaStore = FindObjectOfType<AkaStore>();
        if (akamaruStore == null)
            akaStore.ToogleViewToRA();
        else
            akamaruStore.SetNewView(AkamaruStore.View.RA);
        FindObjectOfType<AkaARManager>().AddThisFittment(AkaPrefabs.LoadedPrefabs[fittmentID]);
    }
    void ReplacePrefab()
    {
        print("Replacing fitttment");
        FindObjectOfType<AkaARManager>().ReplacePrefab(AkaPrefabs.LoadedPrefabs[fittmentID]);
    }
}
