using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Analytics
{
    public abstract class AnalyticsProvider
    {
        private Dictionary<Enum, AnalyticsEventDelegate> m_supportedEvents = new Dictionary<Enum, AnalyticsEventDelegate>();

        public virtual void InitAnalyticsProvider(AnalyticsInitialisationParams initParams)
        {

        }

        public void AddSupportedEvent(Enum eventType, AnalyticsEventDelegate listener)
        {
            m_supportedEvents.Add(eventType, listener);
        }

        public Dictionary<Enum, AnalyticsEventDelegate> GetSupportedEvents()
        {
            return m_supportedEvents;
        }

        public virtual void StartSession()
        {

        }

        public virtual void EndSession()
        {

        }

        public virtual string GetUserID()
        {
            return string.Empty;
        }
    }
}
