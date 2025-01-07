namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;
    using System.Linq;
    using System.Management;

    public static class LibreHardwareMonitorExtensions
    {
        public static String GetHardwareType(this ManagementBaseObject managementBaseObject) => (String)managementBaseObject["HardwareType"];

        public static String GetIdentifier(this ManagementBaseObject managementBaseObject) => (String)managementBaseObject["Identifier"];

        public static String GetInstanceId(this ManagementBaseObject managementBaseObject) => (String)managementBaseObject["InstanceId"];

        public static String GetDisplayName(this ManagementBaseObject managementBaseObject) => (String)managementBaseObject["Name"];

        public static String GetProcessId(this ManagementBaseObject managementBaseObject) => (String)managementBaseObject["ProcessId"];

        public static Single GetValue(this ManagementBaseObject managementBaseObject) => (Single)managementBaseObject["Value"];

        //public static Boolean HasSameItemsAs<T>(this T[] arrays[1], T[] arrays[2])
        //{
        //    if ((null == arrays[1]) && (null == arrays[2]))
        //    {
        //        return true;
        //    }

        //    if ((null == arrays[1]) || (null == arrays[2]))
        //    {
        //        return false;
        //    }

        //    if (arrays[1].Count() != arrays[2].Count())
        //    {
        //        return false;
        //    }

        //    return !arrays[1].Except(arrays[2]).Any() && !arrays[2].Except(arrays[1]).Any();
        //}
    }
}
