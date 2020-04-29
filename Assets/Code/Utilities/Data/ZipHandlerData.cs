using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Utils.ZipData
{
    public class ZipHandlerData : IDisposable
    {
        public long fileCount = 0;
        public long fileCurrentCount = 0;

        private bool m_isDone = false;
        public bool isDone
        {
            get { return m_isDone; }
            set { m_isDone = value; }
        }

        private bool m_isError = false;
        public bool isError
        {
            get { return m_isError; }
            set { m_isError = value; }
        }

        private string m_errorText = string.Empty;
        public string errorText
        {
            get { return m_errorText; }
            set { m_errorText = value; }
        }

        public float progress
        {
            get
            {
                if (fileCount <= 0)
                {
                    return 0;
                }
                return (fileCurrentCount * 1.0f / fileCount);
            }
        }

        public void Dispose()
        {

        }
    }
}
