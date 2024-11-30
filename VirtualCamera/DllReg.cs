using System.ComponentModel;
using System.Runtime.InteropServices;

public class DllReg : IDisposable
{
    private IntPtr hLib;

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    private delegate int PointerToMethodInvoker();

    public DllReg(string filePath)
    {
        hLib = LoadLibrary(filePath);
        if (IntPtr.Zero == hLib)
        {
            var errno = Marshal.GetLastWin32Error();
            throw new Win32Exception(errno, "Failed to load library.");
        }
    }

    public bool RegisterComDLL()
    {
        return CallPointerMethod("DllRegisterServer");
    }

    public bool UnRegisterComDLL()
    {
        return CallPointerMethod("DllUnregisterServer");
    }

    private bool CallPointerMethod(string methodName)
    {
        IntPtr dllEntryPoint = GetProcAddress(hLib, methodName);
        if (IntPtr.Zero == dllEntryPoint)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        PointerToMethodInvoker drs =
               (PointerToMethodInvoker)Marshal.GetDelegateForFunctionPointer(dllEntryPoint,
                           typeof(PointerToMethodInvoker));
        return drs() == 0;
    }

    public void Dispose()
    {
        if (hLib != IntPtr.Zero)
        {
            UnRegisterComDLL();
            FreeLibrary(hLib);
            hLib = IntPtr.Zero;
        }
    }
}

//
// How to Use
//
/*using (DllReg dllReg = new DllReg("path\\to\\com.dll"))
{
    dllReg.RegisterComDLL();
    return base.Execute();
}*/
