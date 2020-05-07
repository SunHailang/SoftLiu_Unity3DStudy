using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoLocation
{
    public enum Location
    {
        China,
    }

    private static Location m_location = Location.China;


    public static Location location
    {
        get { return m_location; }
    }
}
