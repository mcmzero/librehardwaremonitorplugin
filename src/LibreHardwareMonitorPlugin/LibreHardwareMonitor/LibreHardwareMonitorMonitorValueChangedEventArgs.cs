namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LibreHardwareMonitorMonitorValueChangedEventArgs : EventArgs
    {
        private readonly LibreHardwareMonitorGaugeType[] _modifiedMonitorTypes;

        public IEnumerable<LibreHardwareMonitorGaugeType> MonitorTypes => this._modifiedMonitorTypes;

        public LibreHardwareMonitorMonitorValueChangedEventArgs(IEnumerable<LibreHardwareMonitorGaugeType> modifiedMonitorTypes) => this._modifiedMonitorTypes = modifiedMonitorTypes.ToArray();
    }
}