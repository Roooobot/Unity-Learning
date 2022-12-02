using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HotUpdateManager : MonoBehaviour
{
    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();
    public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    private void Start() => DownLoadAssembly(DownLoadHotUpdate);

    private async void DownLoadAssembly(Action action)
    {
        TextAsset[] aotDllBytes = new TextAsset[AOTMetaAssemblyNames.Count];
        for(int i = 0; i < AOTMetaAssemblyNames.Count; i++)
        {
            aotDllBytes[i] =await Addressables.LoadAssetAsync<TextAsset>(AOTMetaAssemblyNames[i]).Task;
            s_assetDatas.Add(AOTMetaAssemblyNames[i], aotDllBytes[i].bytes);
        }
        action?.Invoke();
    }

    private async void DownLoadHotUpdate()
    {
        LoadMetaDataForAOTAssembly();
#if UNITY_EDITOR
        var gameAss = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "HotUpdate");
#else
        TextAsset hotUpdateDll =await Addressables.LoadAssetAsync<TextAsset>("HotUpdate.dll").Task;
        Assembly assembly = Assembly.Load(hotUpdateDll.bytes);
#endif
        Addressables.InstantiateAsync("Hotfix", transform.position, Quaternion.identity);
    }

    private void LoadMetaDataForAOTAssembly()
    {
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach(var aotDllName in AOTMetaAssemblyNames)
        {
            byte[] dllBytes =s_assetDatas[aotDllName];
            LoadImageErrorCode error = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"Assembly{aotDllName}---Mode:{mode}---State{error}");
        }
    }
}
