namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    public enum LibreHardwareMonitorGaugeType
    {
        None = 0,
        CPULoad = 1,
        CPUPower = 2,
        CPUCore = 3,
        CPUPackage = 4,
        CPUMonitor = 5,

        GPULoad = 6,
        GPUPower = 7,
        GPUCore = 8,
        GPUHotspot = 9,
        GPUMonitor = 10,

        Memory = 11,
        VrMemory = 12,
        GPUMemory = 13,
        MEMMonitor = 14,

        RAM = 15,
        VrRAM = 16,
        VRAM = 17,
        RAMMonitor = 18,

        NVME1 = 19,
        NVME2 = 20,
        NVME3 = 21,

        Battery = 22,
        
        Count = 23 // should always be the last one
    }
}
