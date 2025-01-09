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

            // Rectangle: x, y , width, height
            this.frOutline = new Int32[4] { 0, 0, 79, 79 }; // 80x80
            this.frMiddle = new Int32[4] { this.frOutline[0] + 7, this.frOutline[1] + 17, this.frOutline[2] - 7 * 2 + 1, this.frOutline[3] - 24 };

            // Line: x1, y1, x2, y2
            var offset = 0;
            this.LeftLine = new Int32[4] { this.frMiddle[0] - offset, this.frMiddle[1], this.frMiddle[0] - offset, this.frMiddle[1] + this.frMiddle[3] };
            this.RightLine = new Int32[4] { this.frMiddle[0] + this.frMiddle[2] + offset, this.frMiddle[1], this.frMiddle[0] + this.frMiddle[2] + offset, this.frMiddle[1] + this.frMiddle[3] };
            this.TopLine = new Int32[4] { this.frMiddle[0], this.frMiddle[1] - offset, this.frMiddle[0] + this.frMiddle[2], this.frMiddle[1] - offset };
            this.BottomLine = new Int32[4] { this.frMiddle[0], this.frMiddle[1] + this.frMiddle[3] + offset, this.frMiddle[0] + this.frMiddle[2], this.frMiddle[1] + this.frMiddle[3] + offset };
        }

        private readonly Int32[] frOutline;
        private readonly Int32[] frMiddle;
        private readonly Int32[] LeftLine;
        private readonly Int32[] RightLine;
        private readonly Int32[] TopLine;
        private readonly Int32[] BottomLine;

        private readonly Int32 width = 80;
        private readonly Int32 height = 11;
        private readonly Int32 titleFontSize = 12;
        private readonly Int32 fontSize = 15;
        private readonly Int32 doubleFontSize = 10;
        private readonly Int32 monFontSize = 12;
        private readonly Int32 unitFontSize = 8;
        private readonly BitmapColor fontColor = BitmapColor.White;

        private readonly String[] drTitleText = new String[3];
        private readonly String[] drUnitText = new String[3];
        private readonly Int32[] drTextY = new Int32[4];

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

        private readonly Single[] _lastMonLevel = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];
        private readonly Single[] _lastLevel = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];
        private readonly Single[] _lastMinLevels = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];
        private readonly Single[] _lastMaxLevel = new Single[(Int32)LibreHardwareMonitorGaugeType.Count];

        private Int32 GetImageIndex(LibreHardwareMonitorGaugeType guageType) => Helpers.MinMax(((Int32)this._lastLevel[(Int32)guageType] + 6) / 7, 0, 15);
        private Int32 GetImageIndexMax(LibreHardwareMonitorGaugeType guageType) => (Int32)Helpers.MinMax((100 * this._lastLevel[(Int32)guageType] / this._lastMaxLevel[(Int32)guageType] + 6) / 7, 0, 15);

        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue, BitmapColor accentColor, BitmapColor color) => new BitmapColor((100 * level / maxLevel) > 90 ? accentColor : color, Helpers.MinMax((Int32)(baseValue + (255 - baseValue) * level / maxLevel), 0, 255));
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue, BitmapColor accentColor) => new BitmapColor((100 * level / maxLevel) > 90 ? accentColor : BitmapColor.White, Helpers.MinMax((Int32)(baseValue + (255 - baseValue) * level / maxLevel), 0, 255));
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue) => this.GetColorByLevel(level, maxLevel, baseValue, BitmapColor.White);
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor) => this.GetColorByLevel(level, maxLevel, 100, accentColor);

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (!Enum.TryParse<LibreHardwareMonitorGaugeType>(actionParameter, out var gaugeType))
            {
                return PluginHelpers.GetNotAvailableButtonImage();
            }
            if (!LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensor(gaugeType, out var sensor))
            {
                if (!LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensorList(gaugeType, out var sensorList))
                {
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
                for (i = 0; i < 4; i++)
                {
                    this.drTextY[i] = 5 + 16 * (i + 1);
                }

                var maxLimit = 95;
                var guageType = new Int32[3];
                var maxLevel = new Single[3];
                var curLevel = new Single[3];

                for (i = 0; i < 3; i++)
                {
                    maxLevel[i] = this._lastMaxLevel[(Int32)gaugeType];
                    curLevel[i] = this._lastLevel[(Int32)gaugeType];
                    guageType[i] = (Int32)gaugeType;
                }

                String drText;
                var accentColor = sensor.Color;
                //var imageIndex = this.GetImageIndex(gaugeType);
                switch (gaugeType)
                {
                    // Guages
                    case LibreHardwareMonitorGaugeType.CPULoad:
                        this.DrawInit(bitmapBuilder);
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        bitmapBuilder.DrawText("CPU", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("%", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUCore:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("Core(℃)", x, y, this.width, this.height, sensor.Color, this.titleFontSize);

                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.CPUCore;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.CPUPackage;

                        maxLimit = 95;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[guageType[0]] = maxLevel[0];

                        curLevel[1] = this._lastLevel[guageType[1]];
                        maxLevel[1] = this._lastMaxLevel[guageType[1]];
                        maxLevel[1] = maxLevel[1] < maxLimit ? maxLimit : maxLevel[1];
                        this._lastMaxLevel[guageType[1]] = maxLevel[1];

                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 15, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[1]:N1}", x + 16, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);

                        this.DrawProgressBar2(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPackage:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("Pkgage", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("℃", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);

                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPower:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("Power", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("W", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);

                        maxLimit = 120;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                        maxLevel[1] = maxLevel[0];
                        maxLevel[2] = maxLevel[0];

                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.Memory:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("Mem", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("%", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.RAM:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("RAM", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.RAM;
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        bitmapBuilder.DrawText($"{curLevel[1]:N1}", x - 18, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("G", x - 6, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x + 12, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("%", x + 26, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[0]];
                            curLevel[i] = this._lastLevel[guageType[0]];
                        }
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.VrMemory:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("VrMem", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("%", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.VrRAM:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("VrRAM", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VrRAM;
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        bitmapBuilder.DrawText($"{curLevel[1]:N1}", x - 18, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("G", x - 6, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x + 12, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("%", x + 26, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[0]];
                            curLevel[i] = this._lastLevel[guageType[0]];
                        }
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.GPULoad:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("GPU", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("%", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUCore:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("Core(℃)", x, y, this.width, this.height, sensor.Color, this.titleFontSize);

                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.GPUCore;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.GPUHotspot;

                        maxLimit = 83;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[guageType[0]] = maxLevel[0];

                        curLevel[1] = this._lastLevel[guageType[1]];
                        maxLevel[1] = this._lastMaxLevel[guageType[1]];
                        maxLevel[1] = maxLevel[1] < maxLimit ? maxLimit : maxLevel[1];
                        this._lastMaxLevel[guageType[1]] = maxLevel[1];

                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 15, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[1]:N1}", x + 16, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);

                        this.DrawProgressBar2(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUHotspot:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("HSpot", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("℃", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);

                        maxLimit = 83;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                        maxLevel[1] = maxLevel[0];
                        maxLevel[2] = maxLevel[0];

                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUPower:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("Power", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("W", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);

                        maxLimit = 320;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                        maxLevel[1] = maxLevel[0];
                        maxLevel[2] = maxLevel[0];

                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUMemory:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("VMem", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("%", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                    case LibreHardwareMonitorGaugeType.VRAM:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("VRAM", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VRAM;
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        bitmapBuilder.DrawText($"{curLevel[1] / 1024:N1}", x - 18, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("G", x - 6, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x + 12, this.drTextY[2], this.width, this.height, this.fontColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("%", x + 26, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[0]];
                            curLevel[i] = this._lastLevel[guageType[0]];
                        }
                        this.DrawProgressBar1(bitmapBuilder, curLevel, maxLevel, accentColor);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    // Storages
                    case LibreHardwareMonitorGaugeType.DiskT1:
                    case LibreHardwareMonitorGaugeType.DiskT2:
                    case LibreHardwareMonitorGaugeType.DiskT3:
                    case LibreHardwareMonitorGaugeType.DiskT4:
                    case LibreHardwareMonitorGaugeType.DiskT5:
                    case LibreHardwareMonitorGaugeType.DiskT6:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText($"{gaugeType}", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N0}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("℃", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.DiskU1:
                    case LibreHardwareMonitorGaugeType.DiskU2:
                    case LibreHardwareMonitorGaugeType.DiskU3:
                    case LibreHardwareMonitorGaugeType.DiskU4:
                    case LibreHardwareMonitorGaugeType.DiskU5:
                    case LibreHardwareMonitorGaugeType.DiskU6:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText($"{gaugeType}", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("%", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    // Monitors
                    case LibreHardwareMonitorGaugeType.CPUMonitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("CPU", x, y, this.width, this.height, sensor.Color, this.titleFontSize);

                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.CPULoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.CPUCore;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.CPUPower;

                        this.drTitleText[0] = "[L]";
                        this.drTitleText[1] = "[C]";
                        this.drTitleText[2] = "[P]";

                        this.drUnitText[0] = "%";
                        this.drUnitText[1] = "℃";
                        this.drUnitText[2] = "W";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            drText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(drText, x + 8, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.GPUMonitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("GPU", x, y, this.width, this.height, sensor.Color, this.titleFontSize);

                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.GPULoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.GPUCore;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.GPUPower;

                        this.drTitleText[0] = "[L]";
                        this.drTitleText[1] = "[C]";
                        this.drTitleText[2] = "[P]";

                        this.drUnitText[0] = "%";
                        this.drUnitText[1] = "℃";
                        this.drUnitText[2] = "W";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            drText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(drText, x + 8, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.MEMMonitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("MEM", x, y, this.width, this.height, sensor.Color, this.titleFontSize);

                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;

                        this.drTitleText[0] = "[S]";
                        this.drTitleText[1] = "[V]";
                        this.drTitleText[2] = "[G]";

                        this.drUnitText[0] = "%";
                        this.drUnitText[1] = "%";
                        this.drUnitText[2] = "%";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            drText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(drText, x + 8, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.RAMMonitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("RAM", x, y, this.width, this.height, sensor.Color, this.titleFontSize);

                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.RAM;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VrRAM;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.VRAM;

                        this.drTitleText[0] = "[S]";
                        this.drTitleText[1] = "[V]";
                        this.drTitleText[2] = "[G]";

                        this.drUnitText[0] = "G";
                        this.drUnitText[1] = "G";
                        this.drUnitText[2] = "G";

                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = i == 2 ? this._lastMonLevel[guageType[i]] / 1024 : this._lastMonLevel[guageType[i]];
                            drText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(drText, x + 8, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                        }

                        // for progress line.
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.DT1Monitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("DISK", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskT1 + i;
                            this.drTitleText[i] = $"[{i + 1}]";
                            this.drUnitText[i] = "℃";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                drText = $"{curLevel[i]:N0}";
                                bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(drText, x + 10, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                            }
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.DT2Monitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("DISK", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskT4 + i;
                            this.drTitleText[i] = $"[{i + 4}]";
                            this.drUnitText[i] = "℃";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                drText = $"{curLevel[i]:N0}";
                                bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(drText, x + 10, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                            }
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.DU1Monitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("DISK", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskU1 + i;
                            this.drTitleText[i] = $"[{i + 1}]";
                            this.drUnitText[i] = "%";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                drText = $"{curLevel[i]:N1}";
                                bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(drText, x + 10, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                            }
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.DU2Monitor:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("DISK", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskU4 + i;
                            this.drTitleText[i] = $"[{i + 4}]";
                            this.drUnitText[i] = "%";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                drText = $"{curLevel[i]:N1}";
                                bitmapBuilder.DrawText(this.drTitleText[i], x - 21, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(drText, x + 10, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], x + 25, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], accentColor), this.unitFontSize);
                            }
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;

                    case LibreHardwareMonitorGaugeType.Battery:
                        this.DrawInit(bitmapBuilder);
                        bitmapBuilder.DrawText("Battery", x, y, this.width, this.height, sensor.Color, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", x - 1, this.drTextY[2], this.width, this.height, this.fontColor, this.fontSize);
                        bitmapBuilder.DrawText("%", x + 18, this.drTextY[2], this.width, this.height, this.fontColor, this.unitFontSize);
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor);
                        break;
                }

                return bitmapBuilder.ToImage();
            }
        }
        private void DrawInit(BitmapBuilder bitmapBuilder)
        {
            var rgb = 55;
            bitmapBuilder.Clear(new BitmapColor(rgb,rgb,rgb));
            bitmapBuilder.FillRectangle(this.frMiddle[0], this.frMiddle[1], this.frMiddle[2], this.frMiddle[3], BitmapColor.Black);
        }

        private void DrawGuage(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor accentColor)
        {
            var red = new BitmapColor(255, 0, 0);
            var baseValue = 50;

            // Top Line
            var level = curLevel[2] / maxLevel[2];
            var middlePoint = this.TopLine[0] + (this.TopLine[2] - this.TopLine[0]) / 2;
            var startPoint = middlePoint - (this.TopLine[2] - this.TopLine[0]) * level / 2;
            var endPoint = middlePoint + (this.TopLine[2] - this.TopLine[0]) * level / 2;
            bitmapBuilder.DrawLine(startPoint, this.TopLine[1], endPoint, this.TopLine[3], this.GetColorByLevel(curLevel[2], maxLevel[2], baseValue, red, accentColor), 3);

            // Bottom Line
            level = curLevel[2] / maxLevel[2];
            middlePoint = this.BottomLine[0] + (this.BottomLine[2] - this.BottomLine[0]) / 2;
            startPoint = middlePoint - (this.BottomLine[2] - this.BottomLine[0]) * level / 2;
            endPoint = middlePoint + (this.BottomLine[2] - this.BottomLine[0]) * level / 2;
            bitmapBuilder.DrawLine(startPoint, this.BottomLine[1], endPoint, this.BottomLine[3], this.GetColorByLevel(curLevel[2], maxLevel[2], baseValue, red, accentColor), 3);

            // Left Line
            level = curLevel[0] / maxLevel[0];
            middlePoint = this.LeftLine[1] + (this.LeftLine[3] - this.LeftLine[1]) / 2;
            startPoint = middlePoint - (this.LeftLine[3] - this.LeftLine[1]) * level / 2;
            endPoint = middlePoint + (this.LeftLine[3] - this.LeftLine[1]) * level / 2;
            bitmapBuilder.DrawLine(this.LeftLine[0], startPoint, this.LeftLine[2], endPoint, this.GetColorByLevel(curLevel[0], maxLevel[0], baseValue, red, accentColor), 3);

            // Right Line
            level = curLevel[1] / maxLevel[1];
            middlePoint = this.RightLine[1] + (this.RightLine[3] - this.RightLine[1]) / 2;
            startPoint = middlePoint - (this.RightLine[3] - this.RightLine[1]) * level / 2;
            endPoint = middlePoint + (this.RightLine[3] - this.RightLine[1]) * level / 2;
            bitmapBuilder.DrawLine(this.RightLine[0], startPoint, this.RightLine[2], endPoint, this.GetColorByLevel(curLevel[1], maxLevel[1], baseValue, red, accentColor), 3);

            baseValue = 100;

            // Out Line
            bitmapBuilder.DrawRectangle(this.frOutline[0], this.frOutline[1], this.frOutline[2], this.frOutline[3], this.GetColorByLevel(curLevel[0], maxLevel[0], baseValue));

            // Middle Line
            bitmapBuilder.DrawRectangle(this.frMiddle[0], this.frMiddle[1], this.frMiddle[2], this.frMiddle[3], this.GetColorByLevel(curLevel[0], maxLevel[0], baseValue));
        }

        private void DrawProgressBar1(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor accentColor)
        {
            //bitmapBuilder.DrawImage(PluginResources.ReadBinaryFile($"g{imageIndex}.png"), 0, this.drTextY[0] - 12);
            bitmapBuilder.DrawImage(PluginResources.ReadBinaryFile($"g0.png"), 0, this.drTextY[0] - 12);

            const Single width = 52;
            const Single height = 20;
            const Single x1 = 14;
            const Single y1 = 26;
            const Single y2 = y1 + height;

            var level = curLevel[0] / maxLevel[0];
            var frH = 1 + height * level;
            var frY = 1 + y2 - frH;

            var red = new BitmapColor(255, 0, 0);
            accentColor = level > 0.9 ? red : accentColor;
            bitmapBuilder.FillRectangle((Int32)x1, (Int32)frY, (Int32)width, (Int32)frH, new BitmapColor(accentColor, 200));
        }
        private void DrawProgressBar2(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor accentColor)
        {
            bitmapBuilder.DrawImage(PluginResources.ReadBinaryFile($"g0.png"), 0, this.drTextY[0] - 12);

            const Single width = 22;
            const Single height = 20;
            const Single x1 = 14;
            const Single y1 = 26;
            const Single y2 = y1 + height;

            var level = curLevel[0] / maxLevel[0];
            var frH = 1 + height * level;
            var frY = 1 + y2 - frH;

            var red = new BitmapColor(255, 0, 0);
            accentColor = level > 0.9 ? red : accentColor;
            bitmapBuilder.FillRectangle((Int32)x1, (Int32)frY, (Int32)width, (Int32)frH, new BitmapColor(accentColor, 200));

            level = curLevel[1] / maxLevel[1];
            frH = 1 + height * level;
            frY = 1 + y2 - frH;

            accentColor = level > 0.9 ? red : accentColor;
            bitmapBuilder.FillRectangle((Int32)(x1 + width + 8), (Int32)frY, (Int32)width, (Int32)frH, new BitmapColor(accentColor, 200));
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

            if (!this._lastLevel[(Int32)gaugeType].Equals(sensor.Value))
            {
                //PluginLog.Info("UpdateGaugeIndex: "+sensor.MonitorType + "," + sensor.GaugeType);
                this._lastLevel[(Int32)gaugeType] = sensor.Value;
                this._lastMinLevels[(Int32)gaugeType] = sensor.MinValue;
                this._lastMaxLevel[(Int32)gaugeType] = sensor.MaxValue;
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
                if (!this._lastMonLevel[(Int32)sensor.GaugeType].Equals(sensor.Value))
                {
                    //PluginLog.Info("UpdateMonitorIndex: " + sensor.MonitorType + "," + sensor.GaugeType + "," + sensor.Value);
                    this._lastMonLevel[(Int32)sensor.GaugeType] = sensor.Value;
                    this._lastMinLevels[(Int32)sensor.GaugeType] = sensor.MinValue;
                    this._lastMaxLevel[(Int32)sensor.GaugeType] = sensor.MaxValue;
                    ret = true;
                }
            }

            return ret;
        }

        private void OnHardwareMonitorProcessStarted(Object sender, EventArgs e) => this.ActionImageChanged(null);

        private void OnHardwareMonitorProcessExited(Object sender, EventArgs e) => this.ActionImageChanged(null);
    }
}
