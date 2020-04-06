using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Plugins.Native
{
    public class NativeBinding
    {
        private static readonly INativeImplementation m_implementation = null;

        public static INativeImplementation Instance { get { return m_implementation; } }

        private NativeBinding()
        {

        }

        static NativeBinding()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            m_implementation = new NativeImplementationAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
            m_implementation = new NativeImplementationIOS();
#else
            m_implementation = new NativeImplementationEditor();
#endif
        }
    }
}
