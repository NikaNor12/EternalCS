using EternalCS;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        MemoryRead memoryRead = new MemoryRead();
        memoryRead.Connection();
        //Offsets offsets = new Offsets();

        
        //Process proc = Process.GetProcessesByName(Offsets.ProcessNames)[0];
        //IntPtr baseAddress = memoryRead.GetModuleBaseAddress(proc, Offsets.ModuleName);
        //IntPtr finalAddress = IntPtr.Add(baseAddress, Offsets.BaseOffset);
        //finalAddress = IntPtr.Add(finalAddress, Offsets.AdditionalOffset);

     
        //memoryRead.Connection(Offsets.ProcessNames, Offsets.ModuleName, Offsets.BaseOffset, Offsets.AdditionalOffset, Offsets.HealthOffset, Offsets.NewHealthValue);
    }
}