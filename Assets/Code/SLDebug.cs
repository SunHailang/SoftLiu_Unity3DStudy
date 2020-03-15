using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu
{
    public static class SLDebug
    {

        public static void TextPopup(string text, Transform parent, Vector3 point, Vector3 scale, Color color)
        {
            DebugTextPrefab textPrefab = Resources.Load<DebugTextPrefab>("DebugPrefabs/DebugTextPrefab");
            Assert.Fatal(textPrefab != null, "TextPopup is null.");
            textPrefab = GameObject.Instantiate(textPrefab, parent);
            textPrefab.Configuer(text, scale, color);
        }

    }
}
