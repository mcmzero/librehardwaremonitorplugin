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

            AddParameter(LibreHardwareMonitorGaugeType.CPULoad);
            AddParameter(LibreHardwareMonitorGaugeType.CPUCore);
            AddParameter(LibreHardwareMonitorGaugeType.CPUPackage);
            AddParameter(LibreHardwareMonitorGaugeType.CPUPower);
            AddParameter(LibreHardwareMonitorGaugeType.GPULoad);
            AddParameter(LibreHardwareMonitorGaugeType.GPUCore);
            AddParameter(LibreHardwareMonitorGaugeType.GPUHotspot);
            AddParameter(LibreHardwareMonitorGaugeType.GPUPower);
            AddParameter(LibreHardwareMonitorGaugeType.Memory);
            AddParameter(LibreHardwareMonitorGaugeType.RAM);
            AddParameter(LibreHardwareMonitorGaugeType.VrMemory);
            AddParameter(LibreHardwareMonitorGaugeType.VrRAM);
            AddParameter(LibreHardwareMonitorGaugeType.GPUMemory);
            AddParameter(LibreHardwareMonitorGaugeType.VRAM);
            AddParameter(LibreHardwareMonitorGaugeType.CPUMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.GPUMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.MEMMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.RAMMonitor);
            AddParameter(LibreHardwareMonitorGaugeType.Battery);

            void AddParameter(LibreHardwareMonitorGaugeType gaugeType) => this.AddParameter(gaugeType.ToString(), gaugeType.ToString(), this.GroupName);
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

        private Int32 GetImageIndex(LibreHardwareMonitorGaugeType guageType) => Helpers.MinMax(((Int32)this._lastLevels[(Int32)guageType] + 9) / 20, 0, 5);

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
                var y0 = n + vn * 1;
                var y1 = n + vn * 2;
                var y2 = n + vn * 3;
                var y3 = n + vn * 4;

                var width = 80;
                var height = 16;

                var titleFontSize = 12;
                var fontSize = 12;
                var monFontSize= 12;
                var fontColor = BitmapColor.White;

                var level = this._lastLevels[(Int32)gaugeType];
                var minLevel = this._lastMinLevels[(Int32)gaugeType];
                var maxLevel = this._lastMaxLevels[(Int32)gaugeType];

                var rectangleColor = new BitmapColor(35, 35, 35);
                bitmapBuilder.Clear(rectangleColor);

                //var frTop = new Int32[4] { 0, 0, 80, 18 };
                //var frBottom = new Int32[4] { 0, 72, 80, 8 };
                //bitmapBuilder.FillRectangle(frTop[0], frTop[1], frTop[2], frTop[3], rectangleColor);
                //bitmapBuilder.FillRectangle(frBottom[0], frBottom[1], frBottom[2], frBottom[3], rectangleColor);

                var frMiddle = new Int32[4] { 6, 18, 66, 54 };
                var frOutline = new Int32[4] { 0, 0, 78, 78 };
                var grayColor = new BitmapColor(120, 120, 120);
                var darkGrayColor = new BitmapColor(60, 60, 60);
                var accentColor = sensor.Color;
                bitmapBuilder.FillRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], BitmapColor.Black);

                var imageName = "Memory";
                var imageIndex = this.GetImageIndex(gaugeType);
                switch (gaugeType)
                {
                    case LibreHardwareMonitorGaugeType.CPULoad:
                        bitmapBuilder.DrawText("CPU(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}%", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUCore:
                        bitmapBuilder.DrawText("Core(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPackage:
                        bitmapBuilder.DrawText("Pkg(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}W", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.Memory:
                        bitmapBuilder.DrawText("Mem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}%", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.RAM:
                        bitmapBuilder.DrawText("RAM(GB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}GB", x, y2, width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.Memory);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        minLevel = this._lastMinLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        break;
                    case LibreHardwareMonitorGaugeType.VrMemory:
                        bitmapBuilder.DrawText("VrMem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}%", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.VrRAM:
                        bitmapBuilder.DrawText("VrRAM(GB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}GB", x, y2, width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.VrMemory);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        minLevel = this._lastMinLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        break;

                    case LibreHardwareMonitorGaugeType.GPULoad:
                        bitmapBuilder.DrawText("GPU(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}%", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUCore:
                        bitmapBuilder.DrawText("Core(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUHotspot:
                        bitmapBuilder.DrawText("HSpot(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}W", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUMemory:
                        bitmapBuilder.DrawText("GPUMem(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}%", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.VRAM:
                        bitmapBuilder.DrawText("VRAM(MB)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N0}MB", x, y2, width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.GPUMemory);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        minLevel = this._lastMinLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        break;

                    case LibreHardwareMonitorGaugeType.CPUMonitor:
                        bitmapBuilder.DrawText("CPU", x, y, width, height, sensor.Color, titleFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.CPUPower];
                        bitmapBuilder.DrawText($"{level:N1}W", x, y2, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.CPUCore];
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y1, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.CPULoad];
                        bitmapBuilder.DrawText($"{level:N1}%", x, y0, width, height, fontColor, monFontSize);
                        minLevel = this._lastMinLevels[(Int32)LibreHardwareMonitorGaugeType.CPULoad];
                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.CPULoad];
                        break;

                    case LibreHardwareMonitorGaugeType.GPUMonitor:
                        bitmapBuilder.DrawText("GPU", x, y, width, height, sensor.Color, titleFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUPower];
                        bitmapBuilder.DrawText($"{level:N1}W", x, y2, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUHotspot];
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y1, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPULoad];
                        bitmapBuilder.DrawText($"{level:N1}%", x, y0, width, height, fontColor, monFontSize);
                        minLevel = this._lastMinLevels[(Int32)LibreHardwareMonitorGaugeType.GPULoad];
                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPULoad];
                        break;

                    case LibreHardwareMonitorGaugeType.MEMMonitor:
                        bitmapBuilder.DrawText("MEM(%)", x, y, width, height, sensor.Color, titleFontSize);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        bitmapBuilder.DrawText($"SM:{level:N1}%", x, y0, width, height, fontColor, monFontSize);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        bitmapBuilder.DrawText($"VM:{level:N1}%", x, y1, width, height, fontColor, monFontSize);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        bitmapBuilder.DrawText($"GM:{level:N1}%", x, y2, width, height, fontColor, monFontSize);

                        var levels = new Single[3];
                        var guages = new Int32[3];
                        levels[0] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        guages[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        levels[1] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        guages[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        levels[2] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        guages[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        var maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
                                minLevel = this._lastMinLevels[guages[i]];
                                maxLevel = this._lastMaxLevels[guages[i]];
                                break;
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.RAMMonitor:
                        bitmapBuilder.DrawText("RAM(GB)", x, y, width, height, sensor.Color, titleFontSize);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.RAM];
                        bitmapBuilder.DrawText($"SM:{level:N1}", x, y0, width, height, fontColor, monFontSize);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrRAM];
                        bitmapBuilder.DrawText($"VM:{level:N1}", x, y1, width, height, fontColor, monFontSize);
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VRAM];
                        bitmapBuilder.DrawText($"GM:{level/1024:N2}", x, y2, width, height, fontColor, monFontSize);

                        levels = new Single[3];
                        guages = new Int32[3];
                        levels[0] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        guages[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        levels[1] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        guages[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        levels[2] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        guages[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;
                        maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
                                minLevel = this._lastMinLevels[guages[i]];
                                maxLevel = this._lastMaxLevels[guages[i]];
                                break;
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.Battery:
                        bitmapBuilder.DrawText("Battery(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}%", x, y2, width, height, fontColor, fontSize);
                        break;
                }
                if (gaugeType != LibreHardwareMonitorGaugeType.CPUMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.GPUMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.MEMMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.RAMMonitor)
                {
                    var imageBytes = PluginResources.ReadBinaryFile($"{imageName}{imageIndex}.png");
                    bitmapBuilder.DrawImage(imageBytes, x, y0 - 9);
                }

                var rateLevel = (level - minLevel) / (maxLevel - minLevel);
                if (rateLevel > 0.9)
                {
                    bitmapBuilder.DrawRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], accentColor);
                    bitmapBuilder.DrawRectangle(frOutline[0], frOutline[1], frOutline[2], frOutline[3], accentColor);
                }
                else if (rateLevel > 0.7)
                {
                    bitmapBuilder.DrawRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], accentColor);
                    bitmapBuilder.DrawRectangle(frOutline[0], frOutline[1], frOutline[2], frOutline[3], grayColor);
                }
                else if (rateLevel > 0.5)
                {
                    bitmapBuilder.DrawRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], grayColor);
                    bitmapBuilder.DrawRectangle(frOutline[0], frOutline[1], frOutline[2], frOutline[3], grayColor);
                }
                else if (rateLevel > 0.3)
                {
                    bitmapBuilder.DrawRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], grayColor);
                    bitmapBuilder.DrawRectangle(frOutline[0], frOutline[1], frOutline[2], frOutline[3], darkGrayColor);
                }
                else
                {
                    bitmapBuilder.DrawRectangle(frMiddle[0], frMiddle[1], frMiddle[2], frMiddle[3], darkGrayColor);
                    bitmapBuilder.DrawRectangle(frOutline[0], frOutline[1], frOutline[2], frOutline[3], darkGrayColor);
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
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.RAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VrMemory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VrRAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUMemory);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.VRAM);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.CPUMonitor);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.GPUMonitor);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.RAMMonitor);
            this.UpdateGaugeIndex(LibreHardwareMonitorGaugeType.MEMMonitor);
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
