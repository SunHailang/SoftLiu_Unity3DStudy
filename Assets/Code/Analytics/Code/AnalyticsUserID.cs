using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu.Analytics
{
    public class AnalyticsUserID
    {
        public static string GenerateUserID()
        {
            // Generate a GUID so that we can identify users over the course of firing multiple events etc.
            return System.Guid.NewGuid().ToString();
        }
    }
}
