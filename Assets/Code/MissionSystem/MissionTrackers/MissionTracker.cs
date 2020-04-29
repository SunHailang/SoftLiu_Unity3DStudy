using System;
namespace SoftLiu.MissionSystem
{
    public class MissionTracker : IDisposable
    {
        protected MissionData m_data;

        protected bool m_isCompleted;

        protected bool m_isValid = false;

        public virtual float progress { get { return 0; } }

        public virtual bool isCompleted { get { return m_isCompleted; } }

        public MissionTracker(MissionData data)
        {
            m_data = data;
        }

        public virtual void Reset() { }
        public virtual void OnSave() { }
        public virtual void OnLoad() { }
        public virtual void OnGameStart() { }
        public virtual void OnGameUpdate() { }
        public virtual void OnGameEnd() { }

        public void Dispose() { }
    }
}
