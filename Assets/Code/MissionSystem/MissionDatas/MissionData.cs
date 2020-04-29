using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.MissionSystem
{
    public enum MissionCategory
    {
        maxTypes,
    }

    public enum MissionType
    {
        None,
    }

    public class MissionData
    {
        protected MissionType m_type;


        // reward
        protected string m_rewardType;
        protected int m_rewardAmount;

        public MissionData() : this(MissionType.None, MissionCategory.maxTypes)
        {

        }

        public MissionData(MissionType type, MissionCategory category)
        {

        }

        public virtual string GetMissionDescription()
        {
            return "";
        }

        public virtual MissionTracker CreateTracker(MissionCategory category)
        {
            return new MissionTracker(this);
        }

        public virtual void OnSave()
        {

        }

        public virtual void OnLoad()
        {

        }

    }
}
