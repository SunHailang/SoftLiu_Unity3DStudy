using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Save.Errors
{
    public enum ErrorState
    {
        Authentication,
        WrongUser,
        Connection,
        Corruption,
        SyncFailed,
        UpgradeNeeded,
        UpgradeNeededAvailable,
        UpgradeNeededLocalCorrupt,
        UpgradeNeededAvailableLocalCorrupt,
        PermissionError,
        UploadFailed,
        SaveError,
        SaveLock,
        None
    }

    public enum ErrorCodes
    {
        Unset = 0,

        #region Server Defined Errors
        UnknownError = -1,
        ParamError = -2,
        ValidationError = -3,
        SDKError = -4,
        UserError = -5,
        ConfigError = -6,
        AuthError = -7,
        CompatibilityError = -8,
        UploadDisallowedError = -9,
        MaintanenceError = -10,
        SaveLockedError = -25,
        #endregion

        ClientConnectionTimeout = -11,
        ClientConnectionError = -12,
        ServerConnectionTimeout = -13,
        ServerConnectionError = -14,
        InvalidResponseError = -15,
        LoginError = -16,
        PermissionError = -17,
        FileNotFoundError = -18,
        FilePermissionError = -19,
        CorruptedFileError = -20,
        SaveError = -21,
        S3TokenInvalid = -22,
        UserAuthError = -23,
        DnsHostResolveError = -24,
        S3InvalidUploadIntegrity = -26
    }
    public class Error
    {
        protected string m_message = null;
        protected ErrorCodes m_code = ErrorCodes.Unset;

        private string m_stackTrace = null;

        public Error(string message, ErrorCodes code)
        {
            m_message = message;
            m_code = code;
            m_stackTrace = System.Environment.StackTrace;
        }

        public string message
        {
            get { return m_message; }
        }

        public ErrorCodes code
        {
            get { return m_code; }
        }

        public string stackTrace
        {
            get { return m_stackTrace; }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", this.GetType(), m_message, m_code);
        }
    }


    public class SyncError : Error
    {
        public SyncError(string message = null, ErrorCodes code = ErrorCodes.Unset)
            : base(message != null ? message : "Synchronization Error", code == ErrorCodes.Unset ? ErrorCodes.UnknownError : code)
        {
        }
    }

    public class UploadDisallowedError : Error
    {
        public UploadDisallowedError(string message = null, ErrorCodes code = ErrorCodes.Unset)
            : base(message != null ? message : "Upload Disallowed Error", code == ErrorCodes.Unset ? ErrorCodes.UploadDisallowedError : code)
        {
        }
    }

    #region Exceptions
    public class CorruptedSaveException : Exception
    {
        public CorruptedSaveException(Exception innerException)
            : base("Save Corrupted", innerException)
        {
            Debug.LogError("CORRUPTEDSAVEEXCEPTION!! SAVE FAILED LOADING!! THIS IS A CRITICAL ISSUE THAT NEEDS RESOLVING IMMEDIATELY.");
        }
    }
    #endregion
}
