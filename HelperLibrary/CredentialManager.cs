
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary {
    static public class CredentialManager {

        static public bool WriteCreds(string credentialsName, string username, string password) {
            return WriteCreds(credentialsName, new NetworkCredential() { UserName = username, Password = password });
        }

        static public bool WriteCreds(string credentialsName, string username, SecureString password) {
            return WriteCreds(credentialsName, new NetworkCredential() { UserName = username, SecurePassword = password });
        }

        static public bool WriteCreds(string credentialsName, NetworkCredential networkCredential) {
            var cred = ConvertFrom(credentialsName, networkCredential);
            return CredWrite(ref cred, 0);
        }

        static public NetworkCredential ReadCreds(string credentialsName) {
            IntPtr credPointer;
            var result = CredRead(credentialsName, 1, 0, out credPointer);
            var networkCred = new NetworkCredential() { UserName = "", Password = "" };

            if (result == true) {
                var nativeCred = Marshal.PtrToStructure<NativeCredential>(credPointer);
                networkCred.UserName = nativeCred.UserName;
                networkCred.Password = nativeCred.CredentialBlobSize > 1 ? Marshal.PtrToStringUni(nativeCred.CredentialBlob, (int)nativeCred.CredentialBlobSize / 2) : "";
            }

            CredFree(credPointer);
            return networkCred;
        }

        static public (string username, string password) ReadCredsString(string credentialsName) {
            var networkCred = ReadCreds(credentialsName);
            return (networkCred.UserName, networkCred.Password);
        }

        static public (string username, SecureString password) ReadCredsSecureString(string credentialsName) {
            var networkCred = ReadCreds(credentialsName);
            return (networkCred.UserName, networkCred.SecurePassword);
        }

        static private NativeCredential ConvertFrom(string credentialsName, NetworkCredential networkCredential) {
            var nativeCred = new NativeCredential() {
                TargetName = credentialsName,
                UserName = networkCredential.UserName,
                CredentialBlob = Marshal.StringToCoTaskMemUni(networkCredential.Password),
                CredentialBlobSize = (UInt32)Encoding.Unicode.GetBytes(networkCredential.Password).Length,
                Type = 1,   //1 - GENERIC
                Persist = 2 //2 - LOCAL_MACHINE
            };
            return nativeCred;
        }

        #region SYSTEM CREDENTIALS
        //Write cred
        [DllImport("Advapi32.dll", SetLastError = true, EntryPoint = "CredWriteW", CharSet = CharSet.Unicode)]
        private static extern bool CredWrite([In] ref NativeCredential userCredential, [In] UInt32 flags);
        //Read cred
        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredRead([MarshalAs(UnmanagedType.LPWStr)] string target, uint type, int reservedFlag, out IntPtr CredentialPtr);
        //Cred Free
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool CredFree([In] IntPtr buffer);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NativeCredential {
            public UInt32 Flags;
            public UInt32 Type;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string TargetName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public IntPtr CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string TargetAlias;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string UserName;
        }
        #endregion SYSTEM CREDENTIALS
    }
}

