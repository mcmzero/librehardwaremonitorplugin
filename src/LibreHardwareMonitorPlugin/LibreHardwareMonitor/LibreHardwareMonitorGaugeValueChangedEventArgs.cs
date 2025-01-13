namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LibreHardwareMonitorGaugeValueChangedEventArgs : EventArgs
    {
        private readonly LHMGaugeType[] _modifiedGaugeTypes;

        public IEnumerable<LHMGaugeType> GaugeTypes => this._modifiedGaugeTypes;

        internal LibreHardwareMonitorGaugeValueChangedEventArgs(IEnumerable<LHMGaugeType> modifiedGaugeTypes) => this._modifiedGaugeTypes = modifiedGaugeTypes.ToArray();
    }
}
