using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ScreenUIAdaptation))]
public class ScreenUIAdaptationEditor : Editor
{
    private ScreenUIAdaptation m_screenUI;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
        {
            foreach (var item in targets)
            {
                m_screenUI = (ScreenUIAdaptation) item;
                if (m_screenUI != null)
                {
                    RectTransform trans = m_screenUI.GetComponent<RectTransform>();
                    if (trans != null)
                    {
                        trans.sizeDelta = new Vector2(-m_screenUI.m_width, -m_screenUI.m_height);
                    }
                }
            }
        }
    }
}