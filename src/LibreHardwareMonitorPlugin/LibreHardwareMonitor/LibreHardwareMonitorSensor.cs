﻿namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;

    using Loupedeck;

    public class LibreHardwareMonitorSensor
    {
        private Boolean _isModified = false;

        public String Id { get; }

        public String Name { get; }

        public String InstanceId { get; }

        public String Identifier { get; }

        public String DisplayName { get; }

        public String FormatString { get; }

        public LibreHardwareMonitorGaugeType GaugeType { get; }
        public LibreHardwareMonitorGaugeType MonitorType { get; }

        public Single Value { get; private set; }
        public Single MinValue { get; private set; }
        public Single MaxValue { get; private set; }

        public BitmapColor Color { get; set; }

        public BitmapColor GetColor() => this.Color;
        public BitmapColor SetColor(BitmapColor color) => this.Color = color;

        internal LibreHardwareMonitorSensor(String name, String instanceId, String identifier, String displayName, String formatString, Single value, LibreHardwareMonitorGaugeType gaugeType, LibreHardwareMonitorGaugeType monitorType)
        {
            this.Id = LibreHardwareMonitorSensor.CreateSensorId(instanceId, identifier);

            this.Name = name;
            this.InstanceId = instanceId;
            this.Identifier = identifier;
            this.DisplayName = displayName;
            this.FormatString = formatString;
            this.Value = value;
            this.MinValue = value;
            this.MaxValue = 100;
            this.GaugeType = gaugeType;
            this.MonitorType = monitorType;
        }

        internal LibreHardwareMonitorSensor(String name, String instanceId, String identifier, String displayName, String formatString, Single value, LibreHardwareMonitorGaugeType gaugeType, LibreHardwareMonitorGaugeType monitorType, BitmapColor color)
        {
            this.Id = LibreHardwareMonitorSensor.CreateSensorId(instanceId, identifier);

            this.Name = name;
            this.InstanceId = instanceId;
            this.Identifier = identifier;
            this.DisplayName = displayName;
            this.FormatString = formatString;
            this.Value = value;
            this.MinValue = value;
            this.MaxValue = 100;
            this.GaugeType = gaugeType;
            this.MonitorType = monitorType;
            this.Color = color;
        }

        public Boolean IsModified()
        {
            var isModified = this._isModified;
            this._isModified = false;
            return isModified;
        }

        internal Boolean SetValue(Single value)
        {
            this._isModified = Math.Abs(this.Value - value) >= 0.1;

            if (!this._isModified)
            {
                return false;
            }

            this.Value = value;
            if (value < this.MinValue)
            {
                this.MinValue = value;
            }
            if (value > this.MaxValue)
            {
                this.MaxValue = value < 95 ? 95 : value;
            }

            return true;
        }

        public String GetButtonText() => String.Format(this.FormatString, this.Value);

        internal static String CreateSensorId(String instanceId, String identifier) => $"{instanceId}-{identifier}";
    }
}
