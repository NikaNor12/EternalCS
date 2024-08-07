using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EternalCS
{
    public class MemoryRead
    {
        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        public void Connection()
        {
            Process[] processes = Process.GetProcessesByName(Offsets.ProcessNames);

            if (processes.Length == 0)
            {
                throw new Exception("Process was not found");
            }

            Process proc = processes[0];
            Console.WriteLine($"Pname- {proc.ProcessName}, Pid- {proc.Id}");

            IntPtr processHandle = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION, false, proc.Id);
            if (processHandle == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to open process. Error code: {Marshal.GetLastWin32Error()}");
                return;
            }

            IntPtr moduleBaseAddress = GetModuleBaseAddress(proc, Offsets.ModuleName);
            if (moduleBaseAddress == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get module base address.");
                CloseHandle(processHandle);
                return;
            }

            IntPtr finalAddress = IntPtr.Add(moduleBaseAddress, Offsets.BaseOffset);
            finalAddress = IntPtr.Add(finalAddress, Offsets.AdditionalOffset);
            finalAddress = IntPtr.Add(finalAddress, Offsets.HealthOffset);

            Console.WriteLine($"Calculated Final Address: 0x{finalAddress.ToInt64():X}");

            // Read memory
            byte[] buffer = new byte[4];
            int bytesRead;
            if (ReadProcessMemory(processHandle, finalAddress, buffer, buffer.Length, out bytesRead))
            {
                Console.WriteLine($"Read successful: {BitConverter.ToInt32(buffer, 0)}");
            }
            else
            {
                Console.WriteLine($"Failed to read memory. Error code: {Marshal.GetLastWin32Error()}");
            }

            // Write memory
            byte[] newHealthBytes = BitConverter.GetBytes(Offsets.NewHealthValue);
            int bytesWritten;
            if (WriteProcessMemory(processHandle, finalAddress, newHealthBytes, newHealthBytes.Length, out bytesWritten))
            {
                if (bytesWritten == newHealthBytes.Length)
                {
                    Console.WriteLine($"Write successful. Bytes written: {bytesWritten}");
                }
                else
                {
                    Console.WriteLine($"Partial write. Bytes written: {bytesWritten}");
                }
            }
            else
            {
                Console.WriteLine($"Failed to write memory. Error code: {Marshal.GetLastWin32Error()}");
            }

            CloseHandle(processHandle);
        }

        private IntPtr GetModuleBaseAddress(Process process, string moduleName)
        {
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    return module.BaseAddress;
                }
            }
            return IntPtr.Zero;
        }
    }
}
