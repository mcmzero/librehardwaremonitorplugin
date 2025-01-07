﻿namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;
    using System.Linq;
    using System.Reflection.Emit;

    using Loupedeck;

    public class ShowGaugeCommand : PluginDynamicCommand
    {
        public ShowGaugeCommand()
        {
            this.IsWidget = true;
            this.GroupName = "Gauges";
            var MonitorsGroupName = "Monitors";

            AddParameter(LibreHardwareMonitorGaugeType.CPULoad, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPUCore, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPUPackage, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPUPower, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPUMonitor, MonitorsGroupName);

            AddParameter(LibreHardwareMonitorGaugeType.GPULoad, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUCore, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUHotspot, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUPower, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUMonitor, MonitorsGroupName);

            AddParameter(LibreHardwareMonitorGaugeType.Memory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.VrMemory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.RAM, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUMemory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.MEMMonitor, MonitorsGroupName);

            AddParameter(LibreHardwareMonitorGaugeType.VrRAM, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.VRAM, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.RAMMonitor, MonitorsGroupName);

            AddParameter(LibreHardwareMonitorGaugeType.DT1Monitor, MonitorsGroupName);
            AddParameter(LibreHardwareMonitorGaugeType.DT4Monitor, MonitorsGroupName);

            AddParameter(LibreHardwareMonitorGaugeType.DU1Monitor, MonitorsGroupName);
            AddParameter(LibreHardwareMonitorGaugeType.DU4Monitor, MonitorsGroupName);

            AddParameter(LibreHardwareMonitorGaugeType.Battery, this.GroupName);

            void AddParameter(LibreHardwareMonitorGaugeType gaugeType, String GroupName) => this.AddParameter(gaugeType.ToString(), gaugeType.ToString(), GroupName);
        }

        protected override Boolean OnLoad()
        {
            LibreHardwareMonitorPlugin.HardwareMonitor.SensorListChanged += this.OnSensorListChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.GaugeValuesChanged += this.OnGaugeValuesChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessStarted += this.OnHardwareMonitorProcessStarted;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessExited += this.OnHardwareMonitorProcessExited;

            return true;
        }

        protected override Boolean OnUnload()
        {
            LibreHardwareMonitorPlugin.HardwareMonitor.SensorListChanged -= this.OnSensorListChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.GaugeValuesChanged -= this.OnGaugeValuesChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessStarted -= this.OnHardwareMonitorProcessStarted;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessExited -= this.OnHardwareMonitorProcessExited;

            return true;
        }

        protected override void RunCommand(String actionParameter) => LibreHardwareMonitor.ActivateOrRun();

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => PluginHelpers.GetNotAvailableButtonText();

        private readonly Single[] _lastLevels = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];
        private readonly Single[] _lastMinLevels = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];
        private readonly Single[] _lastMaxLevels = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];

        private Int32 GetImageIndex(LibreHardwareMonitorGaugeType guageType) => Helpers.MinMax(((Int32)this._lastLevels[(Int32)guageType] + 6) / 7, 0, 15);
        private Int32 GetImageIndexMax(LibreHardwareMonitorGaugeType guageType) => (Int32)Helpers.MinMax((100 * this._lastLevels[(Int32)guageType] / this._lastMaxLevels[(Int32)guageType] + 6) / 7, 0, 15);

        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue)
        {
            var rgbn = Helpers.MinMax((Int32)(baseValue + (255 - baseValue) * level / maxLevel), 0, 255);
            return new BitmapColor(rgbn, rgbn, rgbn);
        }
        private BitmapColor GetColorByLevel(Single level, BitmapColor accentColor) => level > 90 ? accentColor : this.GetColorByLevel(level, 100, 100);
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor) => (100 * level/maxLevel) > 90 ? accentColor : this.GetColorByLevel(level, maxLevel, 100);
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor, Int32 baseValue) => (100 * level / maxLevel) > 90 ? accentColor : this.GetColorByLevel(level, maxLevel, baseValue);

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (!Enum.TryParse<LibreHardwareMonitorGaugeType>(actionParameter, out var gaugeType) || !LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensor(gaugeType, out var sensor))
            {
                return PluginHelpers.GetNotAvailableButtonImage();
            }

            using (var bitmapBuilder = new BitmapBuilder(PluginImageSize.Width90))
            {
                var x = 0;
                var y = 0;

                var n = 5;
                var vn = 16;
                var ys = new Int32[4];
                for (var i = 0; i < 4; i++)
                {
                    ys[i] = n + vn * (i + 1);
                }

                var width = 80;
                var height = 11;

                var titleFontSize = 12;
                var fontSize = 12;
                var monFontSize= 12;
                var fontColor = BitmapColor.White;

                var guageTypes = new Int32[3];
                var maxLevels = new Single[3];
                var levels = new Single[3];
                for (var i = 0; i < 3; i++)
                {
                    maxLevels[i] = this._lastMaxLevels[(Int32)gaugeType];
                    levels[i] = this._lastLevels[(Int32)gaugeType];
                    guageTypes[i] = (Int32)gaugeType;
                }

                var rectangleColor = new BitmapColor(35, 35, 35);
                bitmapBuilder.Clear(rectangleColor);

                // Rectangle: x, y , width, height
                var frOutline  = new Int32[4] { 0, 0, 79, 79 };
                var frMiddle   = new Int32[4] { frOutline[0] + 7, frOutline[1] + 17, frOutline[2] - 7 * 2 + 1, frOutline[3] - 24 };

                // Line: x1, y1, x2, y2
                var offset = 0;
                var LeftLine   = new Int32[4] { frMiddle[0] - offset, frMiddle[1],
                                                frMiddle[0] - offset, frMiddle[1] + frMiddle[3] };
                var RightLine  = new Int32[4] { frMiddle[0] + frMiddle[2] + offset, frMiddle[1],
                                                frMiddle[0] + frMiddle[2] + offset, frMiddle[1] + frMiddle[3] };
                var TopLine    = new Int32[4] { frMiddle[0], frMiddle[1] - offset,
                                                frMiddle[0] + frMiddle[2], frMiddle[1] - offset };
                var BottomLine = new Int32[4] { frMiddle[0], frMiddle[1] + frMiddle[3] + offset,
                                                frMiddle[0] + frMiddle[2], frMiddle[1] + frMiddle[3] + offset };

                var lightGrayColor = new BitmapColor(150, 150, 140);
                var grayColor = new BitmapColor(120, 120, 110);
                var darkGrayColor = new BitmapColor(60, 60, 50);
                var accentColor = sensor.Color;

                bitmapBuilder.FillRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], BitmapColor.Black);

                var imageIndex = this.GetImageIndex(gaugeType);
                switch (gaugeType)
                {
                    case LibreHardwareMonitorGaugeType.CPULoad:
                        bitmapBuilder.DrawText("CPU(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}%", x, ys[2], width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUCore:
                        bitmapBuilder.DrawText("Core(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        levels[2] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.CPUPackage];
                        bitmapBuilder.DrawText($"{levels[0]:N1} | {levels[2]:N1}", x, ys[2], width, height, fontColor, 10);
                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPackage:
                        bitmapBuilder.DrawText("Pkg(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}℃", x, ys[2], width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}W", x, ys[2], width, height, fontColor, fontSize);

                        maxLevels[0] = maxLevels[0] < 120 ? 120 : maxLevels[0];
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        maxLevels[2] = maxLevels[0];

                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.Memory:
                        bitmapBuilder.DrawText("Mem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}%", x, ys[2], width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.RAM:
                        bitmapBuilder.DrawText("RAM(GB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}GB", x, ys[2], width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.Memory);
                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        }
                        break;
                    case LibreHardwareMonitorGaugeType.VrMemory:
                        bitmapBuilder.DrawText("VrMem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}%", x, ys[2], width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.VrRAM:
                        bitmapBuilder.DrawText("VrRAM(GB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}GB", x, ys[2], width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.VrMemory);
                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.GPULoad:
                        bitmapBuilder.DrawText("GPU(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}%", x, ys[2], width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUCore:
                        bitmapBuilder.DrawText("Core(℃)", x, y, width, height, sensor.Color, titleFontSize);

                        maxLevels[0] = maxLevels[0] < 83 ? 83 : maxLevels[0];
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevels[0];
                        imageIndex = this.GetImageIndexMax(gaugeType);

                        maxLevels[1] = maxLevels[0];

                        levels[2] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUHotspot];
                        maxLevels[2] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPUHotspot];
                        maxLevels[2] = maxLevels[2] < 83 ? 83 : maxLevels[2];
                        this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPUHotspot] = maxLevels[2];
                        
                        bitmapBuilder.DrawText($"{levels[0]:N1} | {levels[2]:N1}", x, ys[2], width, height, fontColor, 10);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUHotspot:
                        bitmapBuilder.DrawText("HSpot(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}℃", x, ys[2], width, height, fontColor, fontSize);

                        maxLevels[0] = maxLevels[0] < 83 ? 83 : maxLevels[0];
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        maxLevels[2] = maxLevels[0];

                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}W", x, ys[2], width, height, fontColor, fontSize);

                        maxLevels[0] = maxLevels[0] < 320 ? 320 : maxLevels[0];
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        maxLevels[2] = maxLevels[0];

                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUMemory:
                        bitmapBuilder.DrawText("GPUMem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}%", x, ys[2], width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.VRAM:
                        bitmapBuilder.DrawText("VRAM(MB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N0}MB", x, ys[2], width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.GPUMemory);
                        levels[0] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        maxLevels[0] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        break;

                    case LibreHardwareMonitorGaugeType.CPUMonitor:
                        bitmapBuilder.DrawText("CPU", x, y, width, height, sensor.Color, titleFontSize);
                        Int32 gt;
                        String drtext;

                        gt = (Int32)LibreHardwareMonitorGaugeType.CPULoad;
                        maxLevels[0] = this._lastMaxLevels[gt];
                        levels[0] = this._lastLevels[gt];
                        drtext = levels[0] < 10 ? $"[L] 0{levels[0]:N1}%" : $"[L] {levels[0]:N1}%";
                        bitmapBuilder.DrawText(drtext, x, ys[0], width, height, this.GetColorByLevel(levels[0], maxLevels[0], accentColor), monFontSize);

                        gt = (Int32)LibreHardwareMonitorGaugeType.CPUCore;
                        maxLevels[1] = this._lastMaxLevels[gt];
                        levels[1] = this._lastLevels[gt];
                        drtext = levels[1] < 10 ? $"[C] 0{levels[1]:N1}℃" : $"[C] {levels[1]:N1}℃";
                        bitmapBuilder.DrawText(drtext, x, ys[1], width, height, this.GetColorByLevel(levels[1], maxLevels[1], accentColor), monFontSize);

                        gt = (Int32)LibreHardwareMonitorGaugeType.CPUPower;
                        maxLevels[2] = this._lastMaxLevels[gt];
                        levels[2] = this._lastLevels[gt];
                        drtext = levels[2] < 10 ? $"[P] 0{levels[2]:N1}W" : $"[P] {levels[2]:N1}W";
                        bitmapBuilder.DrawText(drtext, x, ys[2], width, height, this.GetColorByLevel(levels[1], maxLevels[2], accentColor), monFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.GPUMonitor:
                        bitmapBuilder.DrawText("GPU", x, y, width, height, sensor.Color, titleFontSize);

                        gt = (Int32)LibreHardwareMonitorGaugeType.GPULoad;
                        maxLevels[0] = this._lastMaxLevels[gt];
                        levels[0] = this._lastLevels[gt];
                        drtext = levels[0] < 10 ? $"[L] 0{levels[0]:N1}%" : $"[L] {levels[0]:N1}%";
                        bitmapBuilder.DrawText(drtext, x, ys[0], width, height, this.GetColorByLevel(levels[0], maxLevels[0], accentColor), monFontSize);

                        gt = (Int32)LibreHardwareMonitorGaugeType.GPUCore;
                        maxLevels[1] = this._lastMaxLevels[gt];
                        levels[1] = this._lastLevels[gt];
                        drtext = levels[1] < 10 ? $"[C] 0{levels[1]:N1}℃" : $"[C] {levels[1]:N1}℃";
                        bitmapBuilder.DrawText(drtext, x, ys[1], width, height, this.GetColorByLevel(levels[1], maxLevels[1], accentColor), monFontSize);

                        gt = (Int32)LibreHardwareMonitorGaugeType.GPUPower;
                        maxLevels[2] = this._lastMaxLevels[gt];
                        levels[2] = this._lastLevels[gt];
                        drtext = levels[2] < 10 ? $"[P] 0{levels[2]:N1}W" : $"[P] {levels[2]:N1}W";
                        bitmapBuilder.DrawText(drtext, x, ys[2], width, height, this.GetColorByLevel(levels[1], maxLevels[2], accentColor), monFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.MEMMonitor:
                        bitmapBuilder.DrawText("MEM(%)", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        guageTypes[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastLevels[guageTypes[i]];
                        }

                        drtext = levels[0] < 10 ? $"[S] 0{levels[0]:N1}%" : $"[S] {levels[0]:N1}%";
                        bitmapBuilder.DrawText(drtext, x, ys[0], width, height, this.GetColorByLevel(levels[0], maxLevels[0], accentColor), monFontSize);

                        drtext = levels[1] < 10 ? $"[V] 0{levels[1]:N1}%" : $"[V] {levels[1]:N1}%";
                        bitmapBuilder.DrawText(drtext, x, ys[1], width, height, this.GetColorByLevel(levels[1], maxLevels[1], accentColor), monFontSize);

                        drtext = levels[2] < 10 ? $"[G] 0{levels[2]:N1}%" : $"[G] {levels[2]:N1}%";
                        bitmapBuilder.DrawText(drtext, x, ys[2], width, height, this.GetColorByLevel(levels[1], maxLevels[2], accentColor), monFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.RAMMonitor:
                        bitmapBuilder.DrawText("RAM(GB)", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        guageTypes[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastLevels[guageTypes[i]];
                        }
                        bitmapBuilder.DrawText($"[S] {this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.RAM]:N1}G", x, ys[0], width, height, this.GetColorByLevel(levels[0], maxLevels[0], accentColor), monFontSize);
                        bitmapBuilder.DrawText($"[V] {this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrRAM]:N1}G", x, ys[1], width, height, this.GetColorByLevel(levels[1], maxLevels[1], accentColor), monFontSize);
                        bitmapBuilder.DrawText($"[G] {this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VRAM] / 1024:N2}G", x, ys[2], width, height, this.GetColorByLevel(levels[1], maxLevels[2], accentColor), monFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.DT1Monitor:
                        bitmapBuilder.DrawText("DISK(℃)", x, y, width, height, sensor.Color, titleFontSize);

                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.DT1Monitor + i];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DT1Monitor + i];
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DT1Monitor + i;
                            //PluginLog.Info($"NVME{i + 1}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType + "," + imageIndex);
                            if (levels[i] > 0)
                            {
                                bitmapBuilder.DrawText($"[{i + 1}] {levels[i]:N0}℃", x, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DT4Monitor:
                        bitmapBuilder.DrawText("DISK(℃)", x, y, width, height, sensor.Color, titleFontSize);

                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.DT4Monitor + i];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DT4Monitor + i];
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DT4Monitor + i;
                            //PluginLog.Info($"NVME{i + 4}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType);
                            if (levels[i] > 0)
                            {
                                bitmapBuilder.DrawText($"[{i + 4}] {levels[i]:N0}℃", x, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU1Monitor:
                        bitmapBuilder.DrawText("DISK(%)", x, y, width, height, sensor.Color, titleFontSize);

                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.DU1Monitor + i];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DU1Monitor + i];
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DU1Monitor + i;
                            //PluginLog.Info($"NVME{i + 1}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType);
                            if (levels[i] > 0)
                            {
                                drtext = levels[i] < 10 ? $"[{i + 1}] 0{levels[i]:N1}%" : $"[{i + 1}] {levels[i]:N1}%";
                                bitmapBuilder.DrawText(drtext, x, ys[i], width, height, this.GetColorByLevel(levels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU4Monitor:
                        bitmapBuilder.DrawText("DISK(%)", x, y, width, height, sensor.Color, titleFontSize);

                        for (var i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.DU4Monitor + i];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DU4Monitor + i];
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DU4Monitor + i;
                            //PluginLog.Info($"NVME{i + 4}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType);
                            if (levels[i] > 0)
                            {
                                drtext = levels[i] < 10 ? $"[{i + 4}] 0{levels[i]:N1}%" : $"[{i + 4}] {levels[i]:N1}%";
                                bitmapBuilder.DrawText(drtext, x, ys[i], width, height, this.GetColorByLevel(levels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Battery:
                        bitmapBuilder.DrawText("Battery(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}%", x, ys[2], width, height, fontColor, fontSize);
                        break;
                }
                if (gaugeType != LibreHardwareMonitorGaugeType.CPUMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.GPUMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.MEMMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.RAMMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DT1Monitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DT4Monitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DU1Monitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DU4Monitor)
                {
                    //PluginLog.Info($"g{imageIndex}.png");
                    var imageBytes = PluginResources.ReadBinaryFile($"g{imageIndex}.png");
                    bitmapBuilder.DrawImage(imageBytes, x, ys[0] - 9);
                }

                var middlePoint = TopLine[0] + (TopLine[2] - TopLine[0]) / 2;
                var startPoint  = middlePoint - (TopLine[2] - TopLine[0]) * levels[2] / maxLevels[2] / 2;
                var endPoint    = middlePoint + (TopLine[2] - TopLine[0]) * levels[2] / maxLevels[2] / 2;
                bitmapBuilder.DrawLine(startPoint, TopLine[1], endPoint, TopLine[3], this.GetColorByLevel(levels[2], maxLevels[2], accentColor), 3);
                middlePoint = BottomLine[0] + (BottomLine[2] - BottomLine[0]) / 2;
                startPoint  = middlePoint - (BottomLine[2] - BottomLine[0]) * levels[2] / maxLevels[2] / 2;
                endPoint    = middlePoint + (BottomLine[2] - BottomLine[0]) * levels[2] / maxLevels[2] / 2;
                bitmapBuilder.DrawLine(startPoint, BottomLine[1], endPoint, BottomLine[3], this.GetColorByLevel(levels[2], maxLevels[2], accentColor), 3);

                middlePoint = LeftLine[1] + (LeftLine[3] - LeftLine[1]) / 2;
                startPoint = middlePoint - (LeftLine[3] - LeftLine[1]) * levels[0] / maxLevels[0] / 2;
                endPoint    = middlePoint + (LeftLine[3] - LeftLine[1]) * levels[0] / maxLevels[0] / 2;
                bitmapBuilder.DrawLine(LeftLine[0], startPoint, LeftLine[2], endPoint, this.GetColorByLevel(levels[0], maxLevels[0], accentColor), 3);

                middlePoint = RightLine[1] + (RightLine[3] - RightLine[1]) / 2;
                startPoint  = middlePoint  - (RightLine[3] - RightLine[1]) * levels[1] / maxLevels[1] / 2;
                endPoint    = middlePoint  + (RightLine[3] - RightLine[1]) * levels[1] / maxLevels[1] / 2;
                bitmapBuilder.DrawLine(RightLine[0], startPoint, RightLine[2], endPoint, this.GetColorByLevel(levels[1], maxLevels[1], accentColor), 3);

                bitmapBuilder.DrawRectangle(frOutline[0], frOutline[1], frOutline[2], frOutline[3], this.GetColorByLevel(levels[0], maxLevels[0], accentColor, 0));
                bitmapBuilder.DrawRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], this.GetColorByLevel(levels[0], maxLevels[0], accentColor, 0));

                return bitmapBuilder.ToImage();
            }
        }

        private void OnGaugeValuesChanged(Object sender, LibreHardwareMonitorGaugeValueChangedEventArgs e)
        {
            foreach (var gaugeType in e.GaugeTypes)
            {
                if (this.UpdateGaugeIndex(gaugeType))
                {
                    this.ActionImageChanged(gaugeType.ToString());
                }
            }
        }

        private void OnSensorListChanged(Object sender, EventArgs e)
        {
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPULoad);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUCore);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUPackage);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUPower);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUMonitor);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPULoad);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUCore);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUHotspot);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUPower);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUMonitor);
            
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Memory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VrMemory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUMemory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.MEMMonitor);
            
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.RAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VrRAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VRAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.RAMMonitor);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DT1Monitor);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DT2);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DT3);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DT4Monitor);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DT5);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DT6);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DU1Monitor);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DU2);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DU3);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DU4Monitor);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DU5);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DU6);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Battery);

            this.ActionImageChanged(null);
        }

        private Boolean UpdateGaugeIndex(LibreHardwareMonitorGaugeType gaugeType)
        {
            if (!LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensor(gaugeType, out var sensor))
            {
                return false;
            }

            if (!this._lastLevels[(Int32)gaugeType].Equals(sensor.Value))
            {
                this._lastLevels[(Int32)gaugeType] = sensor.Value;
                this._lastMinLevels[(Int32)gaugeType] = sensor.MinValue;
                this._lastMaxLevels[(Int32)gaugeType] = sensor.MaxValue;
                return true;
            }

            return false;
        }

        private void OnHardwareMonitorProcessStarted(Object sender, EventArgs e) => this.ActionImageChanged(null);

        private void OnHardwareMonitorProcessExited(Object sender, EventArgs e) => this.ActionImageChanged(null);
    }
}
