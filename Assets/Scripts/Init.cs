using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    Button button1;
    Button button2;
    Button button3;
    public List<AssetReference> level = new List<AssetReference>();
    public AsyncOperationHandle<SceneInstance> loadHandle;
    void Start()
    {
        button1 = transform.GetChild(1).GetComponent<Button>();
        button2 = transform.GetChild(2).GetComponent<Button>();
        button3 = transform.GetChild(3).GetComponent<Button>();
        button1.onClick.AddListener(LoadL1);
    }

    public void StartDownloadScene(int index)
    {

    }

    //public void LoadL1()
    //{
    //    loadHandle = Addressables.LoadSceneAsync(level[0],LoadSceneMode.Single);
    //}
    public void LoadL1()
    {
        loadHandle = Addressables.LoadSceneAsync(level[1], LoadSceneMode.Single);
    }

    public void OnDestroy()
    {
        Addressables.UnloadSceneAsync(loadHandle);
    }

}
