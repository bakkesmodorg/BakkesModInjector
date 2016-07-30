using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BakkesModInjector
{
    //Source: evolution536 @http://www.unknowncheats.me/forum/c/82629-basic-c-dll-injector.html
    public enum DLLInjectionResult
    {
        DLL_NOT_FOUND,
        GAME_PROCESS_NOT_FOUND,
        INJECTION_FAILED,
        SUCCESS
    }

    public sealed class DllInjector
    {
        static readonly IntPtr INTPTR_ZERO = IntPtr.Zero;
        static readonly uint desiredAccess = (0x2 | 0x8 | 0x10 | 0x20 | 0x400);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

       private DllInjector() { }

        public DLLInjectionResult Inject(string processName, string dllPath)
        {
            if (!File.Exists(dllPath))
            {
                return DLLInjectionResult.DLL_NOT_FOUND;
            }

            uint processId = 0;

            Process[] processes = Process.GetProcesses();
            foreach(Process p in processes) {
                if(p.ProcessName == processName)
                {
                    processId = (uint)p.Id;
                }
            }
            if (processId == 0) return DLLInjectionResult.GAME_PROCESS_NOT_FOUND;
            if (!injectDLL(processId, dllPath)) return DLLInjectionResult.INJECTION_FAILED;
            return DLLInjectionResult.SUCCESS;
        }

        bool injectDLL(uint processToInject, string dllPath)
        {
            IntPtr processHandle = OpenProcess(desiredAccess, 1, processToInject);

            if (processHandle == INTPTR_ZERO) return false;

            IntPtr loadLibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (loadLibraryAddress == INTPTR_ZERO) return false;

            IntPtr argAddress = VirtualAllocEx(processHandle, (IntPtr)null, (IntPtr)dllPath.Length, (0x1000 | 0x2000), 0X40);

            if (argAddress == INTPTR_ZERO) return false;

            byte[] bytes = Encoding.ASCII.GetBytes(dllPath);

            if (WriteProcessMemory(processHandle, argAddress, bytes, (uint)bytes.Length, 0) == 0)
                return false;

            if (CreateRemoteThread(processHandle, (IntPtr)null, INTPTR_ZERO, loadLibraryAddress, argAddress, 0, (IntPtr)null) == INTPTR_ZERO)
            {
                return false;
            }

            CloseHandle(processHandle);
            return true;
        }

        static DllInjector instance;

        public static DllInjector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DllInjector();
                }
                return instance;
            }
        }
    }
}
