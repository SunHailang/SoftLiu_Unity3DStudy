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
            m_implementation = new NativeImplementationEditor();
        }
    }
}
