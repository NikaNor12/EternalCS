namespace EternalCS
{
    public class Offsets
    {
        public const string ProcessNames = "hl"; // Replace with your actual process name
        public const string ModuleName = "hw.dll";
        public const int BaseOffset = 0x007BBD9C; // Base offset for LocalPlayer
        public const int AdditionalOffset = 0x7C; // Additional offset for LocalPlayer
        public const int HealthOffset = 0x160; // Offset for Health
        public const int NewHealthValue = 1; // new health  
    }
}