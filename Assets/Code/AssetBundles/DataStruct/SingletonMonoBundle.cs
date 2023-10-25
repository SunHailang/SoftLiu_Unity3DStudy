using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoftLiu.AssetBundles
{
    [CreateAssetMenu]
    public class SingletonMonoBundle : Bundle
    {
        [SerializeField]
        private string m_stateName;
        public string stateName { get { return m_stateName; } }

        public void InitData(string assetBundleName, string[] assets, string stateName)
        {
            base.InitData(assetBundleName, assets);
            m_stateName = stateName;
        }
        

        public void UpdateBundleData(string[] assets)
        {
            m_assetPaths = assets;
        }

        public override void EnableBundle()
        {
#if UNITY_EDITOR
            //first, wipe any assets in that bundle
            // UnityEditor.AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
            // for (int i = 0; i < assetPaths.Length; i++)
            // {
            //     //assign to bundle
            //     UnityEditor.AssetImporter.GetAtPath(assetPaths[i]).assetBundleName = assetBundleName;
            // }
#endif
        }

        public override void DisableBundle()
        {
#if UNITY_EDITOR
            // UnityEditor.AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
#endif
        }

    }
}
