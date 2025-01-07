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
                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPackage:
                        bitmapBuilder.DrawText("Pkg(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y2, width, height, fontColor, fontSize);
                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.CPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}W", x, y2, width, height, fontColor, fontSize);
                        maxLevel = this._lastMaxLevels[(Int32)gaugeType];
                        maxLevel = maxLevel < 120 ? 120 : maxLevel;
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevel;
                        imageIndex = this.GetImageIndexMax(gaugeType);
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
                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        break;

                    case LibreHardwareMonitorGaugeType.GPULoad:
                        bitmapBuilder.DrawText("GPU(%)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}%", x, y2, width, height, fontColor, fontSize);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUCore:
                        bitmapBuilder.DrawText("Core(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y2, width, height, fontColor, fontSize);
                        maxLevel = this._lastMaxLevels[(Int32)gaugeType];
                        maxLevel = maxLevel < 83 ? 83 : maxLevel;
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevel;
                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUHotspot:
                        bitmapBuilder.DrawText("HSpot(℃)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}℃", x, y2, width, height, fontColor, fontSize);
                        maxLevel = this._lastMaxLevels[(Int32)gaugeType];
                        maxLevel = maxLevel < 83 ? 83 : maxLevel;
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevel;
                        imageIndex = this.GetImageIndexMax(gaugeType);
                        break;
                    case LibreHardwareMonitorGaugeType.GPUPower:
                        bitmapBuilder.DrawText("Power(W)", x, y, width, height, sensor.Color, titleFontSize);
                        bitmapBuilder.DrawText($"{level:N1}W", x, y2, width, height, fontColor, fontSize);
                        maxLevel = this._lastMaxLevels[(Int32)gaugeType];
                        maxLevel = maxLevel < 320 ? 320 : maxLevel;
                        this._lastMaxLevels[(Int32)gaugeType] = maxLevel;
                        imageIndex = this.GetImageIndexMax(gaugeType);
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
                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        break;

                    case LibreHardwareMonitorGaugeType.CPUMonitor:
                        bitmapBuilder.DrawText("CPU", x, y, width, height, sensor.Color, titleFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.CPUPower];
                        String drtext;
                        drtext = level < 10 ? $"[P] 0{level:N1}W" : $"[P] {level:N1}W";
                        fontColor = level < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText(drtext, x, y2, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.CPUCore];
                        fontColor = level < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText($"[C] {level:N1}℃", x, y1, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.CPULoad];
                        drtext = level < 10 ? $"[L] 0{level:N1}%" : $"[L] {level:N1}%";
                        fontColor = level < 10 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText(drtext, x, y0, width, height, fontColor, monFontSize);

                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.CPULoad];
                        break;

                    case LibreHardwareMonitorGaugeType.GPUMonitor:
                        bitmapBuilder.DrawText("GPU", x, y, width, height, sensor.Color, titleFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUPower];
                        drtext = level < 10 ? $"[P] 0{level:N1}W" : $"[P] {level:N1}W";
                        fontColor = level < 100 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText(drtext, x, y2, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUCore];
                        fontColor = level < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText($"[C] {level:N1}℃", x, y1, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPULoad];
                        drtext = level < 10 ? $"[L] 0{level:N1}%" : $"[L] {level:N1}%";
                        fontColor = level < 10 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText(drtext, x, y0, width, height, fontColor, monFontSize);

                        maxLevel = this._lastMaxLevels[(Int32)LibreHardwareMonitorGaugeType.GPULoad];
                        break;

                    case LibreHardwareMonitorGaugeType.MEMMonitor:
                        var levels = new Single[3];
                        var guages = new Int32[3];
                        levels[0] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        guages[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        levels[1] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        guages[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        levels[2] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        guages[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;

                        bitmapBuilder.DrawText("MEM(%)", x, y, width, height, sensor.Color, titleFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        drtext = levels[0] < 10 ? $"[S] 0{level:N1}%" : $"[S] {level:N1}%";
                        fontColor = levels[0] < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText(drtext, x, y0, width, height, fontColor, monFontSize);
                        
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        drtext = levels[1] < 10 ? $"[V] 0{level:N1}%" : $"[V] {level:N1}%";
                        fontColor = levels[1] < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText(drtext, x, y1, width, height, fontColor, monFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        drtext = levels[2] < 10 ? $"[G] 0{level:N1}%" : $"[G] {level:N1}%";
                        fontColor = levels[2] < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText(drtext, x, y2, width, height, fontColor, monFontSize);

                        var maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
                                maxLevel = this._lastMaxLevels[guages[i]];
                                break;
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.RAMMonitor:
                        levels = new Single[3];
                        guages = new Int32[3];
                        levels[0] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.Memory];
                        guages[0] = (Int32)LibreHardwareMonitorGaugeType.Memory;
                        levels[1] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrMemory];
                        guages[1] = (Int32)LibreHardwareMonitorGaugeType.VrMemory;
                        levels[2] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.GPUMemory];
                        guages[2] = (Int32)LibreHardwareMonitorGaugeType.GPUMemory;

                        bitmapBuilder.DrawText("RAM(GB)", x, y, width, height, sensor.Color, titleFontSize);

                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.RAM];
                        fontColor = levels[0] < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText($"[S] {level:N1}G", x, y0, width, height, fontColor, monFontSize);
                        
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VrRAM];
                        fontColor = levels[1] < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText($"[V] {level:N1}G", x, y1, width, height, fontColor, monFontSize);
                        
                        level = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.VRAM];
                        fontColor = levels[2] < 50 ? grayColor : BitmapColor.White;
                        bitmapBuilder.DrawText($"[G] {level/1024:N2}G", x, y2, width, height, fontColor, monFontSize);

                        maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
                                maxLevel = this._lastMaxLevels[guages[i]];
                                break;
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DT1Monitor:
                        bitmapBuilder.DrawText("DISK(℃)", x, y, width, height, sensor.Color, titleFontSize);

                        levels = new Single[3];
                        guages = new Int32[3];
                        for (var i = 0; i < 3; i++)
                        {
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DT1Monitor + i];
                            guages[i] = (Int32)LibreHardwareMonitorGaugeType.DT1Monitor + i;
                            var yn = n + vn * (i + 1);
                            imageIndex = this.GetImageIndex(LibreHardwareMonitorGaugeType.DT1Monitor);
                            //PluginLog.Info($"NVME{i + 1}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType + "," + imageIndex);
                            if (levels[i] > 0)
                            {
                                fontColor = levels[i] < 50 ? grayColor : BitmapColor.White;
                                bitmapBuilder.DrawText($"[{i + 1}] {levels[i]:N0}℃", x, yn, width, height, fontColor, monFontSize);
                            }
                        }

                        maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
                                maxLevel = this._lastMaxLevels[guages[i]];
                                break;
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DT4Monitor:
                        bitmapBuilder.DrawText("DISK(℃)", x, y, width, height, sensor.Color, titleFontSize);

                        levels = new Single[3];
                        guages = new Int32[3];
                        for (var i = 0; i < 3; i++)
                        {
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DT4Monitor + i];
                            guages[i] = (Int32)LibreHardwareMonitorGaugeType.DT4Monitor + i;
                            var yn = n + vn * (i + 1);
                            //PluginLog.Info($"NVME{i + 4}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType);
                            if (levels[i] > 0)
                            {
                                fontColor = levels[i] < 50 ? grayColor : BitmapColor.White;
                                bitmapBuilder.DrawText($"[{i + 4}] {levels[i]:N0}℃", x, yn, width, height, fontColor, monFontSize);
                            }
                        }

                        maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
                                maxLevel = this._lastMaxLevels[guages[i]];
                                break;
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU1Monitor:
                        bitmapBuilder.DrawText("DISK(%)", x, y, width, height, sensor.Color, titleFontSize);

                        levels = new Single[3];
                        guages = new Int32[3];
                        for (var i = 0; i < 3; i++)
                        {
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DU1Monitor + i];
                            guages[i] = (Int32)LibreHardwareMonitorGaugeType.DU1Monitor + i;
                            var yn = n + vn * (i + 1);
                            //PluginLog.Info($"NVME{i + 1}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType);
                            if (levels[i] > 0)
                            {
                                fontColor = levels[i] < 50 ? grayColor : BitmapColor.White;
                                drtext = levels[i] < 10 ? $"[{i + 1}] 0{levels[i]:N1}%" : $"[{i + 1}] {levels[i]:N1}%";
                                bitmapBuilder.DrawText(drtext, x, yn, width, height, fontColor, monFontSize);
                            }
                        }

                        maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
                                maxLevel = this._lastMaxLevels[guages[i]];
                                break;
                            }
                        }
                        break;

                    case LibreHardwareMonitorGaugeType.DU4Monitor:
                        bitmapBuilder.DrawText("DISK(%)", x, y, width, height, sensor.Color, titleFontSize);

                        levels = new Single[3];
                        guages = new Int32[3];
                        for (var i = 0; i < 3; i++)
                        {
                            levels[i] = this._lastLevels[(Int32)LibreHardwareMonitorGaugeType.DU4Monitor + i];
                            guages[i] = (Int32)LibreHardwareMonitorGaugeType.DU4Monitor + i;
                            var yn = n + vn * (i + 1);
                            //PluginLog.Info($"NVME{i + 4}: " + level + "℃," + x + "," + yn + "," + sensor.Color + "," + titleFontSize + "," + monFontSize + "," + gaugeType);
                            if (levels[i] > 0)
                            {
                                fontColor = levels[i] < 50 ? grayColor : BitmapColor.White;
                                drtext = levels[i] < 10 ? $"[{i + 4}] 0{levels[i]:N1}%" : $"[{i + 4}] {levels[i]:N1}%";
                                bitmapBuilder.DrawText(drtext, x, yn, width, height, fontColor, monFontSize);
                            }
                        }

                        maxLevels = levels.Max();
                        for (var i = 0; i < 3; i++)
                        {
                            if (levels[i] == maxLevels)
                            {
                                level = this._lastLevels[guages[i]];
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
                    && gaugeType != LibreHardwareMonitorGaugeType.RAMMonitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DT1Monitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DT4Monitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DU1Monitor
                    && gaugeType != LibreHardwareMonitorGaugeType.DU4Monitor)
                {
                    //PluginLog.Info($"g{imageIndex}.png");
                    var imageBytes = PluginResources.ReadBinaryFile($"g{imageIndex}.png");
                    bitmapBuilder.DrawImage(imageBytes, x, y0 - 9);
                }

                var rateLevel = level / maxLevel;
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
