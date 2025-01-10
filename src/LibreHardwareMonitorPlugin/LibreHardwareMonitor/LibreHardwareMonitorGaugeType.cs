﻿namespace NotADoctor99.LibreHardwareMonitorPlugin
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

        DT1Monitor = 19,
        DT2Monitor = 20,
        DiskT1 = 21,
        DiskT2 = 22,
        DiskT3 = 23,
        DiskT4 = 24,
        DiskT5 = 25,
        DiskT6 = 26,

        DU1Monitor = 27,
        DU2Monitor = 28,
        DiskU1 = 29,
        DiskU2 = 30,
        DiskU3 = 31,
        DiskU4 = 32,
        DiskU5 = 33,
        DiskU6 = 34,

        Battery = 35,

        Count = 36 // should always be the last one
    }
}
