namespace NotADoctor99.LibreHardwareMonitorPlugin
{
    using System;
    using System.Linq;

    using Loupedeck;

    public class ShowGaugeCommand : PluginDynamicCommand
    {
        public ShowGaugeCommand()
        {
            this.IsWidget = true;
            this.GroupName = "Gauges";
            var GroupMonitor = "Monitors";
            var GroupStorage = "Storages";

            AddParameter(LHMGaugeType.CPU_Load, this.GroupName);
            AddParameter(LHMGaugeType.CPU_Core, this.GroupName);
            AddParameter(LHMGaugeType.CPU_Package, this.GroupName);
            AddParameter(LHMGaugeType.CPU_Power, this.GroupName);

            AddParameter(LHMGaugeType.GPU_Load, this.GroupName);
            AddParameter(LHMGaugeType.GPU_Core, this.GroupName);
            AddParameter(LHMGaugeType.GPU_Hotspot, this.GroupName);
            AddParameter(LHMGaugeType.GPU_Power, this.GroupName);

            AddParameter(LHMGaugeType.Memory, this.GroupName);
            AddParameter(LHMGaugeType.Virtual_Memory, this.GroupName);
            AddParameter(LHMGaugeType.GPU_Memory, this.GroupName);

            AddParameter(LHMGaugeType.Storage_T_1, GroupStorage);
            AddParameter(LHMGaugeType.Storage_T_2, GroupStorage);
            AddParameter(LHMGaugeType.Storage_T_3, GroupStorage);
            AddParameter(LHMGaugeType.Storage_T_4, GroupStorage);
            AddParameter(LHMGaugeType.Storage_T_5, GroupStorage);
            AddParameter(LHMGaugeType.Storage_T_6, GroupStorage);

            AddParameter(LHMGaugeType.Storage_U_1, GroupStorage);
            AddParameter(LHMGaugeType.Storage_U_2, GroupStorage);
            AddParameter(LHMGaugeType.Storage_U_3, GroupStorage);
            AddParameter(LHMGaugeType.Storage_U_4, GroupStorage);
            AddParameter(LHMGaugeType.Storage_U_5, GroupStorage);
            AddParameter(LHMGaugeType.Storage_U_6, GroupStorage);

            AddParameter(LHMGaugeType.Monitor_CPU, GroupMonitor);
            AddParameter(LHMGaugeType.Monitor_GPU, GroupMonitor);
            AddParameter(LHMGaugeType.Monitor_Memory_Load, GroupMonitor);
            AddParameter(LHMGaugeType.Monitor_Memory, GroupMonitor);
            AddParameter(LHMGaugeType.Monitor_Storage_T_G1, GroupMonitor);
            AddParameter(LHMGaugeType.Monitor_Storage_T_G2, GroupMonitor);
            AddParameter(LHMGaugeType.Monitor_Storage_U_G1, GroupMonitor);
            AddParameter(LHMGaugeType.Monitor_Storage_U_G2, GroupMonitor);

            AddParameter(LHMGaugeType.Battery, this.GroupName);

            void AddParameter(LHMGaugeType gaugeType, String GroupName) => this.AddParameter(gaugeType.ToString(), gaugeType.ToString().Replace('_', ' '), GroupName);

            this.InitGuage();
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

        private readonly Single[] _lastMonLevel = new Single[(Int32)LHMGaugeType.Count];
        private readonly Single[] _lastLevel = new Single[(Int32)LHMGaugeType.Count];
        private readonly Single[] _lastMinLevel = new Single[(Int32)LHMGaugeType.Count];
        private readonly Single[] _lastMaxLevel = new Single[(Int32)LHMGaugeType.Count];

        private BitmapColor GetColorByLevel(Single level, Single maxLevel, Int32 baseValue, BitmapColor accentColor) => new BitmapColor(level / maxLevel > 0.9 ? new BitmapColor(255, 30, 30) : accentColor, Helpers.MinMax((Int32)(baseValue + (255 - baseValue) * level / maxLevel), 0, 255));
        private BitmapColor GetColorByLevel(Single level, Single maxLevel, BitmapColor accentColor) => this.GetColorByLevel(level, maxLevel, 200, accentColor);

        private Int32[] frOutLine;
        private Int32[] frInLine;
        private Int32[] LeftLine;
        private Int32[] RightLine;

        private void InitGuage()
        {
            // Rectangle: x, y , width, height
            this.frOutLine = new Int32[4] { 0, 0, 78, 78 }; // 80x80
            this.frInLine = new Int32[4] { this.frOutLine[0] + 8, this.frOutLine[1] + 8 * 2 + 3, this.frOutLine[2] - 8 * 2, this.frOutLine[3] - 8 * 3 };

            // Line: x1, y1, x2, y2
            var offset = 0;
            this.LeftLine = new Int32[4] { this.frInLine[0] - offset, this.frInLine[1], this.frInLine[0] - offset, this.frInLine[1] + this.frInLine[3] };
            this.RightLine = new Int32[4] { this.frInLine[0] + this.frInLine[2] + offset, this.frInLine[1], this.frInLine[0] + this.frInLine[2] + offset, this.frInLine[1] + this.frInLine[3] };
        }

        private void DrawGuage(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel, Int32 barCount)
        {
            bitmapBuilder.Clear(BitmapColor.Black);

            //var colorLevel = new Single[] { 0.2f, 0.6f, 0.8f }; // g b r
            var level = curLevel[0] / maxLevel[0];
            var color1 = this.GetColorByLevel5(level, 255, 120);
            var color2 = this.GetColorByLevel5(level, 25, 120);

            var gray = new BitmapColor(BitmapColor.White, 220);
            bitmapBuilder.FillRectangle(this.frOutLine[0], this.frOutLine[1] + 2, this.frOutLine[2], this.frOutLine[1] + 12, color1);
            bitmapBuilder.FillRectangle(this.frOutLine[0], this.frOutLine[1] + 16, this.frOutLine[2], this.frOutLine[3], color2);
            bitmapBuilder.DrawRectangle(this.frOutLine[0], this.frOutLine[1], this.frOutLine[2], this.frOutLine[3], gray);

            bitmapBuilder.FillRectangle(this.frOutLine[0], this.frOutLine[1], this.frOutLine[2], 1, gray);
            bitmapBuilder.FillRectangle(this.frOutLine[0], this.frOutLine[1] + 15, this.frOutLine[2], 1, gray);

            var leftLineColor = this.GetColorByLevel5(level, 150, 80);
            var rightLineColor = this.GetColorByLevel5(level, 150, 80);
            this.DrawOutline(bitmapBuilder, curLevel, maxLevel, leftLineColor, rightLineColor);

            if (barCount == 2)
            {
                this.DrawBar2(bitmapBuilder, curLevel, maxLevel);
            }
            else
            {
                this.DrawBar(bitmapBuilder, curLevel, maxLevel);
            }
        }

        private void DrawBar(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel)
        {
            bitmapBuilder.FillRectangle(this.frInLine[0], this.frInLine[1], this.frInLine[2], this.frInLine[3], BitmapColor.Black);
            var gray = new BitmapColor(BitmapColor.White, 220);
            var level = curLevel[0] / maxLevel[0];
            if (level > 0.001)
            {
                //var colorLevel = new Single[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f }; // g b r
                var barColor = this.GetColorByLevel3(level, 200, 30);

                var x = this.frInLine[0] + 2;
                var y = this.frInLine[1] + 2;
                var w = this.frInLine[2] - 3;
                var h = this.frInLine[3] - 4;

                this.GetRectangleYHByLevel(level, y, h, out var bottomY, out var bottomH);
                bitmapBuilder.FillRectangle(x, bottomY, w, bottomH, barColor);
                bitmapBuilder.FillRectangle(x, bottomY, w, 1, gray);
                bitmapBuilder.FillRectangle(x, bottomY + bottomH - 1, w, 1, gray);
            }
            bitmapBuilder.DrawRectangle(this.frInLine[0], this.frInLine[1], this.frInLine[2], this.frInLine[3], gray);
        }

        private void DrawBar2(BitmapBuilder bitmapBuilder, Single[] curLevel, Single[] maxLevel)
        {
            var x = this.frInLine[0] + 2;
            var y = this.frInLine[1] + 2;
            var w = this.frInLine[2] / 2 - 3 - 1;
            var h = this.frInLine[3] - 4;
            var lx = this.frInLine[0];
            var ly = this.frInLine[1];
            var lw = this.frInLine[2] / 2 - 1;
            var lh = this.frInLine[3];

            bitmapBuilder.FillRectangle(this.frInLine[0], this.frInLine[1], this.frInLine[2], this.frInLine[3], BitmapColor.Black);
            var gray = new BitmapColor(BitmapColor.White, 220);

            var leftLevel = curLevel[0] / maxLevel[0];
            if (leftLevel > 0.001)
            {
                var leftBarColor = this.GetColorByLevel3(leftLevel, 200, 30);
                this.GetRectangleYHByLevel(leftLevel, y, h, out var bottomLY, out var bottomLH);
                bitmapBuilder.FillRectangle(x, bottomLY, w, bottomLH, leftBarColor);
                bitmapBuilder.FillRectangle(x, bottomLY, w, 1, gray);
                bitmapBuilder.FillRectangle(x, bottomLY + bottomLH - 1, w, 1, gray);
            }
            bitmapBuilder.DrawRectangle(lx, ly, lw, lh, gray);

            var rightLevel = curLevel[1] / maxLevel[1];
            if (rightLevel > 0.001)
            {
                var rightBarColor = this.GetColorByLevel3(rightLevel, 200, 30);
                this.GetRectangleYHByLevel(rightLevel, y, h, out var bottomRY, out var bottomRH);
                bitmapBuilder.FillRectangle(x + w + 5, bottomRY, w, bottomRH, rightBarColor);
                bitmapBuilder.FillRectangle(x + w + 5, bottomRY, w, 1, gray);
                bitmapBuilder.FillRectangle(x + w + 5, bottomRY + bottomRH - 1, w, 1, gray);
            }
            bitmapBuilder.DrawRectangle(lx + w + 5, ly, lw, lh, gray);
        }

        private BitmapColor GetColorByLevel3(Single level, Int32 alpha, Int32 baseRGB)
        {
            var colorLevel = new Single[] { 0f, 0.2f, 0.4f, 0.6f }; // g b r

            Int32 r = baseRGB, g = baseRGB, b = baseRGB, maxColor = 255 - baseRGB;
            if (level <= colorLevel[1])
            {
                // green
                var n = (level - colorLevel[0]) / (colorLevel[1] - colorLevel[0]);
                g += (Int32)(maxColor * n);
            }
            else if (level <= colorLevel[2])
            {
                // blue
                var n = (level - colorLevel[1]) / (colorLevel[2] - colorLevel[1]);
                g += (Int32)(maxColor * (1 - n));
                b += (Int32)(maxColor * n);
            }
            else if (level <= colorLevel[3])
            {
                // red
                var n = (level - colorLevel[2]) / (colorLevel[3] - colorLevel[2]);
                b += (Int32)(maxColor * (1 - n));
                r += (Int32)(maxColor * n);
            }
            else
            {
                r += maxColor;
            }
            return new BitmapColor(Helpers.MinMax(r, 0, 255), Helpers.MinMax(g, 0, 255), Helpers.MinMax(b, 0, 255), alpha);
        }


        private BitmapColor GetColorByLevel5(Single level, Int32 alpha, Int32 baseRGB)
        {
            var colorLevel = new Single[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f }; // g b r

            Int32 r = baseRGB, g = baseRGB, b = baseRGB, maxColor = 255 - baseRGB;
            if (level < colorLevel[1])
            {
                // blue
                var n = (level - colorLevel[0]) / (colorLevel[1] - colorLevel[0]);
                g += (Int32)(maxColor * (1 - n));
                r += (Int32)(maxColor * (1 - n));
                b += (Int32)(baseRGB * n);
            }
            else if (level < colorLevel[2])
            {
                // green
                var n = (level - colorLevel[1]) / (colorLevel[2] - colorLevel[1]);
                r += (Int32)(maxColor * (1 - n));
                b += (Int32)(maxColor * (1 - n));
                g += (Int32)(maxColor * n);
            }
            else if (level < colorLevel[3])
            {
                // yellow(red + green)
                var n = (level - colorLevel[2]) / (colorLevel[3] - colorLevel[2]);
                b += (Int32)(maxColor * (1 - n));
                r += (Int32)(maxColor * n);
                g += (Int32)(maxColor * n);
            }
            else if (level < colorLevel[4])
            {
                // pink(red + blue)
                var n = (level - colorLevel[3]) / (colorLevel[4] - colorLevel[3]);
                g += (Int32)(maxColor * (1 - n));
                r += (Int32)(maxColor * n);
                b += (Int32)(maxColor * n);
            }
            else if (level < colorLevel[5])
            {
                // red
                var n = (level - colorLevel[4]) / (colorLevel[5] - colorLevel[4]);
                g += (Int32)(maxColor * (1 - n));
                b += (Int32)(maxColor * (1 - n));
                r += (Int32)(maxColor * n);
            }
            else
            {
                r += maxColor;
            }
            return new BitmapColor(Helpers.MinMax(r, 0, 255), Helpers.MinMax(g, 0, 255), Helpers.MinMax((Int32)(b * 0.6), 0, 255), alpha);
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
            var gray = new BitmapColor(BitmapColor.White, 220);

            // Left Line: x1, y1, x2, y2
            var lx1 = this.LeftLine[0] - 3;
            var ly1 = this.LeftLine[1];
            var lx2 = this.LeftLine[2] - 3;
            var ly2 = this.LeftLine[3];
            this.GetLineY1Y2ByLevel(curLevel[0] / maxLevel[0], ly1, ly2, out var middleLY1, out var middleLY2);
            bitmapBuilder.DrawLine(lx1, middleLY1, lx2, middleLY2, leftColor, 4);

            bitmapBuilder.DrawLine(lx1 - 2, middleLY1, lx2 + 2, middleLY1, gray, 1);
            bitmapBuilder.DrawLine(lx1 - 2, middleLY2, lx2 + 2, middleLY2, gray, 1);

            // Right Line: x1, y1, x2, y2
            var rx1 = this.RightLine[0] + 4;
            var ry1 = this.RightLine[1];
            var rx2 = this.RightLine[2] + 4;
            var ry2 = this.RightLine[3];
            this.GetLineY1Y2ByLevel(curLevel[0] / maxLevel[0], ry1, ry2, out var middleRY1, out var middleRY2);
            bitmapBuilder.DrawLine(rx1, middleRY1, rx2, middleRY2, rightColor, 4);

            bitmapBuilder.DrawLine(rx1 - 2, middleRY1, rx1 + 2, middleRY1, gray, 1);
            bitmapBuilder.DrawLine(rx1 - 2, middleRY2, rx1 + 2, middleRY2, gray, 1);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (!Enum.TryParse<LHMGaugeType>(actionParameter, out var gaugeType))
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
                var accentColor = (gaugeType == LHMGaugeType.Monitor_Memory
                                   || gaugeType == LHMGaugeType.Monitor_Memory_Load) ? new BitmapColor(255, 10, 90) : sensor.Color;
                var titleColor = BitmapColor.Black;
                var valueColor = BitmapColor.White;
                var unitColor = new BitmapColor(accentColor, 180);
                var monTitleColor = accentColor;
                var monValueColor = BitmapColor.White;

                var maxLevel = new[] { this._lastMaxLevel[(Int32)gaugeType], this._lastMaxLevel[(Int32)gaugeType], this._lastMaxLevel[(Int32)gaugeType] };
                var curLevel = new[] { this._lastLevel[(Int32)gaugeType], this._lastLevel[(Int32)gaugeType], this._lastLevel[(Int32)gaugeType] };
                var monType = new[] { (Int32)gaugeType, (Int32)gaugeType, (Int32)gaugeType };

                String[] titleText;
                var titleX = new[] { 0, -18 };
                var titleY = 0;

                String valueText;
                var valueX = new[] { -1, -15, +15, +3 };
                var valueY1 = new[] { 6 + 16 * 1, 6 + 16 * 2, 6 + 16 * 3, 6 + 16 * 4 };
                var valueY2 = new[] { 6 + 22 * 1, 6 + 22 * 2 };

                String[] unitText;
                var unitX = new[] { 20, -8, 24, 21 };
                var unitY1 = new[] { valueY1[0] + 4, valueY1[1] + 2, valueY1[2] + 2 };
                var unitY2 = new[] { valueY2[0] + 2, valueY2[1] + 2 };

                var titleFontSize = displayName.Length > 13 ? 11 : displayName.Length > 9 ? 12 : 13;
                var valueFontSize = 15;
                var valueDualFontSize = 14;
                var monFontSize = 12;
                var unitFontSize = 10;

                var width = 80;
                var height = 11;
                void DrawGuage1(String dname)
                {
                    this.DrawGuage(bitmapBuilder, curLevel, maxLevel, 1);
                    bitmapBuilder.DrawText(dname, titleX[0], titleY, width, height, titleColor, titleFontSize);
                }
                void DrawGuage2(String dname)
                {
                    this.DrawGuage(bitmapBuilder, curLevel, maxLevel, 2);
                    bitmapBuilder.DrawText(dname, titleX[0], titleY, width, height, titleColor, titleFontSize);
                }
                void DrawValueXY(String vt, Int32 x, Int32 y, Int32 fontType) => bitmapBuilder.DrawText(vt, x, y, width, height, valueColor, fontType == 1 ? valueFontSize : valueDualFontSize);
                void DrawValue(String vt) => bitmapBuilder.DrawText(vt, valueX[0], valueY1[1], width, height, valueColor, valueFontSize);

                void DrawUnitXY(String ut, Int32 x, Int32 y)
                {
                    bitmapBuilder.DrawText(ut, x, y, width, height, BitmapColor.White, unitFontSize);
                    bitmapBuilder.DrawText(ut, x, y, width, height, unitColor, unitFontSize);
                }

                void DrawUnit(String ut)
                {
                    bitmapBuilder.DrawText(ut, unitX[0], unitY1[1], width, height, BitmapColor.White, unitFontSize);
                    bitmapBuilder.DrawText(ut, unitX[0], unitY1[1], width, height, unitColor, unitFontSize);
                }

                var i = 0;
                switch (gaugeType)
                {
                    case LHMGaugeType.GPU_Load:
                    case LHMGaugeType.CPU_Load:
                        DrawGuage1(displayName);
                        DrawValue(curLevel[0] > 99.9 ? $"{curLevel[0]:N0}" : $"{curLevel[0]:N1}");
                        DrawUnit("%");
                        break;

                    case LHMGaugeType.GPU_Core:
                    case LHMGaugeType.CPU_Core:
                        for (i = 0; i < 2; i++)
                        {
                            monType[i] = (Int32)gaugeType + i;
                            curLevel[i] = this._lastLevel[monType[i]];
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                        }
                        DrawGuage2(displayName);
                        for (i = 0; i < 2; i++)
                        {
                            DrawValueXY($"{curLevel[i]:N0}", valueX[i + 1], valueY1[1], 2);
                            DrawUnitXY("℃", unitX[i + 1], unitY1[0]);
                        }
                        break;

                    case LHMGaugeType.GPU_Hotspot:
                    case LHMGaugeType.CPU_Package:
                        DrawGuage1(displayName);
                        DrawValue($"{curLevel[0]:N1}");
                        DrawUnit("℃");
                        break;

                    case LHMGaugeType.CPU_Power:
                    case LHMGaugeType.GPU_Power:
                        DrawGuage1(displayName);
                        DrawValue($"{curLevel[0]:N1}");
                        DrawUnit("W");
                        break;

                    case LHMGaugeType.Virtual_Memory:
                    case LHMGaugeType.Memory:
                    case LHMGaugeType.GPU_Memory:
                        monType[0] -= 4;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[0]];
                            curLevel[i] = this._lastLevel[monType[0]];
                        }
                        DrawGuage1(displayName);
                        for (i = 0; i < 2; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastLevel[monType[i]];
                        }
                        DrawValueXY(gaugeType == LHMGaugeType.GPU_Memory ? $"{curLevel[1] / 1024:N1}" : $"{curLevel[1]:N1}", titleX[0], valueY2[0], 1);
                        DrawUnitXY("G", unitX[0], unitY2[0]);
                        DrawValueXY($"{curLevel[0]:N1}", titleX[0], valueY2[1], 1);
                        DrawUnitXY("%", unitX[0], unitY2[1]);
                        break;

                    case LHMGaugeType.Storage_T_1:
                    case LHMGaugeType.Storage_T_2:
                    case LHMGaugeType.Storage_T_3:
                    case LHMGaugeType.Storage_T_4:
                    case LHMGaugeType.Storage_T_5:
                    case LHMGaugeType.Storage_T_6:
                    case LHMGaugeType.Storage_U_1:
                    case LHMGaugeType.Storage_U_2:
                    case LHMGaugeType.Storage_U_3:
                    case LHMGaugeType.Storage_U_4:
                    case LHMGaugeType.Storage_U_5:
                    case LHMGaugeType.Storage_U_6:
                        DrawGuage1(displayName);
                        DrawValue(gaugeType >= LHMGaugeType.Storage_U_1 ? $"{curLevel[0]:N1}" : $"{curLevel[0]:N0}");
                        DrawUnit(gaugeType >= LHMGaugeType.Storage_U_1 ? "%" : "℃");
                        break;

                    // Monitors
                    case LHMGaugeType.Monitor_CPU:
                    case LHMGaugeType.Monitor_GPU:
                        monType[0] += 1;
                        monType[1] += 2;
                        monType[2] += 4;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastMonLevel[monType[i]];
                        }
                        displayName = gaugeType == LHMGaugeType.Monitor_CPU ? "CPU Monitor" : "GPU Monitor";
                        DrawGuage1(displayName);
                        titleText = new[] { "L", "C", "P" };
                        unitText = new[] { "%", "℃", "W" };
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastMonLevel[monType[i]];
                            valueText = i == 0 && curLevel[i] > 99.9 ? $"{curLevel[i]:N0}" : $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(titleText[i], titleX[1], valueY1[i], width, height, monTitleColor, monFontSize);
                            bitmapBuilder.DrawText(valueText, valueX[3], valueY1[i], width, height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                            bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, BitmapColor.White, unitFontSize);
                            bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, unitColor, unitFontSize);
                        }
                        break;

                    case LHMGaugeType.Monitor_Memory_Load:
                        monType[0] = (Int32)LHMGaugeType.Memory_Load;
                        monType[1] = (Int32)LHMGaugeType.Virtual_Memory_Load;
                        monType[2] = (Int32)LHMGaugeType.GPU_Memory_Load;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastMonLevel[monType[i]];
                        }
                        displayName = "Memory Load";
                        DrawGuage1(displayName);
                        titleText = new[] { "S", "V", "G" };
                        unitText = new[] { "%", "%", "%" };
                        for (i = 0; i < 3; i++)
                        {
                            valueText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(titleText[i], titleX[1], valueY1[i], width, height, monTitleColor, monFontSize);
                            bitmapBuilder.DrawText(valueText, valueX[3], valueY1[i], width, height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                            bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, BitmapColor.White, unitFontSize);
                            bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, unitColor, unitFontSize);
                        }
                        break;

                    case LHMGaugeType.Monitor_Memory:
                        monType[0] = (Int32)LHMGaugeType.Memory_Load;
                        monType[1] = (Int32)LHMGaugeType.Virtual_Memory_Load;
                        monType[2] = (Int32)LHMGaugeType.GPU_Memory_Load;
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastLevel[monType[i]];
                        }
                        displayName = "Memory";
                        DrawGuage1(displayName);
                        monType[0] = (Int32)LHMGaugeType.Memory;
                        monType[1] = (Int32)LHMGaugeType.Virtual_Memory;
                        monType[2] = (Int32)LHMGaugeType.GPU_Memory;
                        titleText = new[] { "S", "V", "G" };
                        unitText = new[] { "G", "G", "G" };
                        for (i = 0; i < 3; i++)
                        {
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = i == 2 ? this._lastMonLevel[monType[i]] / 1024 : this._lastMonLevel[monType[i]];
                            valueText = $"{curLevel[i]:N1}";
                            bitmapBuilder.DrawText(titleText[i], titleX[1], valueY1[i], width, height, monTitleColor, monFontSize);
                            bitmapBuilder.DrawText(valueText, valueX[3], valueY1[i], width, height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                            bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, BitmapColor.White, unitFontSize);
                            bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, unitColor, unitFontSize);
                        }
                        break;

                    case LHMGaugeType.Monitor_Storage_T_G1:
                    case LHMGaugeType.Monitor_Storage_T_G2:
                    case LHMGaugeType.Monitor_Storage_U_G1:
                    case LHMGaugeType.Monitor_Storage_U_G2:
                        var uname = gaugeType == LHMGaugeType.Monitor_Storage_T_G1 || gaugeType == LHMGaugeType.Monitor_Storage_T_G2 ? "℃" : "%";
                        var dname = gaugeType == LHMGaugeType.Monitor_Storage_T_G1 || gaugeType == LHMGaugeType.Monitor_Storage_U_G1 ? "Storage G1" : "Storage G2";
                        titleText = new String[3];
                        unitText = new String[3];
                        for (i = 0; i < 3; i++)
                        {
                            monType[i] = (Int32)gaugeType + i + 1;
                            titleText[i] = (gaugeType == LHMGaugeType.Monitor_Storage_T_G1 || gaugeType == LHMGaugeType.Monitor_Storage_T_G2 ? $"T" : $"U")
                                         + (gaugeType == LHMGaugeType.Monitor_Storage_T_G1 || gaugeType == LHMGaugeType.Monitor_Storage_U_G1 ? $"{i + 1}" : $"{i + 4}");
                            unitText[i] = uname;
                            maxLevel[i] = this._lastMaxLevel[monType[i]];
                            curLevel[i] = this._lastMonLevel[monType[i]];
                            if (curLevel[i] > 0)
                            {
                                if (i == 0)
                                {
                                    DrawGuage1(dname);
                                }
                                valueText = $"{curLevel[i]:N0}";
                                bitmapBuilder.DrawText(titleText[i], titleX[1], valueY1[i], width, height, monTitleColor, monFontSize);
                                bitmapBuilder.DrawText(valueText, valueX[3], valueY1[i], width, height, this.GetColorByLevel(curLevel[i], maxLevel[i], monValueColor), monFontSize);
                                bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, BitmapColor.White, unitFontSize);
                                bitmapBuilder.DrawText(unitText[i], unitX[3], unitY1[i], width, height, unitColor, unitFontSize);
                            }
                        }
                        break;

                    case LHMGaugeType.Battery:
                        DrawGuage1(displayName);
                        DrawValue($"{curLevel[0]:N1}");
                        DrawUnit("%");
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
            this.UpdateGaugeIndex(LHMGaugeType.CPU_Load);
            this.UpdateGaugeIndex(LHMGaugeType.CPU_Core);
            this.UpdateGaugeIndex(LHMGaugeType.CPU_Package);
            this.UpdateGaugeIndex(LHMGaugeType.CPU_Power);

            this.UpdateGaugeIndex(LHMGaugeType.GPU_Load);
            this.UpdateGaugeIndex(LHMGaugeType.GPU_Core);
            this.UpdateGaugeIndex(LHMGaugeType.GPU_Hotspot);
            this.UpdateGaugeIndex(LHMGaugeType.GPU_Power);

            this.UpdateGaugeIndex(LHMGaugeType.Memory_Load);
            this.UpdateGaugeIndex(LHMGaugeType.Virtual_Memory_Load);
            this.UpdateGaugeIndex(LHMGaugeType.GPU_Memory_Load);

            this.UpdateGaugeIndex(LHMGaugeType.Memory);
            this.UpdateGaugeIndex(LHMGaugeType.Virtual_Memory);
            this.UpdateGaugeIndex(LHMGaugeType.GPU_Memory);

            this.UpdateGaugeIndex(LHMGaugeType.Storage_T_1);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_T_2);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_T_3);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_T_4);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_T_5);

            this.UpdateGaugeIndex(LHMGaugeType.Storage_U_1);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_U_2);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_U_3);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_U_4);
            this.UpdateGaugeIndex(LHMGaugeType.Storage_U_5);

            this.UpdateGaugeIndex(LHMGaugeType.Battery);

            this.UpdateMonitorIndex(LHMGaugeType.Monitor_CPU);
            this.UpdateMonitorIndex(LHMGaugeType.Monitor_GPU);
            this.UpdateMonitorIndex(LHMGaugeType.Monitor_Memory_Load);
            this.UpdateMonitorIndex(LHMGaugeType.Monitor_Memory);
            this.UpdateMonitorIndex(LHMGaugeType.Monitor_Storage_T_G1);
            this.UpdateMonitorIndex(LHMGaugeType.Monitor_Storage_T_G2);
            this.UpdateMonitorIndex(LHMGaugeType.Monitor_Storage_U_G1);
            this.UpdateMonitorIndex(LHMGaugeType.Monitor_Storage_U_G2);

            this.ActionImageChanged(null);
        }

        private Boolean UpdateGaugeIndex(LHMGaugeType gaugeType)
        {
            if (!LibreHardwareMonitorPlugin.HardwareMonitor.TryGetSensor(gaugeType, out var sensor))
            {
                return false;
            }

            if (!this._lastLevel[(Int32)gaugeType].Equals(sensor.Value))
            {
                this._lastLevel[(Int32)gaugeType] = sensor.Value;
                this._lastMinLevel[(Int32)gaugeType] = sensor.MinValue;
                this._lastMaxLevel[(Int32)gaugeType] = sensor.MaxValue;
                return true;
            }

            return false;
        }
        private Boolean UpdateMonitorIndex(LHMGaugeType monitorType)
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
                    //PluginLog.Info($"{sensor.GaugeType}: " + sensor.Value);
                    this._lastMonLevel[(Int32)sensor.GaugeType] = sensor.Value;
                    this._lastMinLevel[(Int32)sensor.GaugeType] = sensor.MinValue;
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
