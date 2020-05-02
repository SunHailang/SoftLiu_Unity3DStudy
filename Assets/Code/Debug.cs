using System.Diagnostics;
using UnityEngine;


public static class Debug
{

#if !ENABLE_LOG || PRODUCTION
    [Conditional("FALSE")]
#endif
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message.ToString());

    }

#if !ENABLE_LOG || PRODUCTION
	[Conditional("FALSE")]
#endif
    public static void Log(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

#if !ENABLE_LOG || PRODUCTION
	[Conditional("FALSE")]
#endif
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message.ToString());
    }

#if !ENABLE_LOG || PRODUCTION
	[Conditional("FALSE")]
#endif
    public static void LogWarning(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogWarning(message.ToString(), context);
    }
#if !ENABLE_LOG || PRODUCTION
	[Conditional("FALSE")]
#endif
    public static void LogException(System.Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

#if !ENABLE_LOG || PRODUCTION
	[Conditional("FALSE")]
#endif
    public static void LogException(System.Exception exception, Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }
}
