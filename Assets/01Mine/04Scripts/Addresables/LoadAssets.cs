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
    string productID, fittmentID;
    AkaReference referenceLibrary;
    float parts, progress, partSection;
    int section;
    List<AssetReference> referenceAssetsToDownload, prefabsAssetsToDownload;
    bool newObject;
    public void WatchThisFittmentOnAR(string productID, string fittmentID, bool newObject)
    {
        print("WatchThisFittmentOnAR(" + fittmentID + "," + newObject + ")");
        this.newObject = newObject;
        this.fittmentID = fittmentID;
        this.productID = productID;
        referenceLibrary = null;
        if (AkaCache.LoadedPrefabs.ContainsKey(this.fittmentID))
        {
            PrefabToAR();
        }
        else
            StartCoroutine(DownloadScriptableReference());
    }
    IEnumerator DownloadScriptableReference()
    {
        print("DownloadScriptableReference()");
        string addres = "Assets/01Mine/01InitialAssets/" + productID + ".asset";
        yield return new WaitForEndOfFrame();
        AsyncOperationHandle<AkaReference> pref = Addressables.LoadAssetAsync<AkaReference>(addres);
        pref.Completed += ScriptableLoaded;
    }
    void ScriptableLoaded(AsyncOperationHandle<AkaReference> reference)
    {
        if(reference.Status == AsyncOperationStatus.Succeeded)
        {
            print("ScriptableLoaded()");
            referenceLibrary = reference.Result;
            parts = referenceLibrary.variantPrefabs.Count + referenceLibrary.referenceAssets.Count + 2;
            partSection = 1 / parts;
            progress = partSection;
            section = 0;
            filler.fillAmount = progress;
            Addressables.Release(reference);
            referenceAssetsToDownload = referenceLibrary.referenceAssets;
            prefabsAssetsToDownload = referenceLibrary.variantPrefabs;
            StartCoroutine(DowloadReferenceLibrary());
        }
    }
    IEnumerator DowloadReferenceLibrary()
    {
        print("DowloadReferenceLibrary(" + productID + ")");
        yield return new WaitForEndOfFrame();
        foreach (var item in referenceAssetsToDownload)
        {
            AsyncOperationHandle assetReference = Addressables.DownloadDependenciesAsync(item, true);
            assetReference.Completed += AssetCompleted;
        }
    }
    void AssetCompleted(AsyncOperationHandle asseth)
    {
        if (asseth.Status == AsyncOperationStatus.Succeeded)
        {
            progress += partSection;
            section++;
            filler.fillAmount = progress;
            if (section == referenceAssetsToDownload.Count)
                StartCoroutine(DowloadPrefabs());
        }
    }
    IEnumerator DowloadPrefabs()
    {
        yield return new WaitForEndOfFrame();
        foreach (var item in prefabsAssetsToDownload)
        {
            AsyncOperationHandle<GameObject> prefabHandle = Addressables.LoadAssetAsync<GameObject>(item);
            prefabHandle.Completed += PrefabDownloadComplete;
        }
    }
    void PrefabDownloadComplete(AsyncOperationHandle<GameObject> gObject)
    {
        if(gObject.Status == AsyncOperationStatus.Succeeded)
        {
            progress += partSection;
            section++;
            filler.fillAmount = progress;
            GameObject prefab = gObject.Result;
            string id = prefab.GetComponent<AkaSelectable>().variantID;
            AkaCache.LoadedPrefabs.Add(id, prefab);
            if (section == referenceAssetsToDownload.Count + prefabsAssetsToDownload.Count)
                PrefabToAR();
        }
    }
    void PrefabToAR()
    {
        AkaStore akaStore = FindObjectOfType<AkaStore>();
        akaStore.realidadAumentadaEjecutandose = true;
        if (newObject)
            akaStore.akaARManager.AddThisFittment(AkaCache.LoadedPrefabs[fittmentID]);
        else
            akaStore.akaARManager.ReplaceFittment(AkaCache.LoadedPrefabs[fittmentID]);
    }
}
