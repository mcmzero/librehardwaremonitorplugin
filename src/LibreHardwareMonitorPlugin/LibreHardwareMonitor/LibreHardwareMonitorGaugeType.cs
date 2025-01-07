namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    public enum LibreHardwareMonitorGaugeType
    {
        None = 0,

        CPUMonitor = 1,
        CPULoad = 2,
        CPUCore = 3,
        CPUPower = 4,
        CPUPackage = 5,

        GPUMonitor = 6,
        GPULoad = 7,
        GPUCore = 8,
        GPUPower = 9,
        GPUHotspot = 10,

        MEMMonitor = 11,
        Memory = 12,
        VrMemory = 13,
        GPUMemory = 14,

        RAMMonitor = 15,
        RAM = 16,
        VrRAM = 17,
        VRAM = 18,

        DT1Monitor = 19,
        DT2 = 20,
        DT3 = 21,
        DT4Monitor = 22,
        DT5 = 23,
        DT6 = 24,

        DU1Monitor = 25,
        DU2 = 26,
        DU3 = 27,
        DU4Monitor = 28,
        DU5 = 29,
        DU6 = 30,

        Battery = 31,
        
        Count = 32 // should always be the last one
    }
}
