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

            //AddParameter(LibreHardwareMonitorGaugeType.MemLoad, this.GroupName);
            //AddParameter(LibreHardwareMonitorGaugeType.VMemLoad, this.GroupName);
            //AddParameter(LibreHardwareMonitorGaugeType.GPUMemLoad, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.Memory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.VirtualMemory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPUMemory, this.GroupName);

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
            AddParameter(LibreHardwareMonitorGaugeType.MemoryLoadMonitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.MemoryMonitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DT1Monitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DT2Monitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DU1Monitor, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.DU2Monitor, GroupMonitor);

            void AddParameter(LibreHardwareMonitorGaugeType gaugeType, String GroupName) => this.AddParameter(gaugeType.ToString(), gaugeType.ToString(), GroupName);

            // Rectangle: x, y , width, height
            this.frOutline = new Int32[4] { 0, 0, 78, 78 }; // 80x80
            this.frMiddle = new Int32[4] { this.frOutline[0] + 8, this.frOutline[1] + 8 * 2 + 3, this.frOutline[2] - 8 * 2, this.frOutline[3] - 8 * 3 };

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
        private readonly Int32 doubleFontSize = 14;
        private readonly Int32 monFontSize = 12;
        private readonly Int32 unitFontSize = 10;
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

        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue, BitmapColor accentColor, BitmapColor color) => new BitmapColor(level / maxLevel > 0.9 ? accentColor : color, Helpers.MinMax((Int32)(baseValue + (255 - baseValue) * level / maxLevel), 0, 255));
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue, BitmapColor accentColor) => new BitmapColor(level / maxLevel > 0.9 ? new BitmapColor(255, 30, 30) : accentColor, Helpers.MinMax((Int32)(baseValue + (255 - baseValue) * level / maxLevel), 0, 255));
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue) => this.GetColorByLevel(level, maxLevel, baseValue, BitmapColor.White);
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor) => this.GetColorByLevel(level, maxLevel, 200, accentColor);

        private void DrawGuage(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor accentColor, Int32 barCount)
        {
            bitmapBuilder.Clear(BitmapColor.Black);

            var alpha = 70 * 2.55f;
            var color = new BitmapColor(accentColor, (Int32)alpha);
            bitmapBuilder.FillRectangle(this.frOutline[0], this.frOutline[1], this.frOutline[2], this.frOutline[1] + 15, color);
            alpha = 15 * 2.55f;
            color = new BitmapColor(accentColor, (Int32)alpha);
            bitmapBuilder.FillRectangle(this.frOutline[0], this.frOutline[1] + 17, this.frOutline[2], this.frOutline[3], color);

            alpha = 70 * 2.55f;
            this.DrawOutline(bitmapBuilder, curLevel, maxLevel, new BitmapColor(accentColor, (Int32)alpha));

            bitmapBuilder.DrawRectangle(this.frOutline[0], this.frOutline[1], this.frOutline[2], this.frOutline[3], accentColor);
            bitmapBuilder.FillRectangle(this.frMiddle[0], this.frMiddle[1], this.frMiddle[2], this.frMiddle[3], BitmapColor.Black);

            if (barCount == 2)
            {
                this.DrawBar2(bitmapBuilder, curLevel, maxLevel, accentColor, 100, 125);
            }
            else
            {
                this.DrawBar1(bitmapBuilder, curLevel, maxLevel, accentColor, 100);
            }
        }

        private void DrawBar1(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor accentColor, Int32 alpha)
        {
            Single width = this.frMiddle[2] - 3;
            Single height = this.frMiddle[3] - 4;
            Single x1 = this.frMiddle[0] + 2;
            Single y1 = this.frMiddle[1] + 2;
            var y2 = y1 + height;

            var level = curLevel[0] / maxLevel[0];
            var lh = height * level;
            var ly = 1 + y2 - lh;

            bitmapBuilder.FillRectangle((Int32)x1, (Int32)ly, (Int32)width, (Int32)lh, new BitmapColor(accentColor, 50));
            bitmapBuilder.DrawRectangle(this.frMiddle[0], this.frMiddle[1], this.frMiddle[2], this.frMiddle[3], accentColor);
        }

        private void DrawBar2(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor accentColor, Int32 alpha1, Int32 alpha2)
        {
            var width = this.frMiddle[2] * 0.5f - 4;
            Single height = this.frMiddle[3] - 4;
            Single x1 = this.frMiddle[0] + 2;
            Single y1 = this.frMiddle[1] + 2;
            var y2 = y1 + height;

            var level = curLevel[0] / maxLevel[0];
            var frH = height * level;
            var frY = 1 + y2 - frH;

            bitmapBuilder.FillRectangle((Int32)x1, (Int32)frY, (Int32)width, (Int32)frH, new BitmapColor(accentColor, alpha1));
            bitmapBuilder.DrawRectangle(this.frMiddle[0], this.frMiddle[1], this.frMiddle[2] / 2 - 1, this.frMiddle[3], accentColor);

            level = curLevel[1] / maxLevel[1];
            frH = height * level;
            frY = 1 + y2 - frH;

            bitmapBuilder.FillRectangle((Int32)(x1 + width + 5), (Int32)frY, (Int32)width, (Int32)frH, new BitmapColor(accentColor, alpha2));
            bitmapBuilder.DrawRectangle((Int32)(this.frMiddle[0] + width + 5), this.frMiddle[1], this.frMiddle[2] / 2 - 1, this.frMiddle[3], accentColor);
        }

        private void DrawOutline(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor color)
        {
            //var red = new BitmapColor(255, 0, 0);
            //var baseValue = 50;

            // Top Line
            //var level = curLevel[2] / maxLevel[2];
            //var middlePoint = this.TopLine[0] + (this.TopLine[2] - this.TopLine[0]) / 2;
            //var startPoint = middlePoint - (this.TopLine[2] - this.TopLine[0]) * level / 2;
            //var endPoint = middlePoint + (this.TopLine[2] - this.TopLine[0]) * level / 2;
            //bitmapBuilder.DrawLine(startPoint, this.TopLine[1], endPoint, this.TopLine[3], color, 3);

            // Bottom Line
            //level = curLevel[2] / maxLevel[2];
            //middlePoint = this.BottomLine[0] + (this.BottomLine[2] - this.BottomLine[0]) / 2;
            //startPoint = middlePoint - (this.BottomLine[2] - this.BottomLine[0]) * level / 2;
            //endPoint = middlePoint + (this.BottomLine[2] - this.BottomLine[0]) * level / 2;
            //bitmapBuilder.DrawLine(startPoint, this.BottomLine[1], endPoint, this.BottomLine[3], color, 3);

            // Left Line
            var level = curLevel[0] / maxLevel[0];
            var middlePoint = this.LeftLine[1] + (this.LeftLine[3] - this.LeftLine[1]) / 2;
            var startPoint = middlePoint - (this.LeftLine[3] - this.LeftLine[1]) * level / 2;
            var endPoint = middlePoint + (this.LeftLine[3] - this.LeftLine[1]) * level / 2 + 1;
            bitmapBuilder.DrawLine(this.LeftLine[0] - 3, startPoint, this.LeftLine[2] - 3, endPoint, color, 6);

            // Right Line
            //level = curLevel[1] / maxLevel[1];
            middlePoint = this.RightLine[1] + (this.RightLine[3] - this.RightLine[1]) / 2;
            startPoint = middlePoint - (this.RightLine[3] - this.RightLine[1]) * level / 2;
            endPoint = middlePoint + (this.RightLine[3] - this.RightLine[1]) * level / 2 + 1;
            bitmapBuilder.DrawLine(this.RightLine[0] + 4, startPoint, this.RightLine[2] + 4, endPoint, color, 6);
        }

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
                String drText;
                var maxLimit = 95;

                var accentColor = sensor.Color;
                if (gaugeType == LibreHardwareMonitorGaugeType.MemoryMonitor || gaugeType == LibreHardwareMonitorGaugeType.MemoryLoadMonitor)
                {
                    accentColor = new BitmapColor(253, 100, 150);
                }
                var titleColor = BitmapColor.White;
                var valueColor = BitmapColor.White;
                var monTitleColor = accentColor;
                var monValueColor = BitmapColor.White;
                var unitColor = accentColor;
                var guageType = new Int32[3];
                var maxLevel = new Single[3];
                var curLevel = new Single[3];
                var titleX = 0;
                var titleY = 0;
                var unitX = 20;
                var unitY = 3;

                var i = 0;
                for (i = 0; i < 4; i++)
                {
                    if (i < 3)
                    {
                        maxLevel[i] = this._lastMaxLevel[(Int32)gaugeType];
                        curLevel[i] = this._lastLevel[(Int32)gaugeType];
                        guageType[i] = (Int32)gaugeType;
                    }
                    this.drTextY[i] = 6 + 16 * (i + 1);
                }

                switch (gaugeType)
                {
                    // Guages
                    case LibreHardwareMonitorGaugeType.CPULoad:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("CPU Load", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText(curLevel[0] > 99.9 ? $"{curLevel[0]:N0}" : $"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("%", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.CPUCore:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.CPUCore;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.CPUPackage;

                        maxLimit = 95;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[guageType[0]] = maxLevel[0];

                        curLevel[1] = this._lastLevel[guageType[1]];
                        maxLevel[1] = this._lastMaxLevel[guageType[1]];
                        maxLevel[1] = maxLevel[1] < maxLimit ? maxLimit : maxLevel[1];
                        this._lastMaxLevel[guageType[1]] = maxLevel[1];

                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 2);
                        bitmapBuilder.DrawText("CPU Core", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N0}", titleX - 15, this.drTextY[1], this.width, this.height, valueColor, this.doubleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[1]:N0}", titleX + 15, this.drTextY[1], this.width, this.height, valueColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("℃", unitX - 25, this.drTextY[1] + unitY - 14, this.width, this.height, unitColor, this.unitFontSize);
                        bitmapBuilder.DrawText("℃", unitX + 3, this.drTextY[1] + unitY - 14, this.width, this.height, unitColor, this.unitFontSize);

                        break;
                    case LibreHardwareMonitorGaugeType.CPUPackage:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("CPU Pkgage", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize - 2);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("℃", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPower:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("CPU Power", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("W", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);

                        maxLimit = 120;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                        maxLevel[1] = maxLevel[0];
                        maxLevel[2] = maxLevel[0];
                        break;
                    case LibreHardwareMonitorGaugeType.MemoryLoad:
                    case LibreHardwareMonitorGaugeType.Memory:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.MemoryLoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[0]];
                            curLevel[i] = this._lastLevel[guageType[0]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        bitmapBuilder.DrawText("Memory", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        i = 5;
                        bitmapBuilder.DrawText($"{curLevel[1]:N1}", titleX, this.drTextY[1] - i, this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("G", unitX, this.drTextY[1] - i + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX, this.drTextY[2] - i, this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("%", unitX, this.drTextY[2] - i + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.VirtualMemoryLoad:
                    case LibreHardwareMonitorGaugeType.VirtualMemory:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.VirtualMemoryLoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VirtualMemory;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[0]];
                            curLevel[i] = this._lastLevel[guageType[0]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        bitmapBuilder.DrawText("Virtual Memory", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize - 3);
                        i = 5;
                        bitmapBuilder.DrawText($"{curLevel[1]:N1}", titleX, this.drTextY[1] - i, this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("G", unitX, this.drTextY[1] - i + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX, this.drTextY[2] - i, this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("%", unitX, this.drTextY[2] - i + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.GPULoad:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("GPU Load", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("%", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUCore:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.GPUCore;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.GPUHotspot;

                        maxLimit = 83;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[guageType[0]] = maxLevel[0];

                        curLevel[1] = this._lastLevel[guageType[1]];
                        maxLevel[1] = this._lastMaxLevel[guageType[1]];
                        maxLevel[1] = maxLevel[1] < maxLimit ? maxLimit : maxLevel[1];
                        this._lastMaxLevel[guageType[1]] = maxLevel[1];

                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 2);
                        bitmapBuilder.DrawText("GPU Core", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N0}", titleX - 15, this.drTextY[1], this.width, this.height, valueColor, this.doubleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[1]:N0}", titleX + 15, this.drTextY[1], this.width, this.height, valueColor, this.doubleFontSize);
                        bitmapBuilder.DrawText("℃", unitX - 25, this.drTextY[1] + unitY - 14, this.width, this.height, unitColor, this.unitFontSize);
                        bitmapBuilder.DrawText("℃", unitX + 3, this.drTextY[1] + unitY - 14, this.width, this.height, unitColor, this.unitFontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUHotspot:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("GPU HotSpot", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize - 2);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("℃", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);

                        maxLimit = 83;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                        maxLevel[1] = maxLevel[0];
                        maxLevel[2] = maxLevel[0];
                        break;
                    case LibreHardwareMonitorGaugeType.GPUPower:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("GPU Power", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("W", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);

                        maxLimit = 320;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                        maxLevel[1] = maxLevel[0];
                        maxLevel[2] = maxLevel[0];
                        break;
                    case LibreHardwareMonitorGaugeType.GPUMemoryLoad:
                    case LibreHardwareMonitorGaugeType.GPUMemory:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.GPUMemoryLoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[0]];
                            curLevel[i] = this._lastLevel[guageType[0]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        bitmapBuilder.DrawText("GPU Memory", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize - 2);
                        i = 5;
                        bitmapBuilder.DrawText($"{curLevel[1] / 1024:N1}", titleX, this.drTextY[1] - i, this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("G", unitX, this.drTextY[1] - i + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX, this.drTextY[2] - i, this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("%", unitX, this.drTextY[2] - i + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;

                    // Storages
                    case LibreHardwareMonitorGaugeType.DiskT1:
                    case LibreHardwareMonitorGaugeType.DiskT2:
                    case LibreHardwareMonitorGaugeType.DiskT3:
                    case LibreHardwareMonitorGaugeType.DiskT4:
                    case LibreHardwareMonitorGaugeType.DiskT5:
                    case LibreHardwareMonitorGaugeType.DiskT6:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText($"{gaugeType}", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N0}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("℃", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.DiskU1:
                    case LibreHardwareMonitorGaugeType.DiskU2:
                    case LibreHardwareMonitorGaugeType.DiskU3:
                    case LibreHardwareMonitorGaugeType.DiskU4:
                    case LibreHardwareMonitorGaugeType.DiskU5:
                    case LibreHardwareMonitorGaugeType.DiskU6:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText($"{gaugeType}", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("%", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;

                    // Monitors
                    case LibreHardwareMonitorGaugeType.CPUMonitor:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.CPULoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.CPUCore;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.CPUPower;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("CPU Monitor", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize - 2);
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
                            drText = i == 0 && curLevel[i] > 99.9 ? $"{curLevel[i]:N0}" : $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                            bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.GPUMonitor:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.GPULoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.GPUCore;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.GPUPower;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("GPU Monitor", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize - 2);
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
                            drText = i == 0 && curLevel[i] > 99.9 ? $"{curLevel[i]:N0}" : $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                            bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.MemoryLoadMonitor:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.MemoryLoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VirtualMemoryLoad;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemoryLoad;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("Memory Load", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize - 2);
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
                            bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                            bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.MemoryMonitor:
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.MemoryLoad;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VirtualMemoryLoad;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemoryLoad;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastLevel[guageType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        guageType[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        guageType[1] = (Int32)LibreHardwareMonitorGaugeType.VirtualMemory;
                        guageType[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        bitmapBuilder.DrawText("Memory", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
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
                            bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                            bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                            bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DT1Monitor:
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskT1 + i;
                            this.drTitleText[i] = $"[{i + 1}]";
                            this.drUnitText[i] = "℃";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                if (i == 0)
                                {
                                    this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                                    bitmapBuilder.DrawText("Storage", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                                }
                                drText = $"{curLevel[i]:N0}";
                                bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                                bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DT2Monitor:
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskT4 + i;
                            this.drTitleText[i] = $"[{i + 4}]";
                            this.drUnitText[i] = "℃";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                if (i == 0)
                                {
                                    this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                                    bitmapBuilder.DrawText("Storage", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                                }
                                drText = $"{curLevel[i]:N0}";
                                bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                                bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU1Monitor:
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskU1 + i;
                            this.drTitleText[i] = $"[{i + 1}]";
                            this.drUnitText[i] = "%";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                if (i == 0)
                                {
                                    this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                                    bitmapBuilder.DrawText("Storage", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                                }
                                drText = $"{curLevel[i]:N1}";
                                bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                                bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU2Monitor:
                        for (i = 0; i < 3; i++)
                        {
                            guageType[i] = (Int32)LibreHardwareMonitorGaugeType.DiskU4 + i;
                            this.drTitleText[i] = $"[{i + 4}]";
                            this.drUnitText[i] = "%";

                            maxLevel[i] = this._lastMaxLevel[guageType[i]];
                            curLevel[i] = this._lastMonLevel[guageType[i]];
                            if (curLevel[i] > 0)
                            {
                                if (i == 0)
                                {
                                    this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                                    bitmapBuilder.DrawText("Storage", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                                }
                                drText = $"{curLevel[i]:N1}";
                                bitmapBuilder.DrawText(this.drTitleText[i], titleX - 21, this.drTextY[i], this.width, this.height, monTitleColor, this.monFontSize);
                                bitmapBuilder.DrawText(drText, titleX + 6, this.drTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), this.monFontSize);
                                bitmapBuilder.DrawText(this.drUnitText[i], titleX + 23, this.drTextY[i] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Battery:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("Battery", titleX, titleY, this.width, this.height, titleColor, this.titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, this.drTextY[1], this.width, this.height, valueColor, this.fontSize);
                        bitmapBuilder.DrawText("%", unitX, this.drTextY[1] + unitY, this.width, this.height, unitColor, this.unitFontSize);
                        break;
                }

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

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.MemoryLoad);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VirtualMemoryLoad);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUMemoryLoad);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Memory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VirtualMemory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUMemory);

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
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.MemoryLoadMonitor);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.MemoryMonitor);
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
