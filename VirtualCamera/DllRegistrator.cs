using System.Runtime.InteropServices;

namespace VirtualCamera
{
    internal partial class DllRegistrator : IDisposable
    {
        // All COM DLLs must export the DllRegisterServer()
        // and the DllUnregisterServer() APIs for self-registration/unregistration.
        // They both have the same signature and so only one
        // delegate is required.
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint DllRegUnRegApi();

        [LibraryImport("Kernel32.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        private static partial IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string strLibraryName);

        [LibraryImport("Kernel32.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        private static partial int FreeLibrary(IntPtr hModule);

        [LibraryImport("Kernel32.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        private static partial IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        private IntPtr _hModuleDll;

        // Load the DLL.
        public DllRegistrator(string dllName)
        {
            _hModuleDll = LoadLibrary(dllName);

            if (_hModuleDll == IntPtr.Zero)
            {
                throw new Exception($"Unable to load DLL : {dllName}");
            }
        }

        public bool Register()
        {
            // Obtain the required exported API.
            var pExportedFunction = GetProcAddress(_hModuleDll, "DllRegisterServer");

            if (pExportedFunction == IntPtr.Zero)
            {
                Console.WriteLine("Unable to get required API from DLL.");
                return false;
            }

            // Obtain the delegate from the exported function
            var pDelegateRegUnReg = (DllRegUnRegApi)Marshal.GetDelegateForFunctionPointer(pExportedFunction, typeof(DllRegUnRegApi));

            // Invoke the delegate.
            var hResult = pDelegateRegUnReg();

            return hResult == 0;
        }

        public bool DeRegister()
        {
            // Obtain the required exported API.
            var pExportedFunction = GetProcAddress(_hModuleDll, "DllUnregisterServer");

            if (pExportedFunction == IntPtr.Zero)
            {
                Console.WriteLine("Unable to get required API from DLL.");
            }

            // Obtain the delegate from the exported function
            var pDelegateRegUnReg = (DllRegUnRegApi)Marshal.GetDelegateForFunctionPointer(pExportedFunction, typeof(DllRegUnRegApi));

            // Invoke the delegate.
            var hResult = pDelegateRegUnReg();

            return hResult == 0;
        }

        public void Dispose()
        {
            if (_hModuleDll != IntPtr.Zero)
            {
                DeRegister();
                FreeLibrary(_hModuleDll);
                _hModuleDll = IntPtr.Zero;
            }
        }
    }
}
