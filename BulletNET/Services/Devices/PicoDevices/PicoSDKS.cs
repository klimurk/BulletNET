using BulletNET.Services.Devices.Base;
using BulletNET.Services.Devices.PicoDevices.Interface;
using MathNet.Numerics.IntegralTransforms;
using Pallet.Services.UserDialogService.Interfaces;
using PicoSDK;
using System.Numerics;

namespace BulletNET.Services.Devices.PicoDevices
{
    internal class PicoSDKS : Test, IPico
    {
        private readonly IUserDialogService _IUserDialogService;

        private PicoDevice? device;

        public bool isEnabled => !string.IsNullOrEmpty(device?.Serial);

        public PicoSDKS(IUserDialogService IUserDialogService)
        {
            _IUserDialogService = IUserDialogService;
        }

        public void Connect()
        {
            // variant 2206B serial GP781/0442
            try
            {
                var devices = PicoDevice.Enumerate(true);
                device = PicoDevice.Open(devices[0].serial, true);
            }
            catch
            {
                _IUserDialogService.ShowError("Pico is not finded", "Pico");
            }
        }

        public bool CheckVoltage(double minimum, double maximum, string valueName, string channel)
        {
            StartTest($"Check voltage {valueName} channel {channel}");
            device.StopStreaming();
            uint frequence = 200_000;
            int samplesCount = 200_000;

            List<float> samplesMeasure = GetSamplesData(samplesCount, frequence, channel).ToList();

            Measured = (float)samplesMeasure.Average();
            IsPassed = Measured >= minimum && Measured <= maximum;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation($"{TestName} failed. Retry?", "Test failed")) CheckVoltage(minimum, maximum, valueName, channel);
            return IsPassed;
        }

        public bool CheckFrequency(double minimum, double maximum, string TestName)
        {
            StartTest(TestName);

            const int samplesCount = 4_096;
            const uint frequence = 50_000_000;
            const uint samplingFrequency = 125;
            const string channel = "A";

            List<float> samplesMeasure = GetSamplesData(samplesCount, frequence, channel).ToList();

            Measured = (float)GetFrequencyFourier(samplesCount, samplingFrequency, samplesMeasure);

            IsPassed = Measured >= minimum && Measured <= maximum;

            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation($"{TestName} failed. Retry?", "Test failed")) CheckFrequency(minimum, maximum, TestName);
            return IsPassed; ;
        }

        private float GetFrequencyFourier(int samplesCount, uint samplingFrequency, IEnumerable<float> samplesMeasure)
        {
            List<float> samples = samplesMeasure.ToList();
            float hzPerSample = samplingFrequency / (float)samplesCount; // frequency resolution = sample f./N
            float freq = 0;

            double highestMagnitude = 0;

            Complex[] samplesComplex = new Complex[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                samplesComplex[i] = new Complex(samples[i], 0);
            }

            Fourier.Forward(samplesComplex, FourierOptions.NoScaling);

            for (int i = 1; i < samplesCount / 4; i++)
            {
                double currentMagnitude = samplesComplex[i].Magnitude;

                if (currentMagnitude > highestMagnitude)
                {
                    highestMagnitude = currentMagnitude;
                    freq = i * hzPerSample;
                }
            }

            return freq;
        }

        private IEnumerable<float> GetSamplesData(int samplesCount, uint frequence, string channel)
        {
            List<float> samplesMeasure = new();
            device.StopStreaming();

            device.EnableChannel(channel, "5V");

            device.StreamingData += (sender, args) =>
            {
                foreach (var (ch, ch_samples) in args.Data)
                {
                    samplesMeasure.AddRange(ch_samples);
                }
            };
            var samplesPerSecond = device.StartStreaming(frequence);
            while (samplesMeasure.Count < samplesCount)
            {
                Thread.Sleep(100);
            }

            device.StopStreaming();
            device.DisableChannel(channel);

            return samplesMeasure;
        }
    }
}