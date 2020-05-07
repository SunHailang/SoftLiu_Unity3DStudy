using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerDNS
{
    public static Dictionary<string, Dictionary<string, Dictionary<GeoLocation.Location, string>>> dnsRecords = new Dictionary<string, Dictionary<string, Dictionary<GeoLocation.Location, string>>>()
    {
        {
            "HLSUN",new Dictionary<string, Dictionary<GeoLocation.Location, string>>()
            {
                {
                    "development", new Dictionary<GeoLocation.Location, string>()
                    {
                        { GeoLocation.Location.China, "" },
                    }
                },
                {
                    "preproduction", new Dictionary<GeoLocation.Location, string>()
                    {
                        { GeoLocation.Location.China, "" },
                    }
                },
                {
                    "production", new Dictionary<GeoLocation.Location, string>()
                    {
                        { GeoLocation.Location.China, "" },
                    }
                },
            }
        },
        {
            "Event",new Dictionary<string, Dictionary<GeoLocation.Location, string>>()
            {
                {
                    "development", new Dictionary<GeoLocation.Location, string>()
                    {
                        { GeoLocation.Location.China, "" },
                    }
                },
                {
                    "preproduction", new Dictionary<GeoLocation.Location, string>()
                    {
                        { GeoLocation.Location.China, "" },
                    }
                },
                {
                    "production", new Dictionary<GeoLocation.Location, string>()
                    {
                        { GeoLocation.Location.China, "" },
                    }
                },
            }
        }
    };
}
