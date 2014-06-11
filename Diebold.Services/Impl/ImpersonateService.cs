using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Configuration;
using Diebold.Services.Contracts;

namespace Diebold.Services.Impl
{
    public class ImpersonateService : IImpersonateService
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string AuthenticationType { get; set; }
        
        public ImpersonateService()
        {
            UserName = ConfigurationManager.AppSettings["FileServerUserName"];
            Password = ConfigurationManager.AppSettings["FileServerPassword"];
            Domain = ConfigurationManager.AppSettings["FileServerDomain"];
            AuthenticationType = ConfigurationManager.AppSettings["FileServerAuthenticationType"];
        }

        int LOGON32_LOGON_INTERACTIVE = 2;
        int LOGON32_LOGON_NETWORK = 3;
        int LOGON32_LOGON_BATCH = 4;
        int LOGON32_LOGON_SERVICE = 5;
        int LOGON32_LOGON_UNLOCK = 7;
        int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        int LOGON32_PROVIDER_DEFAULT = 0;
        int LOGON32_PROVIDER_WINNT35 = 1;
        int LOGON32_PROVIDER_WINNT40 = 2;
        int LOGON32_PROVIDER_WINNT50 = 3;

        WindowsImpersonationContext impersonationContext;
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int LogonUserA(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

        public static extern int DuplicateToken(IntPtr ExistingTokenHandle, int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

        public static extern bool RevertToSelf();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern long CloseHandle(IntPtr handle);

        public bool impersonateValidUser()
        {
            bool functionReturnValue = false;

            WindowsIdentity tempWindowsIdentity = null;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            functionReturnValue = false;

            if (AuthenticationType == "domain")
            {
                if (RevertToSelf())
                {
                    UserName = Decrypt(UserName);
                    Password = Decrypt(Password);
                    if (LogonUserA(UserName, Domain, Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                            if ((impersonationContext != null))
                            {
                                functionReturnValue = true;
                            }
                        }
                    }
                }
            }
            else if (AuthenticationType == "network")
            {
                if (RevertToSelf())
                {
                    UserName = Decrypt(UserName);
                    Password = Decrypt(Password);
                    if (LogonUserA(UserName, Domain, Password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                            if ((impersonationContext != null))
                            {
                                functionReturnValue = true;
                            }
                        }
                    }
                }
            }
            else if (AuthenticationType == "service")
            {
                if (RevertToSelf())
                {
                    UserName = Decrypt(UserName);
                    Password = Decrypt(Password);
                    if (LogonUserA(UserName, Domain, Password, LOGON32_LOGON_SERVICE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                            if ((impersonationContext != null))
                            {
                                functionReturnValue = true;
                            }
                        }
                    }
                }
            }
            else if (AuthenticationType == "local")
            {
                if (RevertToSelf())
                {
                    UserName = Decrypt(UserName);
                    Password = Decrypt(Password);
                    if (LogonUserA(UserName, Domain, Password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                            if ((impersonationContext != null))
                            {
                                functionReturnValue = true;
                            }
                        }
                    }
                }
            }


            if (!tokenDuplicate.Equals(IntPtr.Zero))
            {
                CloseHandle(tokenDuplicate);
            }
            if (!token.Equals(IntPtr.Zero))
            {
                CloseHandle(token);
            }
            return functionReturnValue;
        }

        public void undoImpersonation()
        {
            impersonationContext.Undo();
        }

        private string Decrypt(string cipherText)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
        }
    }
}
