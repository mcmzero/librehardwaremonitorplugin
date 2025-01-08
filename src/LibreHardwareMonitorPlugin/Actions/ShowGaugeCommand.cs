namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;

    using Loupedeck;

    public class ShowGaugeCommand : PluginDynamicCommand
    {
        public ShowGaugeCommand()
        {
            this.IsWidget = true;
            this.GroupName = "Gauges";
            var GroupMonitor = "Monitors";
            var GroupStorage = "Storages";

            AddParameter(LibreHardwareMonitorGaugeType.CPULoad, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPUCore, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPUPackage, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPUPower, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.GPULoad, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUCore, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUHotspot, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUPower, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.Memory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.VrMemory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUMemory, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.RAM, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.VrRAM, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.VRAM, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.DiskT1, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskT2, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskT3, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskT5, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskT6, GroupStorage);

            AddParameter(LibreHardwareMonitorGaugeType.DiskU1, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskU2, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskU3, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskU5, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.DiskU6, GroupStorage);

            AddParameter(LibreHardwareMonitorGaugeType.Battery, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.CPUMonitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.GPUMonitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.MEMMonitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.RAMMonitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DT1Monitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DT2Monitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DU1Monitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DU2Monitor, GroupMonitor);

            void AddParameter(LibreHardwareMonitorGaugeType gaugeType, String GroupName) => this.AddParameter(gaugeType.ToString(), gaugeType.ToString(), GroupName);
        }

        protected override Boolean OnLoad()
        {
            LibreHardwareMonitorPlugin.HardwareMonitor.SensorListChanged += this.OnSensorListChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.GaugeValuesChanged += this.OnGaugeValuesChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.MonitorValuesChanged += this.OnMonitorValuesChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessStarted += this.OnHardwareMonitorProcessStarted;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessExited += this.OnHardwareMonitorProcessExited;

            return true;
        }

        protected override Boolean OnUnload()
        {
            LibreHardwareMonitorPlugin.HardwareMonitor.SensorListChanged -= this.OnSensorListChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.GaugeValuesChanged -= this.OnGaugeValuesChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.MonitorValuesChanged -= this.OnMonitorValuesChanged;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessStarted -= this.OnHardwareMonitorProcessStarted;
            LibreHardwareMonitorPlugin.HardwareMonitor.ProcessExited -= this.OnHardwareMonitorProcessExited;

            return true;
        }

        protected override void RunCommand(String actionParameter) => LibreHardwareMonitor.ActivateOrRun();

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => PluginHelpers.GetNotAvailableButtonText();

        private readonly Single[] _lastMonLevels = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];
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
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor) => (100 * level / maxLevel) > 90 ? accentColor : this.GetColorByLevel(level, maxLevel, 100);
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor, Int32 baseValue) => (100 * level / maxLevel) > 90 ? accentColor : this.GetColorByLevel(level, maxLevel, baseValue);

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (!Enum.TryParse<LibreHardwareMonitorGaugeType>(actionParameter, out var gaugeType))
            {
                //PluginLog.Info("GetCommandImage: " + actionParameter);
                return PluginHelpers.GetNotAvailableButtonImage();
            }
            if (!LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensor(gaugeType, out var sensor))
            {
                if (!LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensorList(gaugeType, out var sensorList))
                {
                    //PluginLog.Info("GetCommandImage: " + actionParameter);
                    return PluginHelpers.GetNotAvailableButtonImage();
                }
                else
                {
                    sensor = sensorList[0];
                }
            }

            using (var bitmapBuilder = new BitmapBuilder(PluginImageSize.Width90))
            {
                var x = 0;
                var y = 1;

                var i = 0;
                var n = 5;
                var vn = 16;
                var ys = new Int32[4];
                for (i = 0; i < 4; i++)
                {
                    ys[i] = n + vn * (i + 1);
                }

                var width = 80;
                var height = 11;

                var titleFontSize = 12;
                var fontSize = 12;
                var monFontSize = 12;
                var fontColor = BitmapColor.White;

                var maxLimit = 95;
                var guageTypes = new Int32[3];
                var maxLevels = new Single[3];
                var levels = new Single[3];
                for (i = 0; i < 3; i++)
                {
                    maxLevels[i] = this._lastMaxLevels[(Int32)gaugeType];
                    levels[i] = this._lastLevels[(Int32)gaugeType];
                    guageTypes[i] = (Int32)gaugeType;
                }

                var rectangleColor = new BitmapColor(35, 35, 35);
                bitmapBuilder.Clear(rectangleColor);

                // Rectangle: x, y , width, height
                var frOutline = new Int32[4] { 0, 0, 79, 79 }; // 80x80
                var frMiddle = new Int32[4] { frOutline[0] + 7, frOutline[1] + 17, frOutline[2] - 7 * 2 + 1, frOutline[3] - 24 };

                // Line: x1, y1, x2, y2
                var offset = 0;
                var LeftLine = new Int32[4] { frMiddle[0] - offset, frMiddle[1],
                                                frMiddle[0] - offset, frMiddle[1] + frMiddle[3] };
                var RightLine = new Int32[4] { frMiddle[0] + frMiddle[2] + offset, frMiddle[1],
                                                frMiddle[0] + frMiddle[2] + offset, frMiddle[1] + frMiddle[3] };
                var TopLine = new Int32[4] { frMiddle[0], frMiddle[1] - offset,
                                                frMiddle[0] + frMiddle[2], frMiddle[1] - offset };
                var BottomLine = new Int32[4] { frMiddle[0], frMiddle[1] + frMiddle[3] + offset,
                                                frMiddle[0] + frMiddle[2], frMiddle[1] + frMiddle[3] + offset };

                var lightGrayColor = new BitmapColor(150, 150, 140);
                var grayColor = new BitmapColor(120, 120, 110);
                var darkGrayColor = new BitmapColor(60, 60, 50);
                var accentColor = sensor.Color;

                bitmapBuilder.FillRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], BitmapColor.Black);

                String drText;
                var drTitleText = new String[3];
                var drUnitText = new String[3];

                var imageIndex = this.GetImageIndex(gaugeType);
                switch (gaugeType)
                {
                    // Guages
                    case LibreHardwareMonitorGaugeType.CPULoad:
                        bitmapBuilder.DrawText("CPU(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("%", +18, ys[2], width, height, fontColor, fontSize);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUCore:
                        bitmapBuilder.DrawText("Core(℃)", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.CPUCore;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.CPUPackage;

                        maxLimit = 95;
                        maxLevels[0] = maxLevels[0] < maxLimit ? maxLimit : maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        this._lastMaxLevels[guageTypes[0]] = maxLevels[0];
                        imageIndex = this.GetImageIndexMax(gaugeType);

                        levels[2] = this._lastLevels[guageTypes[2]];
                        maxLevels[2] = this._lastMaxLevels[guageTypes[2]];
                        maxLevels[2] = maxLevels[2] < maxLimit ? maxLimit : maxLevels[2];
                        this._lastMaxLevels[guageTypes[2]] = maxLevels[2];

                        bitmapBuilder.DrawText("|", 0, ys[2], width, height, fontColor, 10);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -15, ys[2], width, height, fontColor, 10);
                        bitmapBuilder.DrawText($"{levels[2]:N1}", +16, ys[2], width, height, fontColor, 10);

                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPackage:
                        bitmapBuilder.DrawText("Pkg(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("℃", +18, ys[2], width, height, fontColor, fontSize);

                        imageIndex = this.GetImageIndexMax(gaugeType);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("W", +18, ys[2], width, height, fontColor, fontSize);

                        maxLimit = 120;
                        maxLevels[0] = maxLevels[0] < maxLimit ? maxLimit : maxLevels[0];
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        maxLevels[2] = maxLevels[0];

                        imageIndex = this.GetImageIndexMax(gaugeType);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.Memory:
                        bitmapBuilder.DrawText("Mem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("%", +18, ys[2], width, height, fontColor, fontSize);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.RAM:
                        bitmapBuilder.DrawText("RAM(GB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("GB", +20, ys[2], width, height, fontColor, fontSize);

                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.Memory);
                        for (i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        }
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.VrMemory:
                        bitmapBuilder.DrawText("VrMem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("%", +18, ys[2], width, height, fontColor, fontSize);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.VrRAM:
                        bitmapBuilder.DrawText("VrRAM(GB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("GB", +20, ys[2], width, height, fontColor, fontSize);

                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.VrMemory);
                        for (i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        }
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;

                    case LibreHardwareMonitorGaugeType.GPULoad:
                        bitmapBuilder.DrawText("GPU(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("%", +18, ys[2], width, height, fontColor, fontSize);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUCore:
                        bitmapBuilder.DrawText("Core(℃)", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.GPUCore;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.GPUHotspot;

                        maxLimit = 83;
                        maxLevels[0] = maxLevels[0] < maxLimit ? maxLimit : maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        this._lastMaxLevels[guageTypes[0]] = maxLevels[0];
                        imageIndex = this.GetImageIndexMax(gaugeType);

                        levels[2] = this._lastLevels[guageTypes[2]];
                        maxLevels[2] = this._lastMaxLevels[guageTypes[2]];
                        maxLevels[2] = maxLevels[2] < maxLimit ? maxLimit : maxLevels[2];
                        this._lastMaxLevels[guageTypes[2]] = maxLevels[2];

                        bitmapBuilder.DrawText("|", 0, ys[2], width, height, fontColor, 10);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -15, ys[2], width, height, fontColor, 10);
                        bitmapBuilder.DrawText($"{levels[2]:N1}", +16, ys[2], width, height, fontColor, 10);

                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUHotspot:
                        bitmapBuilder.DrawText("HSpot(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("℃", +18, ys[2], width, height, fontColor, fontSize);

                        maxLimit = 83;
                        maxLevels[0] = maxLevels[0] < maxLimit ? maxLimit : maxLevels[0];
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        maxLevels[2] = maxLevels[0];

                        imageIndex = this.GetImageIndexMax(gaugeType);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("W", +18, ys[2], width, height, fontColor, fontSize);

                        maxLimit = 320;
                        maxLevels[0] = maxLevels[0] < maxLimit ? maxLimit : maxLevels[0];
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevels[0];
                        maxLevels[1] = maxLevels[0];
                        maxLevels[2] = maxLevels[0];

                        imageIndex = this.GetImageIndexMax(gaugeType);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUMemory:
                        bitmapBuilder.DrawText("GPUMem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("%", +18, ys[2], width, height, fontColor, fontSize);
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;
                    case LibreHardwareMonitorGaugeType.VRAM:
                        bitmapBuilder.DrawText("VRAM(MB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N0}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("MB", +18, ys[2], width, height, fontColor, fontSize);

                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.GPUMemory);
                        levels[0] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        maxLevels[0] = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        this.DrawImage(bitmapBuilder, imageIndex, x, ys[0] - 12);
                        break;

                    // Storages
                    case LibreHardwareMonitorGaugeType.DiskT1:
                    case LibreHardwareMonitorGaugeType.DiskT2:
                    case LibreHardwareMonitorGaugeType.DiskT3:
                    case LibreHardwareMonitorGaugeType.DiskT4:
                    case LibreHardwareMonitorGaugeType.DiskT5:
                    case LibreHardwareMonitorGaugeType.DiskT6:
                        bitmapBuilder.DrawText($"{gaugeType}(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N0}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("℃", +18, ys[2], width, height, fontColor, fontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.DiskU1:
                    case LibreHardwareMonitorGaugeType.DiskU2:
                    case LibreHardwareMonitorGaugeType.DiskU3:
                    case LibreHardwareMonitorGaugeType.DiskU4:
                    case LibreHardwareMonitorGaugeType.DiskU5:
                    case LibreHardwareMonitorGaugeType.DiskU6:
                        bitmapBuilder.DrawText($"{gaugeType}(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("%", +18, ys[2], width, height, fontColor, fontSize);
                        break;

                    // Monitors
                    case LibreHardwareMonitorGaugeType.CPUMonitor:
                        bitmapBuilder.DrawText("CPU", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.CPULoad;
                        guageTypes[1] = (Int32)LibreHardwareMonitorGaugeType.CPUCore;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.CPUPower;

                        drTitleText[0] = "[L]";
                        drTitleText[1] = "[C]";
                        drTitleText[2] = "[P]";

                        drUnitText[0] = "%";
                        drUnitText[1] = "℃";
                        drUnitText[2] = "W";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastMonLevels[guageTypes[i]];
                            drText = levels[i] < 10 ? $"0{levels[i]:N1}" : $"{levels[i]:N1}";
                            bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.GPUMonitor:
                        bitmapBuilder.DrawText("GPU", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.GPULoad;
                        guageTypes[1] = (Int32)LibreHardwareMonitorGaugeType.GPUCore;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.GPUPower;

                        drTitleText[0] = "[L]";
                        drTitleText[1] = "[C]";
                        drTitleText[2] = "[P]";

                        drUnitText[0] = "%";
                        drUnitText[1] = "℃";
                        drUnitText[2] = "W";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastMonLevels[guageTypes[i]];
                            drText = levels[i] < 10 ? $"0{levels[i]:N1}" : $"{levels[i]:N1}";
                            bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.MEMMonitor:
                        bitmapBuilder.DrawText("MEM(%)", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        guageTypes[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;

                        drTitleText[0] = "[S]";
                        drTitleText[1] = "[V]";
                        drTitleText[2] = "[G]";

                        drUnitText[0] = "%";
                        drUnitText[1] = "%";
                        drUnitText[2] = "%";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastMonLevels[guageTypes[i]];
                            drText = levels[i] < 10 ? $"0{levels[i]:N1}" : $"{levels[i]:N1}";
                            bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.RAMMonitor:
                        bitmapBuilder.DrawText("RAM(GB)", x, y, width, height, sensor.Color, titleFontSize);

                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.RAM;
                        guageTypes[1] = (Int32)LibreHardwareMonitorGaugeType.VrRAM;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.VRAM;

                        drTitleText[0] = "[S]";
                        drTitleText[1] = "[V]";
                        drTitleText[2] = "[G]";

                        drUnitText[0] = "G";
                        drUnitText[1] = "G";
                        drUnitText[2] = "G";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = i == 2 ? this._lastMonLevels[guageTypes[i]] / 1024 : this._lastMonLevels[guageTypes[i]];
                            drText = levels[i] < 10 ? $"0{levels[i]:N1}" : $"{levels[i]:N1}";
                            bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                        }

                        // for progress line.
                        guageTypes[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        guageTypes[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        guageTypes[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastLevels[guageTypes[i]];
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DT1Monitor:
                        bitmapBuilder.DrawText("DISK(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DiskT1 + i;
                            drTitleText[i] = $"[{i + 1}]";
                            drUnitText[i] = "℃";

                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastMonLevels[guageTypes[i]];
                            if (levels[i] > 0)
                            {
                                drText = levels[i] < 10 ? $"0{levels[i]:N0}" : $"{levels[i]:N0}";
                                bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DT2Monitor:
                        bitmapBuilder.DrawText("DISK(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DiskT4 + i;
                            drTitleText[i] = $"[{i + 4}]";
                            drUnitText[i] = "℃";

                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastMonLevels[guageTypes[i]];
                            if (levels[i] > 0)
                            {
                                drText = levels[i] < 10 ? $"0{levels[i]:N0}" : $"{levels[i]:N0}";
                                bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU1Monitor:
                        bitmapBuilder.DrawText("DISK(%)", x, y, width, height, sensor.Color, titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DiskU1 + i;
                            drTitleText[i] = $"[{i + 1}]";
                            drUnitText[i] = "%";

                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastMonLevels[guageTypes[i]];
                            if (levels[i] > 0)
                            {
                                drText = levels[i] < 10 ? $"0{levels[i]:N1}" : $"{levels[i]:N1}";
                                bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU2Monitor:
                        bitmapBuilder.DrawText("DISK(%)", x, y, width, height, sensor.Color, titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageTypes[i] = (Int32)LibreHardwareMonitorGaugeType.DiskU4 + i;
                            drTitleText[i] = $"[{i + 4}]";
                            drUnitText[i] = "%";

                            maxLevels[i] = this._lastMaxLevels[guageTypes[i]];
                            levels[i] = this._lastMonLevels[guageTypes[i]];
                            if (levels[i] > 0)
                            {
                                drText = levels[i] < 10 ? $"0{levels[i]:N1}" : $"{levels[i]:N1}";
                                bitmapBuilder.DrawText(drText, x + 6, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drTitleText[i], -21, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                                bitmapBuilder.DrawText(drUnitText[i], +24, ys[i], width, height, this.GetColorByLevel(levels[i], maxLevels[i], accentColor), monFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Battery:
                        bitmapBuilder.DrawText("Battery(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{levels[0]:N1}", -1, ys[2], width, height, fontColor, fontSize);
                        bitmapBuilder.DrawText("%", +18, ys[2], width, height, fontColor, fontSize);
                        break;
                }

                // Top Line
                var baseValue = 50;
                var middlePoint = TopLine[0] + (TopLine[2] - TopLine[0]) / 2;
                var startPoint = middlePoint - (TopLine[2] - TopLine[0]) * levels[2] / maxLevels[2] / 2;
                var endPoint = middlePoint + (TopLine[2] - TopLine[0]) * levels[2] / maxLevels[2] / 2;
                bitmapBuilder.DrawLine(startPoint, TopLine[1], endPoint, TopLine[3], this.GetColorByLevel(levels[2], maxLevels[2], accentColor, baseValue), 3);

                // Bottom Line
                middlePoint = BottomLine[0] + (BottomLine[2] - BottomLine[0]) / 2;
                startPoint = middlePoint - (BottomLine[2] - BottomLine[0]) * levels[2] / maxLevels[2] / 2;
                endPoint = middlePoint + (BottomLine[2] - BottomLine[0]) * levels[2] / maxLevels[2] / 2;
                bitmapBuilder.DrawLine(startPoint, BottomLine[1], endPoint, BottomLine[3], this.GetColorByLevel(levels[2], maxLevels[2], accentColor, baseValue), 3);

                // Left Line
                middlePoint = LeftLine[1] + (LeftLine[3] - LeftLine[1]) / 2;
                startPoint = middlePoint - (LeftLine[3] - LeftLine[1]) * levels[0] / maxLevels[0] / 2;
                endPoint = middlePoint + (LeftLine[3] - LeftLine[1]) * levels[0] / maxLevels[0] / 2;
                bitmapBuilder.DrawLine(LeftLine[0], startPoint, LeftLine[2], endPoint, this.GetColorByLevel(levels[0], maxLevels[0], accentColor, baseValue), 3);

                // Right Line
                middlePoint = RightLine[1] + (RightLine[3] - RightLine[1]) / 2;
                startPoint = middlePoint - (RightLine[3] - RightLine[1]) * levels[1] / maxLevels[1] / 2;
                endPoint = middlePoint + (RightLine[3] - RightLine[1]) * levels[1] / maxLevels[1] / 2;
                bitmapBuilder.DrawLine(RightLine[0], startPoint, RightLine[2], endPoint, this.GetColorByLevel(levels[1], maxLevels[1], accentColor, baseValue), 3);

                // Out Line
                bitmapBuilder.DrawRectangle(frOutline[0], frOutline[1], frOutline[2], frOutline[3], this.GetColorByLevel(levels[0], maxLevels[0], accentColor, baseValue));

                // Middle Line
                bitmapBuilder.DrawRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], this.GetColorByLevel(levels[0], maxLevels[0], accentColor, baseValue));

                return bitmapBuilder.ToImage();
            }
        }

        private void DrawImage(BitmapBuilder bitmapBuilder, Int32 imageIndex, Int32 x, Int32 y) => bitmapBuilder.DrawImage(PluginResources.ReadBinaryFile($"g{imageIndex}.png"), x, y);

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
        private void OnMonitorValuesChanged(Object sender, LibreHardwareMonitorMonitorValueChangedEventArgs e)
        {
            foreach (var monitorType in e.MonitorTypes)
            {
                if (this.UpdateMonitorIndex(monitorType))
                {
                    //PluginLog.Info("OnMonitorValuesChanged: " + monitorType.ToString());
                    this.ActionImageChanged(monitorType.ToString());
                }
            }
        }

        private void OnSensorListChanged(Object sender, EventArgs e)
        {
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPULoad);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUCore);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUPackage);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUPower);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPULoad);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUCore);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUHotspot);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUPower);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Memory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VrMemory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUMemory);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.RAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VrRAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VRAM);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskT1);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskT2);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskT3);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskT5);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskT6);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskU1);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskU2);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskU3);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskU5);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.DiskU6);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Battery);

            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.CPUMonitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.GPUMonitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.MEMMonitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.RAMMonitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.DT1Monitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.DT2Monitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.DU1Monitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.DU2Monitor);

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
                //PluginLog.Info("UpdateGaugeIndex: "+sensor.MonitorType + "," + sensor.GaugeType);
                this._lastLevels[(Int32)gaugeType] = sensor.Value;
                this._lastMinLevels[(Int32)gaugeType] = sensor.MinValue;
                this._lastMaxLevels[(Int32)gaugeType] = sensor.MaxValue;
                return true;
            }

            return false;
        }
        private Boolean UpdateMonitorIndex(LibreHardwareMonitorGaugeType monitorType)
        {
            if (!LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensorList(monitorType, out var sensorList))
            {
                //PluginLog.Info("UpdateMonitorIndex: false " + monitorType);
                return false;
            }

            var ret = false;
            foreach (var sensor in sensorList)
            {
                if (!this._lastMonLevels[(Int32)sensor.GaugeType].Equals(sensor.Value))
                {
                    //PluginLog.Info("UpdateMonitorIndex: " + sensor.MonitorType + "," + sensor.GaugeType + "," + sensor.Value);
                    this._lastMonLevels[(Int32)sensor.GaugeType] = sensor.Value;
                    this._lastMinLevels[(Int32)sensor.GaugeType] = sensor.MinValue;
                    this._lastMaxLevels[(Int32)sensor.GaugeType] = sensor.MaxValue;
                    ret = true;
                }
            }

            return ret;
        }

        private void OnHardwareMonitorProcessStarted(Object sender, EventArgs e) => this.ActionImageChanged(null);

        private void OnHardwareMonitorProcessExited(Object sender, EventArgs e) => this.ActionImageChanged(null);
    }
}
