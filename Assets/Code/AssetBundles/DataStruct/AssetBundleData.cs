using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    [CreateAssetMenu]
    public class AssetBundleData : ScriptableObject, IAssetBundleData
    {
        [SerializeField]
        private Bundle[] m_bundles;

        public Bundle[] Bundles { get { return m_bundles; } set { m_bundles = value; } }

    }
}
