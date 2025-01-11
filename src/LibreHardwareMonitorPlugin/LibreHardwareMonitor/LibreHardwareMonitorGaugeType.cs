namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    public enum LibreHardwareMonitorGaugeType
    {
        None = 0,

        MonCPU = 1,
        CPULoad = 2,
        CPUCore = 3,
        CPUPower = 4,
        CPUPackage = 5,

        MonGPU = 6,
        GPULoad = 7,
        GPUCore = 8,
        GPUPower = 9,
        GPUHotspot = 10,

        MonMemoryLoad = 11,
        MemoryLoad = 12,
        VirtualMemoryLoad = 13,
        GPUMemoryLoad = 14,

        MonMemory = 15,
        Memory = 16,
        VirtualMemory = 17,
        GPUMemory = 18,

        MonStorageTG1 = 19,
        MonStorageTG2 = 20,
        StorageT1 = 21,
        StorageT2 = 22,
        StorageT3 = 23,
        StorageT4 = 24,
        StorageT5 = 25,
        StorageT6 = 26,

        MonStorageUG1 = 27,
        MonStorageUG2 = 28,
        StorageU1 = 29,
        StorageU2 = 30,
        StorageU3 = 31,
        StorageU4 = 32,
        StorageU5 = 33,
        StorageU6 = 34,

        Battery = 35,

        Count = 36 // should always be the last one
    }
}
