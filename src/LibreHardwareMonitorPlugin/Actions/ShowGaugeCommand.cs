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

            AddParameter(LibreHardwareMonitorGaugeType.CPU_Load, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPU_Core, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPU_Package, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.CPU_Power, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.GPU_Load, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPU_Core, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPU_Hotspot, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPU_Power, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.Memory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.Virtual_Memory, this.GroupName);
            AddParameter(LibreHardwareMonitorGaugeType.GPU_Memory, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.Storage_T_1, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_T_2, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_T_3, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_T_4, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_T_5, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_T_6, GroupStorage);

            AddParameter(LibreHardwareMonitorGaugeType.Storage_U_1, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_U_2, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_U_3, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_U_4, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_U_5, GroupStorage);
            AddParameter(LibreHardwareMonitorGaugeType.Storage_U_6, GroupStorage);

            AddParameter(LibreHardwareMonitorGaugeType.Battery, this.GroupName);

            AddParameter(LibreHardwareMonitorGaugeType.Mon_CPU, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Mon_GPU, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Mon_Memory_Load, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Mon_Memory, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Mon_Storage_T_G1, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Mon_Storage_T_G2, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Mon_Storage_U_G1, GroupMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Mon_Storage_U_G2, GroupMonitor);

            void AddParameter(LibreHardwareMonitorGaugeType gaugeType, String GroupName) => this.AddParameter(gaugeType.ToString(), gaugeType.ToString().Replace('_', ' '), GroupName);

            // Rectangle: x, y , width, height
            this.frOutline = new Int32[4] { 0, 0, 78, 78 }; // 80x80
            this.middleSqureCoordinates = new Int32[4] { this.frOutline[0] + 8, this.frOutline[1] + 8 * 2 + 3, this.frOutline[2] - 8 * 2, this.frOutline[3] - 8 * 3 };

            // Line: x1, y1, x2, y2
            var offset = 0;
            this.LeftLine = new Int32[4] { this.middleSqureCoordinates[0] - offset, this.middleSqureCoordinates[1], this.middleSqureCoordinates[0] - offset, this.middleSqureCoordinates[1] + this.middleSqureCoordinates[3] };
            this.RightLine = new Int32[4] { this.middleSqureCoordinates[0] + this.middleSqureCoordinates[2] + offset, this.middleSqureCoordinates[1], this.middleSqureCoordinates[0] + this.middleSqureCoordinates[2] + offset, this.middleSqureCoordinates[1] + this.middleSqureCoordinates[3] };
            this.TopLine = new Int32[4] { this.middleSqureCoordinates[0], this.middleSqureCoordinates[1] - offset, this.middleSqureCoordinates[0] + this.middleSqureCoordinates[2], this.middleSqureCoordinates[1] - offset };
            this.BottomLine = new Int32[4] { this.middleSqureCoordinates[0], this.middleSqureCoordinates[1] + this.middleSqureCoordinates[3] + offset, this.middleSqureCoordinates[0] + this.middleSqureCoordinates[2], this.middleSqureCoordinates[1] + this.middleSqureCoordinates[3] + offset };
        }

        private readonly Int32[] frOutline;
        private readonly Int32[] middleSqureCoordinates;
        private readonly Int32[] LeftLine;
        private readonly Int32[] RightLine;
        private readonly Int32[] TopLine;
        private readonly Int32[] BottomLine;

        private readonly Int32 width = 80;
        private readonly Int32 height = 11;

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

        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue, BitmapColor accentColor) => new BitmapColor(level / maxLevel > 0.9 ? new BitmapColor(255, 30, 30) : accentColor, Helpers.MinMax((Int32)(baseValue + (255 - baseValue) * level / maxLevel), 0, 255));
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor) => this.GetColorByLevel(level, maxLevel, 200, accentColor);

        private Int32 GetAlpha(Int32 rate) => rate * 255 / 100;

        private void DrawGuage(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor accentColor, Int32 barCount)
        {
            bitmapBuilder.Clear(BitmapColor.Black);

            var color1 = new BitmapColor(accentColor, this.GetAlpha(80));
            var color2 = new BitmapColor(accentColor, this.GetAlpha(15));
            bitmapBuilder.FillRectangle(this.frOutline[0], this.frOutline[1], this.frOutline[2], this.frOutline[1] + 15, BitmapColor.White);
            bitmapBuilder.FillRectangle(this.frOutline[0], this.frOutline[1], this.frOutline[2], this.frOutline[1] + 15, color1);
            bitmapBuilder.FillRectangle(this.frOutline[0], this.frOutline[1] + 17, this.frOutline[2], this.frOutline[3], color2);
            bitmapBuilder.DrawRectangle(this.frOutline[0], this.frOutline[1], this.frOutline[2], this.frOutline[3], BitmapColor.White);
            bitmapBuilder.DrawRectangle(this.frOutline[0], this.frOutline[1], this.frOutline[2], this.frOutline[3], color1);

            var leftBarColor = new BitmapColor(accentColor, this.GetAlpha(60));
            var rightBarColor = new BitmapColor(accentColor, this.GetAlpha(60));
            var leftLineColor = new BitmapColor(accentColor, this.GetAlpha(80));
            var rightLineColor = new BitmapColor(accentColor, this.GetAlpha(80));
            if (barCount == 2)
            {
                this.DrawOutline(bitmapBuilder, curLevel, maxLevel, rightBarColor, leftBarColor);
                this.DrawBar(bitmapBuilder, curLevel, maxLevel, leftBarColor, rightBarColor, leftLineColor, rightLineColor);
            }
            else
            {
                this.DrawOutline(bitmapBuilder, curLevel, maxLevel, leftBarColor, leftBarColor);
                this.DrawBar(bitmapBuilder, curLevel, maxLevel, leftBarColor, leftLineColor);
            }
        }

        private void DrawBar(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor barColor, BitmapColor lineColor)
        {
            var x = this.middleSqureCoordinates[0] + 2;
            var y = this.middleSqureCoordinates[1] + 2;
            var w = this.middleSqureCoordinates[2] - 3;
            var h = this.middleSqureCoordinates[3] - 4;

            bitmapBuilder.FillRectangle(this.middleSqureCoordinates[0],
                                        this.middleSqureCoordinates[1],
                                        this.middleSqureCoordinates[2],
                                        this.middleSqureCoordinates[3],
                                        BitmapColor.Black);

            this.GetRectangleYHByLevel(curLevel[0] / maxLevel[0], y, h, out var bottomY, out var bottomH);
            bitmapBuilder.FillRectangle(x, bottomY, w, bottomH, BitmapColor.White);
            bitmapBuilder.FillRectangle(x, bottomY, w, bottomH, barColor);
            bitmapBuilder.DrawRectangle(this.middleSqureCoordinates[0],
                                        this.middleSqureCoordinates[1],
                                        this.middleSqureCoordinates[2],
                                        this.middleSqureCoordinates[3],
                                        BitmapColor.White);
            bitmapBuilder.DrawRectangle(this.middleSqureCoordinates[0],
                                        this.middleSqureCoordinates[1],
                                        this.middleSqureCoordinates[2],
                                        this.middleSqureCoordinates[3],
                                        lineColor);
        }

        private void DrawBar(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor leftBarColor, BitmapColor rightBarColor, BitmapColor leftLineColor, BitmapColor rightLineColor)
        {
            var x = this.middleSqureCoordinates[0] + 2;
            var y = this.middleSqureCoordinates[1] + 2;
            var w = this.middleSqureCoordinates[2] / 2 - 4;
            var h = this.middleSqureCoordinates[3] - 4;

            var lx = this.middleSqureCoordinates[0];
            var ly = this.middleSqureCoordinates[1];
            var lw = this.middleSqureCoordinates[2] / 2 - 1;
            var lh = this.middleSqureCoordinates[3];

            bitmapBuilder.FillRectangle(this.middleSqureCoordinates[0],
                                        this.middleSqureCoordinates[1],
                                        this.middleSqureCoordinates[2],
                                        this.middleSqureCoordinates[3],
                                        BitmapColor.Black);
            // left
            this.GetRectangleYHByLevel(curLevel[0] / maxLevel[0], y, h, out var bottomLY, out var bottomLH);
            bitmapBuilder.FillRectangle(x, bottomLY, w, bottomLH, leftBarColor);
            bitmapBuilder.DrawRectangle(lx, ly, lw, lh, BitmapColor.White);
            bitmapBuilder.DrawRectangle(lx, ly, lw, lh, leftLineColor);

            // right
            this.GetRectangleYHByLevel(curLevel[1] / maxLevel[1], y, h, out var bottomRY, out var bottomRH);
            bitmapBuilder.FillRectangle(x + w + 5, bottomRY, w, bottomRH, BitmapColor.White);
            bitmapBuilder.FillRectangle(x + w + 5, bottomRY, w, bottomRH, rightBarColor);
            bitmapBuilder.DrawRectangle(lx + w + 5, ly, lw, lh, BitmapColor.White);
            bitmapBuilder.DrawRectangle(lx + w + 5, ly, lw, lh, rightLineColor);
        }

        private void GetRectangleYHByLevel(Single level, Int32 y, Int32 height, out Int32 bottomY, out Int32 bottomH)
        {
            // y1 = y2 - height;
            // y2 = y1 + height;
            bottomH = (Int32)(height * level);
            bottomY = height + y - bottomH;
        }
        private void GetLineY1Y2ByLevel(Single level, Int32 y1, Int32 y2, out Int32 middleY1, out Int32 middleY2)
        {
            var halfHeight = (y2 - y1) / 2;
            var middleY = y1 + halfHeight;
            var levelH = halfHeight * level;
            middleY1 = (Int32)(middleY - levelH);
            middleY2 = (Int32)(middleY + levelH);
        }

        private void DrawOutline(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, BitmapColor leftColor, BitmapColor rightColor)
        {
            // Left Line: x1, y1, x2, y2
            var lx1 = this.LeftLine[0] - 3;
            var ly1 = this.LeftLine[1];
            var lx2 = this.LeftLine[2] - 3;
            var ly2 = this.LeftLine[3];
            this.GetLineY1Y2ByLevel(curLevel[0] / maxLevel[0], ly1, ly2, out var middleLY1, out var middleLY2);
            bitmapBuilder.DrawLine(lx1, middleLY1, lx2, middleLY2, BitmapColor.White, 4);
            bitmapBuilder.DrawLine(lx1, middleLY1, lx2, middleLY2, leftColor, 4);

            // Right Line: x1, y1, x2, y2
            var rx1 = this.RightLine[0] + 4;
            var ry1 = this.RightLine[1];
            var rx2 = this.RightLine[2] + 4;
            var ry2 = this.RightLine[3];
            this.GetLineY1Y2ByLevel(curLevel[0] / maxLevel[0], ry1, ry2, out var middleRY1, out var middleRY2);
            bitmapBuilder.DrawLine(rx1, middleRY1, rx2, middleRY2, BitmapColor.White, 4);
            bitmapBuilder.DrawLine(rx1, middleRY1, rx2, middleRY2, rightColor, 4);
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
                var displayName = gaugeType.ToString().Replace('_', ' ');
                var accentColor = (gaugeType == LibreHardwareMonitorGaugeType.Mon_Memory
                                   || gaugeType == LibreHardwareMonitorGaugeType.Mon_Memory_Load) ? new BitmapColor(253, 100, 150) : sensor.Color;
                var titleColor = BitmapColor.Black;
                var valueColor = BitmapColor.White;
                var unitColor = accentColor;
                var monTitleColor = accentColor;
                var monValueColor = BitmapColor.White;

                var maxLimit = 95;
                var maxLevel = new[] { this._lastMaxLevel[(Int32)gaugeType], this._lastMaxLevel[(Int32)gaugeType], this._lastMaxLevel[(Int32)gaugeType] };
                var curLevel = new[] { this._lastLevel[(Int32)gaugeType], this._lastLevel[(Int32)gaugeType], this._lastLevel[(Int32)gaugeType] };
                var monType = new[] { (Int32)gaugeType, (Int32)gaugeType, (Int32)gaugeType };

                var titleText = new String[3];
                var titleX = 0;
                var titleY = 0;

                var unitText = new String[3];
                var unitX = 20;
                var unitY = 3;

                String valueText;
                var valueTextY = new[] { 6 + 16 * 1, 6 + 16 * 2, 6 + 16 * 3, 6 + 16 * 4 };
                var valueTextY2 = new[] { 6 + 22 * 1, 6 + 22 * 2 };

                var titleFontSize = displayName.Length > 13 ? 9 : 12;
                var fontSize = 15;
                var doubleFontSize = 14;
                var monFontSize = 12;
                var unitFontSize = 10;

                var i = 0;
                switch (gaugeType)
                {
                    case LibreHardwareMonitorGaugeType.GPU_Load:
                    case LibreHardwareMonitorGaugeType.CPU_Load:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText(displayName, titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        bitmapBuilder.DrawText(curLevel[0] > 99.9 || curLevel[0] == 0 ? $"{curLevel[0]:N0}" : $"{curLevel[0]:N1}", titleX - 1, valueTextY[1], this.width, this.height, valueColor, fontSize);
                        bitmapBuilder.DrawText("%", unitX, valueTextY[1] + unitY, this.width, this.height, unitColor, unitFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.GPU_Core:
                    case LibreHardwareMonitorGaugeType.CPU_Core:
                        monType[0] = (Int32)gaugeType;
                        monType[1] = (Int32)gaugeType + 1;

                        maxLimit = gaugeType == LibreHardwareMonitorGaugeType.CPU_Core ? 95 : 83;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[monType[0]] = maxLevel[0];

                        curLevel[1] = this._lastLevel[monType[1]];
                        maxLevel[1] = this._lastMaxLevel[monType[1]];
                        maxLevel[1] = maxLevel[1] < maxLimit ? maxLimit : maxLevel[1];
                        this._lastMaxLevel[monType[1]] = maxLevel[1];

                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 2);
                        bitmapBuilder.DrawText(displayName, titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N0}", titleX - 15, valueTextY[1], this.width, this.height, valueColor, doubleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[1]:N0}", titleX + 15, valueTextY[1], this.width, this.height, valueColor, doubleFontSize);
                        bitmapBuilder.DrawText("℃", unitX - 25, valueTextY[1] + unitY - 14, this.width, this.height, unitColor, unitFontSize);
                        bitmapBuilder.DrawText("℃", unitX + 3,  valueTextY[1] + unitY - 14, this.width, this.height, unitColor, unitFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.GPU_Hotspot:
                    case LibreHardwareMonitorGaugeType.CPU_Package:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText(displayName, titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, valueTextY[1], this.width, this.height, valueColor, fontSize);
                        bitmapBuilder.DrawText("℃", unitX, valueTextY[1] + unitY, this.width, this.height, unitColor, unitFontSize);
                        if (gaugeType == LibreHardwareMonitorGaugeType.GPU_Hotspot)
                        {
                            maxLimit = 83;
                            maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                            this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                            maxLevel[1] = maxLevel[0];
                            maxLevel[2] = maxLevel[0];
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.CPU_Power:
                    case LibreHardwareMonitorGaugeType.GPU_Power:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText(displayName, titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, valueTextY[1], this.width, this.height, valueColor, fontSize);
                        bitmapBuilder.DrawText("W", unitX, valueTextY[1] + unitY, this.width, this.height, unitColor, unitFontSize);

                        maxLimit = gaugeType == LibreHardwareMonitorGaugeType.CPU_Power ? 120 : 320;
                        maxLevel[0] = maxLevel[0] < maxLimit ? maxLimit : maxLevel[0];
                        this._lastMaxLevel[(Int32)gaugeType] = maxLevel[0];
                        maxLevel[1] = maxLevel[0];
                        maxLevel[2] = maxLevel[0];
                        break;

                    case LibreHardwareMonitorGaugeType.Virtual_Memory:
                    case LibreHardwareMonitorGaugeType.Memory:
                    case LibreHardwareMonitorGaugeType.GPU_Memory:
                        monType[0] = (Int32)gaugeType - 4;
                        monType[1] = (Int32)gaugeType;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[0]];
                            curLevel[i] = this._lastLevel[monType[0]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastLevel[monType[i]];
                        }
                        bitmapBuilder.DrawText(displayName, titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        bitmapBuilder.DrawText(gaugeType == LibreHardwareMonitorGaugeType.GPU_Memory ? $"{curLevel[1]/1024:N1}" : $"{curLevel[1]:N1}", titleX, valueTextY2[0], this.width, this.height, valueColor, fontSize);
                        bitmapBuilder.DrawText("G", unitX, valueTextY2[0] + unitY, this.width, this.height, unitColor, unitFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX, valueTextY2[1], this.width, this.height, valueColor, fontSize);
                        bitmapBuilder.DrawText("%", unitX, valueTextY2[1] + unitY, this.width, this.height, unitColor, unitFontSize);
                        break;

                    case LibreHardwareMonitorGaugeType.Storage_T_1:
                    case LibreHardwareMonitorGaugeType.Storage_T_2:
                    case LibreHardwareMonitorGaugeType.Storage_T_3:
                    case LibreHardwareMonitorGaugeType.Storage_T_4:
                    case LibreHardwareMonitorGaugeType.Storage_T_5:
                    case LibreHardwareMonitorGaugeType.Storage_T_6:
                    case LibreHardwareMonitorGaugeType.Storage_U_1:
                    case LibreHardwareMonitorGaugeType.Storage_U_2:
                    case LibreHardwareMonitorGaugeType.Storage_U_3:
                    case LibreHardwareMonitorGaugeType.Storage_U_4:
                    case LibreHardwareMonitorGaugeType.Storage_U_5:
                    case LibreHardwareMonitorGaugeType.Storage_U_6:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText(displayName, titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        bitmapBuilder.DrawText(gaugeType >= LibreHardwareMonitorGaugeType.Storage_U_1 ? $"{curLevel[0]:N1}" : $"{curLevel[0]:N0}", titleX - 1, valueTextY[1], this.width, this.height, valueColor, fontSize);
                        bitmapBuilder.DrawText(gaugeType >= LibreHardwareMonitorGaugeType.Storage_U_1 ? "%" : "℃", unitX, valueTextY[1] + unitY, this.width, this.height, unitColor, unitFontSize);
                        break;

                    // Monitors
                    case LibreHardwareMonitorGaugeType.Mon_CPU:
                    case LibreHardwareMonitorGaugeType.Mon_GPU:
                        monType[0] = (Int32)gaugeType + 1;
                        monType[1] = (Int32)gaugeType + 2;
                        monType[2] = (Int32)gaugeType + 4;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastLevel[monType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText(gaugeType == LibreHardwareMonitorGaugeType.Mon_CPU ? "CPU Monitor" : "GPU Monitor", titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        titleText[0] = "L";
                        titleText[1] = "C";
                        titleText[2] = "P";
                        unitText[0] = "%";
                        unitText[1] = "℃";
                        unitText[2] = "W";
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastMonLevel[monType[i]];
                            valueText = i == 0 && curLevel[i] > 99.9 ? $"{curLevel[i]:N0}" : $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(titleText[i], titleX - 21, valueTextY[i], this.width, this.height, monTitleColor, monFontSize);
                            bitmapBuilder.DrawText(valueText, titleX + 3, valueTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                            bitmapBuilder.DrawText(unitText[i], titleX + 23, valueTextY[i] + unitY, this.width, this.height, unitColor, unitFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Mon_Memory_Load:
                        monType[0] = (Int32)LibreHardwareMonitorGaugeType.Memory_Load;
                        monType[1] = (Int32)LibreHardwareMonitorGaugeType.Virtual_Memory_Load;
                        monType[2] = (Int32)LibreHardwareMonitorGaugeType.GPU_Memory_Load;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastLevel[monType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("Memory Load", titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        titleText[0] = "S";
                        titleText[1] = "V";
                        titleText[2] = "G";
                        unitText[0] = "%";
                        unitText[1] = "%";
                        unitText[2] = "%";
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastMonLevel[monType[i]];
                            valueText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(titleText[i], titleX - 21, valueTextY[i], this.width, this.height, monTitleColor, monFontSize);
                            bitmapBuilder.DrawText(valueText, titleX + 6, valueTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                            bitmapBuilder.DrawText(unitText[i], titleX + 23, valueTextY[i] + unitY, this.width, this.height, unitColor, unitFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Mon_Memory:
                        monType[0] = (Int32)LibreHardwareMonitorGaugeType.Memory_Load;
                        monType[1] = (Int32)LibreHardwareMonitorGaugeType.Virtual_Memory_Load;
                        monType[2] = (Int32)LibreHardwareMonitorGaugeType.GPU_Memory_Load;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastLevel[monType[i]];
                        }
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        monType[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        monType[1] = (Int32)LibreHardwareMonitorGaugeType.Virtual_Memory;
                        monType[2] = (Int32)LibreHardwareMonitorGaugeType.GPU_Memory;
                        bitmapBuilder.DrawText("Memory", titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        titleText[0] = "S";
                        titleText[1] = "V";
                        titleText[2] = "G";
                        unitText[0] = "G";
                        unitText[1] = "G";
                        unitText[2] = "G";
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = i == 2 ? this._lastMonLevel[monType[i]] / 1024 : this._lastMonLevel[monType[i]];
                            valueText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(titleText[i], titleX - 21, valueTextY[i], this.width, this.height, monTitleColor, monFontSize);
                            bitmapBuilder.DrawText(valueText, titleX + 6, valueTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                            bitmapBuilder.DrawText(unitText[i], titleX + 23, valueTextY[i] + unitY, this.width, this.height, unitColor, unitFontSize);
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Mon_Storage_T_G1:
                    case LibreHardwareMonitorGaugeType.Mon_Storage_T_G2:
                    case LibreHardwareMonitorGaugeType.Mon_Storage_U_G1:
                    case LibreHardwareMonitorGaugeType.Mon_Storage_U_G2:
                        var ut = gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_T_G1 || gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_T_G2 ? "℃" : "%";
                        var tt = gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_T_G1 || gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_U_G1 ? "Storage G1" : "Storage G2";
                        for (i = 0; i < 3; i++)
                        {
                            monType[i] = (Int32)gaugeType + i + 1;
                            titleText[i] = (gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_T_G1 || gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_T_G2 ? $"T" : $"U")
                                         + (gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_T_G1 || gaugeType == LibreHardwareMonitorGaugeType.Mon_Storage_U_G1 ? $"{i + 1}" : $"{i + 4}");
                            unitText[i] = ut;

                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastMonLevel[monType[i]];
                            if (curLevel[i] > 0)
                            {
                                if (i == 0)
                                {
                                    this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                                    bitmapBuilder.DrawText(tt, titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                                }
                                valueText = $"{curLevel[i]:N0}";
                                bitmapBuilder.DrawText(titleText[i], titleX - 18, valueTextY[i], this.width, this.height, monTitleColor, monFontSize);
                                bitmapBuilder.DrawText(valueText, titleX + 6, valueTextY[i], this.width, this.height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                                bitmapBuilder.DrawText(unitText[i], titleX + 23, valueTextY[i] + unitY, this.width, this.height, unitColor, unitFontSize);
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Battery:
                        this.DrawGuage(bitmapBuilder, curLevel, maxLevel, accentColor, 1);
                        bitmapBuilder.DrawText("Battery", titleX, titleY, this.width, this.height, titleColor, titleFontSize);
                        bitmapBuilder.DrawText($"{curLevel[0]:N1}", titleX - 1, valueTextY[1], this.width, this.height, valueColor, fontSize);
                        bitmapBuilder.DrawText("%", unitX, valueTextY[1] + unitY, this.width, this.height, unitColor, unitFontSize);
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
                    this.ActionImageChanged(monitorType.ToString());
                }
            }
        }

        private void OnSensorListChanged(Object sender, EventArgs e)
        {
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPU_Load);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPU_Core);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPU_Package);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPU_Power);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPU_Load);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPU_Core);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPU_Hotspot);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPU_Power);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Memory_Load);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Virtual_Memory_Load);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPU_Memory_Load);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Memory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Virtual_Memory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPU_Memory);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_T_1);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_T_2);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_T_3);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_T_4);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_T_5);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_U_1);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_U_2);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_U_3);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_U_4);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Storage_U_5);

            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.Battery);

            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_CPU);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_GPU);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_Memory_Load);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_Memory);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_Storage_T_G1);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_Storage_T_G2);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_Storage_U_G1);
            this.UpdateMonitorIndex(LibreHardwareMonitorGaugeType.Mon_Storage_U_G2);

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
                return false;
            }

            var ret = false;
            foreach (var sensor in sensorList)
            {
                if (!this._lastMonLevel[(Int32)sensor.GaugeType].Equals(sensor.Value))
                {
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
