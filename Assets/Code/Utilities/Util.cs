//--------------------------------------------------------------------------------
// Util.cs
//--------------------------------------------------------------------------------
// This is a home for some static functions and constants etc.
//--------------------------------------------------------------------------------

using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class Util
{
    //----------------------------------------------------------------------------
    // Unity GameObject/Component helper functions
    //----------------------------------------------------------------------------

    // FindSubObject is a recursive version of Transform.Find(), it will find a child
    // object by name, returning the first one encountered (searches depth first, does
    // not necessarily find closest one to the root, which maybe we want?)

    public static Transform FindSubObjectRecursive(this Transform trans, string name)
    {
        if (trans.name == name)
            return trans;

        foreach (Transform t in trans)
        {
            Transform found = t.FindSubObjectRecursive(name);
            if (found != null)
                return found;
        }
        return null;
    }

    public static Transform FindSubObjectRecursiveNameContains(this Transform trans, string name)
    {
        if (trans.name.Contains(name))
            return trans;

        foreach (Transform t in trans)
        {
            Transform found = t.FindSubObjectRecursiveNameContains(name);
            if (found != null)
                return found;
        }
        return null;
    }

    public static GameObject FindSubObject(this Component comp, string name)            // on component, return object
    {
        Transform t = comp.transform.FindSubObjectRecursive(name);
        return (t == null) ? null : t.gameObject;
    }

    public static GameObject FindSubObject(this GameObject obj, string name)            // on object, return object
    {
        Transform t = obj.transform.FindSubObjectRecursive(name);
        return (t == null) ? null : t.gameObject;
    }

    public static GameObject FindSubObjectNameContains(this GameObject obj, string name)            // on object, return object
    {
        Transform t = obj.transform.FindSubObjectRecursiveNameContains(name);
        return (t == null) ? null : t.gameObject;
    }

    public static Transform FindSubObjectTransform(this Component comp, string name)    // on component, return transform
    {
        return comp.transform.FindSubObjectRecursive(name);
    }
    public static Transform FindSubObjectTransform(this GameObject obj, string name)    // on object, return transform
    {
        return obj.transform.FindSubObjectRecursive(name);
    }
    public static T FindComponentInParents<T>(this GameObject obj) where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp == null)
        {
            // recurse
            Transform parent = obj.transform.parent;
            if (parent != null)
            {
                return parent.gameObject.FindComponentInParents<T>();
            }
        }
        return comp;
    }
    // ReparentChild - an extension method for Transform, to find a child object and attach it to a different child object.
    // Often need this for things like attach points and child collision etc. on prefabs.  e.g. to have a collider on a character's
    // head, it's safer to stick it on an object parented to the root, and then attach it to the head at runtime (the prefab is
    // less likely to randomly screw up if the model is updated).
    public static void ReparentChild(this Transform t, string childName, string newParentName)
    {
        Transform child = t.FindSubObjectTransform(childName);
        Transform newParent = t.FindSubObjectTransform(newParentName);
        child.parent = newParent;
    }

    // version where we already have transform ref for the child
    public static void ReparentChild(this Transform t, Transform child, string newParentName)
    {
        Transform newParent = t.FindSubObjectTransform(newParentName);
        child.parent = newParent;
    }

    // version where we already have transform ref for the parent
    public static void ReparentChild(this Transform t, string childName, Transform newParent)
    {
        Transform child = t.FindSubObjectTransform(childName);
        child.parent = newParent;
    }

    // Get component if it exists, otherwise add it and return the newly added one
    public static T ForceGetComponent<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() ?? obj.AddComponent<T>();
    }

    public static T ForceGetComponent<T>(this Component comp) where T : Component
    {
        return comp.GetComponent<T>() ?? comp.gameObject.AddComponent<T>();
    }

    // disable a component of type T if it exists (technically Behaviour rather than component, it has to have the 'enabled' property)
    public static void DisableComponent<T>(this GameObject obj) where T : Behaviour
    {
        T comp = obj.GetComponent<T>();
        if (comp != null)
            comp.enabled = false;
    }

    public static void DisableCollidersInChildren(this GameObject obj)
    {
        foreach (Collider coll in obj.GetComponentsInChildren<Collider>())
            coll.enabled = false;
    }

    public static void EnableCollidersInChildren(this GameObject obj)
    {
        foreach (Collider coll in obj.GetComponentsInChildren<Collider>())
            coll.enabled = true;
    }


    // GetFirstComponentInChildren is a version of GetComponentInChildren that finds the one closest
    // to the root of the hierarchy.
    public static T GetFirstComponentInChildren<T>(this Component comp) where T : Component
    {
        Transform trans = comp.transform;

        // first see if it's on the root component
        T t = comp.GetComponent<T>();
        if (t != null)
            return t;

        // next see if it can be found at the first child level
        foreach (Transform tr in trans)
        {
            t = tr.GetComponent<T>();
            if (t != null)
                return t;
        }

        // haven't found it, now we'll search the entire hierarchy
        // and if we find any, find the least deep one.
        //
        // The first 2 checks are not necessary, just an optimization
        // to stop us trawling through the entire hierarchy every time
        // because this is a bit crap.
        T[] ts = comp.GetComponentsInChildren<T>();
        if ((ts == null) || (ts.Length == 0))
            return null;

        // found at least one, now search for the one nearest the root
        T bestOne = null;
        int bestDepth = 9999;
        foreach (T test in ts)
        {
            Transform tr = test.transform;
            int depth = 0;
            while (tr != trans)             // count how many parents to step through before we get back to the root
            {
                depth++;
                tr = tr.parent;
            }
            if (depth < bestDepth)
            {
                bestDepth = depth;
                bestOne = test;
            }
        }
        return bestOne;
    }

    public static T GetFirstComponentInChildren<T>(this GameObject obj) where T : Component
    {
        return obj.transform.GetFirstComponentInChildren<T>();
    }

    public static T[] GetInterfacesInChildren<T>(this GameObject obj)
    {
        if (!typeof(T).IsInterface) throw new System.SystemException("Specified type is not an interface!");
        MonoBehaviour[] mObjs = obj.GetComponents<MonoBehaviour>();
        return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
    }


    // extension methods for Transform, for setting individual components of position etc
    public static float SetPosX(this Transform t, float x)
    {
        Vector3 v = t.position;
        v.x = x;
        t.position = v;
        return x;   // return the value we set, for convenience
    }
    public static float SetPosY(this Transform t, float y)
    {
        Vector3 v = t.position;
        v.y = y;
        t.position = v;
        return y;
    }
    public static float SetPosZ(this Transform t, float z)
    {
        Vector3 v = t.position;
        v.z = z;
        t.position = v;
        return z;
    }
    public static float SetLocalScale(this Transform t, float s)
    {
        t.localScale = new Vector3(s, s, s);
        return s;
    }

    public static void CopyFrom(this Transform t, Transform from, bool includeScale = true)
    {
        t.position = from.position;
        t.rotation = from.rotation;
        if (includeScale)
            t.localScale = from.localScale;
    }

    // extension method to replace ParticleSystem.IsAlive(), which doesn't work properly on emitters that go off-screen
    public static bool IsReallyAlive(this ParticleSystem ps)
    {
#if UNITY_5_3_OR_NEWER
        return (ps.particleCount != 0) || ps.emission.enabled;
#else
        return (ps.particleCount != 0) || ps.enableEmission;
#endif
    }

    // doesn't look like System.Type has this. Using it to make slightly less messy in spawner preview stuff
    public static bool IsSameOrSubclassOf(this System.Type t, System.Type other)
    {
        return (t == other) || t.IsSubclassOf(other);
    }

    public static string ColorToHex(Color32 color)
    {
        return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

    public static Color HexToColor(string hex)
    {
        return new Color32(
            byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
            255);
    }

    public static Color HexToColor(string hex, byte alpha)
    {
        Color32 c = HexToColor(hex);
        c.a = alpha;
        return c;
    }

    #region NGUI Helper methods
    //Enables-Disables the colliders in a certain layer. You can pass a list of objects to be ignored.
    public static void SetInteractablesEnabled(GameObject root, bool enabled, string[] layers, List<GameObject> exceptions = null)
    {
        Collider[] interactables = root.CollectCollidersEnabled(!enabled, layers == null ? null : layers);

        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i] != null)
            {
                bool exceptionFound = false;
                if (exceptions != null)
                {
                    for (int j = 0; j < exceptions.Count; ++j)
                    {
                        if (exceptions[j] == interactables[i].gameObject)
                        {
                            exceptionFound = true;
                        }
                    }
                }

                //Only enable/disable the colliders that are not found in the exceptions list
                if (exceptionFound == false)
                {
                    interactables[i].enabled = enabled;
                }
            }
        }
    }

    public static void SetUiInteractablesEnabled(GameObject root, bool enabled, List<GameObject> exceptions = null)
    {
        SetInteractablesEnabled(root, enabled, new string[] { "UI", "BackgroundUI" }, exceptions);
    }


    private static Collider[] CollectCollidersEnabled(this GameObject obj, bool enabled, string[] layers = null)
    {
        List<Collider> collidersList = new List<Collider>();

        if (obj == null)
        {
            return collidersList.ToArray();
        }


        Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null)
                continue;
            if (colliders[i].enabled == enabled)
            {
                bool layerOk = layers == null;
                if (!layerOk)
                {
                    for (int j = 0; j < layers.Length; j++)
                    {
                        if (colliders[i].gameObject.layer == LayerMask.NameToLayer(layers[j]))
                        {
                            layerOk = true;
                            break;
                        }
                    }
                }
                if (layerOk)
                {
                    collidersList.Add(colliders[i]);
                }
            }
        }
        return collidersList.ToArray();
    }
    #endregion

    // Returns a sphere from the bounds of a collider
    public static void GetSphereFromCollider(Collider coll, out Vector3 centre, out float radius)
    {
        Bounds bounds = coll.bounds;
        centre = bounds.center;
        Vector3 extents = bounds.extents;
        float rx = extents.x;
        float ry = extents.y;
        radius = Mathf.Max(rx, ry);
    }

    // Check if enough disk space is available on device, the input size is in Bytes
    public static bool IsDiskSpaceAvailable(string path, int size)
    {
        bool isAvailable = true;

        try
        {
            System.IO.File.WriteAllBytes(path, new byte[size]);
        }
        catch (System.Exception e)
        {
            isAvailable = false;
            Debug.LogError(string.Format("IsDiskSpaceAvailable :: Exception checking for free disk space :: Required size = {0} Bytes :: Required path = {1} :: Exception = {2}", size, path, e.ToString()));
        }
        finally
        {
            // Try to delete the test file no matter the result
            try
            {
                System.IO.File.Delete(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("IsDiskSpaceAvailable :: Exception deleting test file after checking for free disk space :: Required size = {0} Bytes :: Required path = {1} :: Exception = {2}", size, path, e.ToString()));
            }
        }

        return isAvailable;
    }

    // Check the size of file on disk, if the file doesn't exist or there is an error reading the file, -1 will be returned
    // The result size is in Bytes
    public static long GetFileSizeOnDisk(string path)
    {
        long size = -1;

        try
        {
            if (System.IO.File.Exists(path))
            {
                size = new System.IO.FileInfo(path).Length;
            }
        }
        catch (System.Exception e)
        {
            size = -1;
            Debug.LogError(string.Format("GetFileSizeOnDisk :: Exception getting file size :: Path = {0} :: Exception = {1}", path, e.ToString()));
        }

        return size;
    }

    public static YieldInstruction StartCoroutineWithoutMonobehaviour(string name, IEnumerator coroutine)
    {
        GameObject go = new GameObject(name);
        UtilBehavior mono = go.AddComponent<UtilBehavior>();
        return mono.StartCoroutine(HelperCoroutine(coroutine, mono));
    }

    public static GameObject RunCoroutineWithoutMonobehaviour(string name, IEnumerator coroutine)
    {
        GameObject go = new GameObject(name);
        UtilBehavior mono = go.AddComponent<UtilBehavior>();
        mono.StartCoroutine(HelperCoroutine(coroutine, mono));
        return go;
    }

    public static IEnumerator HelperCoroutine(IEnumerator coroutine, MonoBehaviour helperObject)
    {
        MonoBehaviour.DontDestroyOnLoad(helperObject.gameObject);
        yield return helperObject.StartCoroutine(coroutine);
        MonoBehaviour.Destroy(helperObject.gameObject);
    }

    // http://answers.unity3d.com/questions/959091/how-can-i-make-a-lerp-move-in-an-arc-instead-of-a.html
    public static Vector3 CubeBezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return (((-p0 + 3 * (p1 - p2) + p3) * t + (3 * (p0 + p2) - 6 * p1)) * t + 3 * (p1 - p0)) * t + p0;
    }

    // https://forum.unity.com/threads/calculatefrustumplanes-without-allocations.371636/
    private static System.Action<Plane[], Matrix4x4> _calculateFrustumPlanes_Imp;
    public static void CalculateFrustumPlanesNonAlloc(Plane[] planes, Matrix4x4 worldToProjectMatrix)
    {
        if (_calculateFrustumPlanes_Imp == null)
        {
            var meth = typeof(GeometryUtility).GetMethod("Internal_ExtractPlanes", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new System.Type[] { typeof(Plane[]), typeof(Matrix4x4) }, null);
            if (meth == null) throw new System.Exception("Failed to reflect internal method. Your Unity version may not contain the presumed named method in GeometryUtility.");

            _calculateFrustumPlanes_Imp = System.Delegate.CreateDelegate(typeof(System.Action<Plane[], Matrix4x4>), meth) as System.Action<Plane[], Matrix4x4>;
            if (_calculateFrustumPlanes_Imp == null) throw new System.Exception("Failed to reflect internal method. Your Unity version may not contain the presumed named method in GeometryUtility.");
        }
        _calculateFrustumPlanes_Imp(planes, worldToProjectMatrix);
    }

    public static string FormatTimeFromTenthsOfASecond(int tenthsOfASecond)
    {
        float seconds = tenthsOfASecond / 10f;
        int minutes = Mathf.FloorToInt(seconds / 60f);
        seconds -= (minutes * 60);
        string text = minutes.ToString("D2") + ":" + seconds.ToString("N1");
        return text;
    }

    public static string GetThreadStatus(Thread thread)
    {
        if (thread == null)
        {
            return "null";
        }
        if (thread.IsAlive)
        {
            return "Alive";
        }
        return "Dead";
    }

    /// <summary>
    ///     Util exists to allow spoofiing.
    /// </summary>
    /// <returns></returns>
    public static NetworkReachability IsInternetReachable()
    {
        var currentReachability = Application.internetReachability;
        return currentReachability;
    }


    // Adding this here to make it easier to use enums in the state parameters rather than strings  
    public static T CastObjectToEnum<T>(object o)
    {
        T enumVal = (T)Enum.Parse(typeof(T), o.ToString());
        return enumVal;
    }

    //	Empty behaviour for Coroutine helper
    class UtilBehavior : MonoBehaviour
    {
    }

}
