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


    }
}
