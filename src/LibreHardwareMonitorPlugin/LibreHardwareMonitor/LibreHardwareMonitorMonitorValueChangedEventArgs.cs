namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LibreHardwareMonitorMonitorValueChangedEventArgs : EventArgs
    {
        private readonly LHMGaugeType[] _modifiedMonitorTypes;

        public IEnumerable<LHMGaugeType> MonitorTypes => this._modifiedMonitorTypes;

        public LibreHardwareMonitorMonitorValueChangedEventArgs(IEnumerable<LHMGaugeType> modifiedMonitorTypes) => this._modifiedMonitorTypes = modifiedMonitorTypes.ToArray();
    }
}