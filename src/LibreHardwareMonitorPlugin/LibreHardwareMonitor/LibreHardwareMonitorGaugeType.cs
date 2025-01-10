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

        MemoryLoadMonitor = 11,
        MemoryLoad = 12,
        VirtualMemoryLoad = 13,
        GPUMemoryLoad = 14,

        MemoryMonitor = 15,
        Memory = 16,
        VirtualMemory = 17,
        GPUMemory = 18,

        Storage1TMonitor = 19,
        Storage2TMonitor = 20,
        Storage1T = 21,
        Storage2T = 22,
        Storage3T = 23,
        Storage4T = 24,
        Storage5T = 25,
        Storage6T = 26,

        Storage1UMonitor = 27,
        StorageUMonitor = 28,
        Storage1U = 29,
        Storage2U = 30,
        Storage3U = 31,
        Storage4U = 32,
        Storage5U = 33,
        Storage6U = 34,

        Battery = 35,

        Count = 36 // should always be the last one
    }
}
