using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Analytics
{
    // Main delegate to call analytics
    public delegate void AnalyticsEventDelegate(System.Enum eventType, Dictionary<string, object> optParams);
}
