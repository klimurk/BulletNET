using BulletNET.Services.BarcodeReader;
using BulletNET.Services.BarcodeReader.Interface;
using BulletNET.Services.DelayService;
using BulletNET.Services.Devices.BluetoothDevice;
using BulletNET.Services.Devices.BluetoothDevice.Interface;
using BulletNET.Services.Devices.MansonDevice;
using BulletNET.Services.Devices.MansonDevice.Interface;
using BulletNET.Services.Devices.PicoDevices;
using BulletNET.Services.Devices.PicoDevices.Interface;
using BulletNET.Services.Devices.QuidoDevice;
using BulletNET.Services.Devices.QuidoDevice.Interfaces;
using BulletNET.Services.Managers;
using BulletNET.Services.SequenceReaderService;
using BulletNET.Services.SequenceReaderService.Interfaces;
using BulletNET.Services.UserDialogService;
using BulletNET.Services.UserDialogService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Pallet.Services.UserDialogService.Interfaces;

namespace BulletNET.Services
{
    internal static class Registator
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services) =>
            services
                .AddTransient<IUserDialogService, UserDialog>()
                .AddTransient<IOpenFileDialogService, OpenFileDialogService>()
                .AddSingleton<ISequenceReader, SequenceReader>()
                .AddSingleton<IBarcodeCRC, BarcodeCRC>()
                .AddSingleton<IManson, Manson>()
                .AddSingleton<IQuido, Quido>()
                .AddSingleton<IPico, Pico>()
                .AddSingleton<IBluetooth, Bluetooth>()
                .AddSingleton<IDelay, Delay>()
                .AddSingleton<IManagerUser, ManagerUser>()

            ;
    }
}