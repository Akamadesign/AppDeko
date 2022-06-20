using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MyAppManager : MonoBehaviour
{
    public static MyAppManager manager;
    public string fittmentToLoad;
    private void Awake()
    {
        if (!manager)
        {
            manager = this;
            gameObject.name = "AprobedManager";
        }
        if (manager == this)
            DontDestroyOnLoad(gameObject);
    }
    public void LoadThisScene(string sceneName)
    {
        SceneManager.LoadScene("LoadScene");
        StartCoroutine(WaitAndLoadFinalScene(sceneName));
    }
    IEnumerator WaitAndLoadFinalScene(string scene)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(scene);
    }
}
