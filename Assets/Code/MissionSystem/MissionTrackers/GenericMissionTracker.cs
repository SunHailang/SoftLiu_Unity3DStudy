using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoftLiu.MissionSystem
{
    public class GenericMissionTracker : MissionTracker
    {

        protected int m_currentValue;
        protected int m_targetValue;

        public int currentValue { get { return m_currentValue; } }
        public int targetValue { get { return m_targetValue; } }

        public override float progress { get { return (float)m_currentValue / (float)m_targetValue; } }

        public GenericMissionTracker() : this(null, MissionCategory.maxTypes) { }

        public GenericMissionTracker(GenericMissionData data, MissionCategory category) : base(data)
        {

        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public override void OnSave()
        {
            base.OnSave();
        }

    }
}
