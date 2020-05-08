using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu.Analytics
{
    public class AnalyticsInitialisationParams
    {
        //  Initialization params for analytics system
        //  Providers enabled for this project
        public List<AnalyticsProvider> providers = new List<AnalyticsProvider>();

        //  Default state
        public AnalyticsState state = AnalyticsState.ANALYTICS_ENABLED;

        //  User ID
        public string userID = null;

        //  Is analytics in test mode
        public bool testMode = false;
    }
}
