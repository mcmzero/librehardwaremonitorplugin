namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    public enum LHMGaugeType
    {
        None = 0,

        Monitor_CPU = 1,
        CPU_Load = 2,
        CPU_Core = 3,
        CPU_Package = 4,
        CPU_Power = 5,

        Monitor_GPU = 6,
        GPU_Load = 7,
        GPU_Core = 8,
        GPU_Hotspot = 9,
        GPU_Power = 10,

        Monitor_Memory_Load = 11,
        Memory_Load = 12,
        Virtual_Memory_Load = 13,
        GPU_Memory_Load = 14,

        Monitor_Memory = 15,
        Memory = 16,
        Virtual_Memory = 17,
        GPU_Memory = 18,

        Monitor_Storage_T_G1 = 19,
        Storage_T_1 = 20,
        Storage_T_2 = 21,
        Storage_T_3 = 22,
        Monitor_Storage_T_G2 = 23,
        Storage_T_4 = 24,
        Storage_T_5 = 25,
        Storage_T_6 = 26,

        Monitor_Storage_U_G1 = 27,
        Storage_U_1 = 28,
        Storage_U_2 = 29,
        Storage_U_3 = 30,
        Monitor_Storage_U_G2 = 31,
        Storage_U_4 = 32,
        Storage_U_5 = 33,
        Storage_U_6 = 34,

        Battery = 35,

        Count = 36 // should always be the last one
    }
}
