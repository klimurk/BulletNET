using BulletNET.Services.Devices.Base;
using BulletNET.Services.Devices.PicoDevices.Interface;
using BulletNET.Services.Devices.PicoDevices.PicoSupport;
using MathNet.Numerics.IntegralTransforms;
using Pallet.Services.UserDialogService.Interfaces;
using PicoPinnedArray;
using PicoStatus;
using PS2000AImports;
using System.Numerics;

namespace BulletNET.Services.Devices.PicoDevices
{
    public class Pico : Test, IPico
    {
        #region Services

        private readonly IUserDialogService _IUserDialogService;

        #endregion Services

        private const int __DUAL_SCOPE = 2;
        private const int __BUFFER_SIZE = 1024;
        private const int __MAX_CHANNELS = 4;

        private readonly ushort[] inputRanges = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };

        private short _handle;

        private int _digitalPorts;

        private short _maxValue;
        private int _channelCount;
        private ChannelSettings[] _channelSettings;

        private readonly short _oversample = 1;
        private Imports.ps2000aBlockReady _callbackDelegate;

        private bool _ready;

        private uint _Status;

        private uint Status
        {
            get => _Status;
            set
            {
                _Status = value;
                CheckStatus(value);
                isEnabled = value == StatusCodes.PICO_OK;
            }
        }

        private void CheckStatus(uint status)
        {
            switch (status)
            {
                case StatusCodes.PICO_MEMORY_FAIL:
                    _IUserDialogService.ShowError("Not enough memory could be allocated on the host machine", "Pico");
                    break;

                //case StatusCodes.PICO_NOT_FOUND:
                //    _IUserDialogService.ShowError("No Pico Technology device could be found", "Pico");
                //    break;

                case StatusCodes.PICO_FW_FAIL:
                    _IUserDialogService.ShowError("Unable to download firmware", "Pico");
                    break;

                //case StatusCodes.PICO_OPEN_OPERATION_IN_PROGRESS:
                //    _IUserDialogService.ShowError("The driver is busy opening a device", "Pico");
                //    break;

                case StatusCodes.PICO_OPERATION_FAILED:
                    _IUserDialogService.ShowError("An unspecified failure occurred", "Pico");
                    break;

                case StatusCodes.PICO_NOT_RESPONDING:
                    _IUserDialogService.ShowError("The PicoScope is not responding to commands from the PC", "Pico");
                    break;

                case StatusCodes.PICO_CONFIG_FAIL:
                    _IUserDialogService.ShowError("The configuration information in the PicoScope is corrupt or missing", "Pico");
                    break;

                case StatusCodes.PICO_KERNEL_DRIVER_TOO_OLD:
                    _IUserDialogService.ShowError("The picopp.sys file is too old to be used with the device driver", "Pico");
                    break;

                case StatusCodes.PICO_EEPROM_CORRUPT:
                    _IUserDialogService.ShowError("The EEPROM has become corrupt, so the device will use a default setting", "Pico");
                    break;

                case StatusCodes.PICO_OS_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The operating system on the PC is not supported by this driver", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_HANDLE:
                    _IUserDialogService.ShowError("There is no device with the handle value passed", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_PARAMETER:
                    _IUserDialogService.ShowError("A parameter value is not valid", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_TIMEBASE:
                    _IUserDialogService.ShowError("The timebase is not supported or is invalid", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_VOLTAGE_RANGE:
                    _IUserDialogService.ShowError("The voltage range is not supported or is invalid", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_CHANNEL:
                    _IUserDialogService.ShowError("The channel number is not valid on this device or no channels have been set", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_TRIGGER_CHANNEL:
                    _IUserDialogService.ShowError("The channel set for a trigger is not available on this device", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_CONDITION_CHANNEL:
                    _IUserDialogService.ShowError("The channel set for a condition is not available on this device", "Pico");
                    break;

                case StatusCodes.PICO_NO_SIGNAL_GENERATOR:
                    _IUserDialogService.ShowError("The device does not have a signal generator", "Pico");
                    break;

                case StatusCodes.PICO_STREAMING_FAILED:
                    _IUserDialogService.ShowError("Streaming has failed to start or has stopped without user request", "Pico");
                    break;

                case StatusCodes.PICO_BLOCK_MODE_FAILED:
                    _IUserDialogService.ShowError("Block failed to start - a parameter may have been set wrongly", "Pico");
                    break;

                case StatusCodes.PICO_NULL_PARAMETER:
                    _IUserDialogService.ShowError("A parameter that was required is NULL", "Pico");
                    break;

                case StatusCodes.PICO_ETS_MODE_SET:
                    _IUserDialogService.ShowError("The current functionality is not available while using ETS capture mode", "Pico");
                    break;

                case StatusCodes.PICO_DATA_NOT_AVAILABLE:
                    _IUserDialogService.ShowError(">No data is available from a run block call", "Pico");
                    break;

                case StatusCodes.PICO_STRING_BUFFER_TO_SMALL:
                    _IUserDialogService.ShowError("The buffer passed for the information was too small", "Pico");
                    break;

                case StatusCodes.PICO_ETS_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("ETS is not supported on this device", "Pico");
                    break;

                case StatusCodes.PICO_AUTO_TRIGGER_TIME_TO_SHORT:
                    _IUserDialogService.ShowError("The auto trigger time is less than the time it will take to collect the pre-trigger data", "Pico");
                    break;

                case StatusCodes.PICO_BUFFER_STALL:
                    _IUserDialogService.ShowError("The collection of data has stalled as unread data would be overwritten", "Pico");
                    break;

                case StatusCodes.PICO_TOO_MANY_SAMPLES:
                    _IUserDialogService.ShowError("Number of samples requested is more than available in the current memory segment", "Pico");
                    break;

                case StatusCodes.PICO_TOO_MANY_SEGMENTS:
                    _IUserDialogService.ShowError("Not possible to create number of segments requested", "Pico");
                    break;

                case StatusCodes.PICO_PULSE_WIDTH_QUALIFIER:
                    _IUserDialogService.ShowError("A null pointer has been passed in the trigger function or one of the parameters is out of range", "Pico");
                    break;

                case StatusCodes.PICO_DELAY:
                    _IUserDialogService.ShowError("One or more of the hold-off parameters are out of range", "Pico");
                    break;

                case StatusCodes.PICO_SOURCE_DETAILS:
                    _IUserDialogService.ShowError("One or more of the source details are incorrect", "Pico");
                    break;

                case StatusCodes.PICO_CONDITIONS:
                    _IUserDialogService.ShowError("One or more of the conditions are incorrect", "Pico");
                    break;

                case StatusCodes.PICO_USER_CALLBACK:
                    _IUserDialogService.ShowError("The driver's thread is currently in the -API-Ready callback function and therefore the action cannot be carried out", "Pico");
                    break;

                case StatusCodes.PICO_DEVICE_SAMPLING:
                    _IUserDialogService.ShowError("An attempt is being made to get stored data while streaming. Either stop streaming by calling -API-Stop, or use -API-GetStreamingLatestValues", "Pico");
                    break;

                case StatusCodes.PICO_NO_SAMPLES_AVAILABLE:
                    _IUserDialogService.ShowError("Data is unavailable because a run has not been completed", "Pico");
                    break;

                case StatusCodes.PICO_SEGMENT_OUT_OF_RANGE:
                    _IUserDialogService.ShowError("The memory segment index is out of range", "Pico");
                    break;

                case StatusCodes.PICO_BUSY:
                    _IUserDialogService.ShowError("he device is busy so data cannot be returned yet", "Pico");
                    break;

                case StatusCodes.PICO_STARTINDEX_INVALID:
                    _IUserDialogService.ShowError("The start time to get stored data is out of range", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_INFO:
                    _IUserDialogService.ShowError("The information number requested is not a valid number", "Pico");
                    break;

                case StatusCodes.PICO_INFO_UNAVAILABLE:
                    _IUserDialogService.ShowError("The handle is invalid so no information is available about the device. Only PICO_DRIVER_VERSION is available", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_SAMPLE_INTERVAL:
                    _IUserDialogService.ShowError("The sample interval selected for streaming is out of range", "Pico");
                    break;

                case StatusCodes.PICO_TRIGGER_ERROR:
                    _IUserDialogService.ShowError("ETS is set but no trigger has been set. A trigger setting is required for ETS", "Pico");
                    break;

                case StatusCodes.PICO_MEMORY:
                    _IUserDialogService.ShowError("Driver cannot allocate memory", "Pico");
                    break;

                case StatusCodes.PICO_SIG_GEN_PARAM:
                    _IUserDialogService.ShowError("Incorrect parameter passed to the signal generator", "Pico");
                    break;

                case StatusCodes.PICO_SHOTS_SWEEPS_WARNING:
                    _IUserDialogService.ShowError("Conflict between the shots and sweeps parameters sent to the signal generator", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_TRIGGER_SOURCE:
                    _IUserDialogService.ShowError("A software trigger has been sent but the trigger source is not a software trigger", "Pico");
                    break;

                case StatusCodes.PICO_AUX_OUTPUT_CONFLICT:
                    _IUserDialogService.ShowError("An -API-SetTrigger call has found a conflict between the trigger source and the AUX output enable", "Pico");
                    break;

                case StatusCodes.PICO_AUX_OUTPUT_ETS_CONFLICT:
                    _IUserDialogService.ShowError("ETS mode is being used and AUX is set as an input", "Pico");
                    break;

                case StatusCodes.PICO_WARNING_EXT_THRESHOLD_CONFLICT:
                    _IUserDialogService.ShowError("Attempt to set different EXT input thresholds set for signal generator and oscilloscope trigger", "Pico");
                    break;

                case StatusCodes.PICO_WARNING_AUX_OUTPUT_CONFLICT:
                    _IUserDialogService.ShowError("An -API-SetTrigger... function has set AUX as an output and the signal generator is using it as a trigger.", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_OUTPUT_OVER_VOLTAGE:
                    _IUserDialogService.ShowError("The combined peak to peak voltage and the analog offset voltage exceed the maximum voltage the signal generator can produce", "Pico");
                    break;

                case StatusCodes.PICO_DELAY_NULL:
                    _IUserDialogService.ShowError("NULL pointer passed as delay parameter", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_BUFFER:
                    _IUserDialogService.ShowError("The buffers for overview data have not been set while streaming", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_OFFSET_VOLTAGE:
                    _IUserDialogService.ShowError("The analog offset voltage is out of range", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_PK_TO_PK:
                    _IUserDialogService.ShowError("The analog peak-to-peak voltage is out of range", "Pico");
                    break;

                case StatusCodes.PICO_CANCELLED:
                    _IUserDialogService.ShowError("A block collection has been canceled", "Pico");
                    break;

                case StatusCodes.PICO_SEGMENT_NOT_USED:
                    _IUserDialogService.ShowError("The segment index is not currently being used", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_CALL:
                    _IUserDialogService.ShowError("The wrong GetValues function has been called for the collection mode in use", "Pico");
                    break;

                case StatusCodes.PICO_GET_VALUES_INTERRUPTED:
                    _IUserDialogService.ShowError("Interruption get value", "Pico");
                    break;

                case StatusCodes.PICO_NOT_USED:
                    _IUserDialogService.ShowError("The function is not available", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_SAMPLERATIO:
                    _IUserDialogService.ShowError("The aggregation ratio requested is out of range", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_STATE:
                    _IUserDialogService.ShowError("Device is in an invalid state", "Pico");
                    break;

                case StatusCodes.PICO_NOT_ENOUGH_SEGMENTS:
                    _IUserDialogService.ShowError("The number of segments allocated is fewer than the number of captures requested", "Pico");
                    break;

                case StatusCodes.PICO_DRIVER_FUNCTION:
                    _IUserDialogService.ShowError("A driver function has already been called and not yet finished. Only one call to the driver can be made at any one time", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_COUPLING:
                    _IUserDialogService.ShowError("An invalid coupling type was specified in -API-SetChannel", "Pico");
                    break;

                case StatusCodes.PICO_BUFFERS_NOT_SET:
                    _IUserDialogService.ShowError("An attempt was made to get data before a data buffer was defined", "Pico");
                    break;

                case StatusCodes.PICO_RATIO_MODE_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The selected down sampling mode (used for data reduction) is not allowed", "Pico");
                    break;

                case StatusCodes.PICO_RAPID_NOT_SUPPORT_AGGREGATION:
                    _IUserDialogService.ShowError("Aggregation was requested in rapid block mode", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_TRIGGER_PROPERTY:
                    _IUserDialogService.ShowError("An invalid parameter was passed to -API-SetTriggerChannelProperties", "Pico");
                    break;

                case StatusCodes.PICO_INTERFACE_NOT_CONNECTED:
                    _IUserDialogService.ShowError("The driver was unable to contact the oscilloscope", "Pico");
                    break;

                case StatusCodes.PICO_RESISTANCE_AND_PROBE_NOT_ALLOWED:
                    _IUserDialogService.ShowError("Resistance-measuring mode is not allowed in conjunction with the specified probe", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_WAVEFORM_SETUP_FAILED:
                    _IUserDialogService.ShowError("A problem occurred in -API-SetSigGenBuiltIn or -API-SetSigGenArbitrary", "Pico");
                    break;

                case StatusCodes.PICO_FPGA_FAIL:
                    _IUserDialogService.ShowError("FPGA not successfully set up", "Pico");
                    break;

                case StatusCodes.PICO_POWER_MANAGER:
                    _IUserDialogService.ShowError("Fail Power manager", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_ANALOGUE_OFFSET:
                    _IUserDialogService.ShowError("An impossible analog offset value was specified in -API-SetChannel", "Pico");
                    break;

                case StatusCodes.PICO_PLL_LOCK_FAILED:
                    _IUserDialogService.ShowError("There is an error within the device hardware", "Pico");
                    break;

                case StatusCodes.PICO_ANALOG_BOARD:
                    _IUserDialogService.ShowError("There is an error within the device hardware", "Pico");
                    break;

                case StatusCodes.PICO_CONFIG_FAIL_AWG:
                    _IUserDialogService.ShowError("Unable to configure the signal generator", "Pico");
                    break;

                case StatusCodes.PICO_INITIALISE_FPGA:
                    _IUserDialogService.ShowError("The FPGA cannot be initialized, so unit cannot be opened", "Pico");
                    break;

                case StatusCodes.PICO_EXTERNAL_FREQUENCY_INVALID:
                    _IUserDialogService.ShowError("The frequency for the external clock is not within 15% of the nominal value", "Pico");
                    break;

                case StatusCodes.PICO_CLOCK_CHANGE_ERROR:
                    _IUserDialogService.ShowError("The FPGA could not lock the clock signal", "Pico");
                    break;

                case StatusCodes.PICO_TRIGGER_AND_EXTERNAL_CLOCK_CLASH:
                    _IUserDialogService.ShowError("You are trying to configure the AUX input as both a trigger and a reference clock", "Pico");
                    break;

                case StatusCodes.PICO_PWQ_AND_EXTERNAL_CLOCK_CLASH:
                    _IUserDialogService.ShowError("You are trying to configure the AUX input as both a pulse width qualifier and a reference clock.", "Pico");
                    break;

                case StatusCodes.PICO_UNABLE_TO_OPEN_SCALING_FILE:
                    _IUserDialogService.ShowError("The requested scaling file cannot be opened", "Pico");
                    break;

                case StatusCodes.PICO_MEMORY_CLOCK_FREQUENCY:
                    _IUserDialogService.ShowError("The frequency of the memory is reporting incorrectly", "Pico");
                    break;

                case StatusCodes.PICO_I2C_NOT_RESPONDING:
                    _IUserDialogService.ShowError("The I2C that is being actioned is not responding to requests", "Pico");
                    break;

                case StatusCodes.PICO_NO_CAPTURES_AVAILABLE:
                    _IUserDialogService.ShowError("There are no captures available and therefore no data can be returned", "Pico");
                    break;

                case StatusCodes.PICO_TOO_MANY_TRIGGER_CHANNELS_IN_USE:
                    _IUserDialogService.ShowError("The number of trigger channels is greater than 4, except for a PS4824 where 8 channels are allowed for rising/falling/rising_or_falling trigger directions", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_TRIGGER_DIRECTION:
                    _IUserDialogService.ShowError("When more than 4 trigger channels are set on a PS4824 and the direction is out of range", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_TRIGGER_STATES:
                    _IUserDialogService.ShowError("When more than 4 trigger channels are set and their trigger condition states are not -API-_CONDITION_TRUE", "Pico");
                    break;

                case StatusCodes.PICO_NOT_USED_IN_THIS_CAPTURE_MODE:
                    _IUserDialogService.ShowError("The capture mode the device is currently running in does not support the current request", "Pico");
                    break;

                case StatusCodes.PICO_GET_DATA_ACTIVE:
                    _IUserDialogService.ShowError("Get data active", "Pico");
                    break;

                case StatusCodes.PICO_IP_NETWORKED:
                    _IUserDialogService.ShowError("The device is currently connected via the IP Network socket and thus the call made is not supported", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_IP_ADDRESS:
                    _IUserDialogService.ShowError("An incorrect IP address has been passed to the driver", "Pico");
                    break;

                case StatusCodes.PICO_IPSOCKET_FAILED:
                    _IUserDialogService.ShowError("The IP socket has failed", "Pico");
                    break;

                case StatusCodes.PICO_IPSOCKET_TIMEDOUT:
                    _IUserDialogService.ShowError("The IP socket has timed out", "Pico");
                    break;

                case StatusCodes.PICO_SETTINGS_FAILED:
                    _IUserDialogService.ShowError("Failed to apply the requested settings", "Pico");
                    break;

                case StatusCodes.PICO_NETWORK_FAILED:
                    _IUserDialogService.ShowError("The network connection has failed", "Pico");
                    break;

                case StatusCodes.PICO_WS2_32_DLL_NOT_LOADED:
                    _IUserDialogService.ShowError("Unable to load the WS2 DLL", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_IP_PORT:
                    _IUserDialogService.ShowError("The specified IP port is invalid", "Pico");
                    break;

                case StatusCodes.PICO_COUPLING_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The type of coupling requested is not supported on the opened device", "Pico");
                    break;

                case StatusCodes.PICO_BANDWIDTH_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("Bandwidth limiting is not supported on the opened device", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_BANDWIDTH:
                    _IUserDialogService.ShowError("The value requested for the bandwidth limit is out of range", "Pico");
                    break;

                case StatusCodes.PICO_AWG_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The arbitrary waveform generator is not supported by the opened device", "Pico");
                    break;

                case StatusCodes.PICO_ETS_NOT_RUNNING:
                    _IUserDialogService.ShowError("Data has been requested with ETS mode set but run block has not been called, or stop has been called", "Pico");
                    break;

                case StatusCodes.PICO_SIG_GEN_WHITENOISE_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("White noise output is not supported on the opened device", "Pico");
                    break;

                case StatusCodes.PICO_SIG_GEN_WAVETYPE_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The wave type requested is not supported by the opened device", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_DIGITAL_PORT:
                    _IUserDialogService.ShowError("The requested digital port number is out of range (MSOs only)", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_DIGITAL_CHANNEL:
                    _IUserDialogService.ShowError("The digital channel is not in the range -API-_DIGITAL_CHANNEL0 to -API-_DIGITAL_CHANNEL15, the digital channels that are supported.", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_DIGITAL_TRIGGER_DIRECTION:
                    _IUserDialogService.ShowError("The digital trigger direction is not a valid trigger direction and should be equal in value to one of the -API-_DIGITAL_DIRECTION enumerations.", "Pico");
                    break;

                case StatusCodes.PICO_SIG_GEN_PRBS_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("Signal generator does not generate pseudo-random binary sequence", "Pico");
                    break;

                case StatusCodes.PICO_ETS_NOT_AVAILABLE_WITH_LOGIC_CHANNELS:
                    _IUserDialogService.ShowError("When a digital port is enabled, ETS sample mode is not available for use", "Pico");
                    break;

                case StatusCodes.PICO_WARNING_REPEAT_VALUE:
                    _IUserDialogService.ShowError("Warning repeat value", "Pico");
                    break;

                case StatusCodes.PICO_POWER_SUPPLY_CONNECTED:
                    _IUserDialogService.ShowError("4-channel scopes only: The DC power supply is connected", "Pico");
                    break;

                case StatusCodes.PICO_POWER_SUPPLY_NOT_CONNECTED:
                    _IUserDialogService.ShowError("4-channel scopes only: The DC power supply is not connected", "Pico");
                    break;

                case StatusCodes.PICO_POWER_SUPPLY_REQUEST_INVALID:
                    _IUserDialogService.ShowError("Incorrect power mode passed for current power source", "Pico");
                    break;

                case StatusCodes.PICO_POWER_SUPPLY_UNDERVOLTAGE:
                    _IUserDialogService.ShowError("The supply voltage from the USB source is too low", "Pico");
                    break;

                case StatusCodes.PICO_CAPTURING_DATA:
                    _IUserDialogService.ShowError("The oscilloscope is in the process of capturing data", "Pico");
                    break;

                case StatusCodes.PICO_USB3_0_DEVICE_NON_USB3_0_PORT:
                    _IUserDialogService.ShowError("A USB 3.0 device is connected to a non-USB 3.0 port", "Pico");
                    break;

                case StatusCodes.PICO_NOT_SUPPORTED_BY_THIS_DEVICE:
                    _IUserDialogService.ShowError("A function has been called that is not supported by the current device", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_DEVICE_RESOLUTION:
                    _IUserDialogService.ShowError("The device resolution is invalid (out of range)", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_NUMBER_CHANNELS_FOR_RESOLUTION:
                    _IUserDialogService.ShowError("The number of channels that can be enabled is limited in 15 and 16-bit modes. (Flexible Resolution Oscilloscopes only)", "Pico");
                    break;

                case StatusCodes.PICO_CHANNEL_DISABLED_DUE_TO_USB_POWERED:
                    _IUserDialogService.ShowError("USB power not sufficient for all requested channels", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_DC_VOLTAGE_NOT_CONFIGURABLE:
                    _IUserDialogService.ShowError("The signal generator does not have a configurable DC offset", "Pico");
                    break;

                case StatusCodes.PICO_NO_TRIGGER_ENABLED_FOR_TRIGGER_IN_PRE_TRIG:
                    _IUserDialogService.ShowError("An attempt has been made to define pre-trigger delay without first enabling a trigger", "Pico");
                    break;

                case StatusCodes.PICO_TRIGGER_WITHIN_PRE_TRIG_NOT_ARMED:
                    _IUserDialogService.ShowError("An attempt has been made to define pre-trigger delay without first arming a trigger", "Pico");
                    break;

                case StatusCodes.PICO_TRIGGER_WITHIN_PRE_NOT_ALLOWED_WITH_DELAY:
                    _IUserDialogService.ShowError("Pre-trigger delay and post-trigger delay cannot be used at the same time", "Pico");
                    break;

                case StatusCodes.PICO_TRIGGER_INDEX_UNAVAILABLE:
                    _IUserDialogService.ShowError("The array index points to a nonexistent trigger", "Pico");
                    break;

                case StatusCodes.PICO_TOO_MANY_CHANNELS_IN_USE:
                    _IUserDialogService.ShowError("There are more 4 analog channels with a trigger condition set", "Pico");
                    break;

                case StatusCodes.PICO_NULL_CONDITIONS:
                    _IUserDialogService.ShowError("The condition parameter is a null pointer", "Pico");
                    break;

                case StatusCodes.PICO_DUPLICATE_CONDITION_SOURCE:
                    _IUserDialogService.ShowError("There is more than one condition pertaining to the same channel", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_CONDITION_INFO:
                    _IUserDialogService.ShowError("The parameter relating to condition information is out of range", "Pico");
                    break;

                case StatusCodes.PICO_SETTINGS_READ_FAILED:
                    _IUserDialogService.ShowError("Reading the metadata has failed", "Pico");
                    break;

                case StatusCodes.PICO_SETTINGS_WRITE_FAILED:
                    _IUserDialogService.ShowError("Writing the metadata has failed", "Pico");
                    break;

                case StatusCodes.PICO_ARGUMENT_OUT_OF_RANGE:
                    _IUserDialogService.ShowError("A parameter has a value out of the expected range", "Pico");
                    break;

                case StatusCodes.PICO_HARDWARE_VERSION_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The driver does not support the hardware variant connected", "Pico");
                    break;

                case StatusCodes.PICO_DIGITAL_HARDWARE_VERSION_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The driver does not support the digital hardware variant connected", "Pico");
                    break;

                case StatusCodes.PICO_ANALOGUE_HARDWARE_VERSION_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("The driver does not support the analog hardware variant connected", "Pico");
                    break;

                case StatusCodes.PICO_UNABLE_TO_CONVERT_TO_RESISTANCE:
                    _IUserDialogService.ShowError("Converting a channel's ADC value to resistance has failed", "Pico");
                    break;

                case StatusCodes.PICO_DUPLICATED_CHANNEL:
                    _IUserDialogService.ShowError("The channel is listed more than once in the function call", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_RESISTANCE_CONVERSION:
                    _IUserDialogService.ShowError("The range cannot have resistance conversion applied", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_VALUE_IN_MAX_BUFFER:
                    _IUserDialogService.ShowError("An invalid value is in the max buffer", "Pico");
                    break;

                case StatusCodes.PICO_INVALID_VALUE_IN_MIN_BUFFER:
                    _IUserDialogService.ShowError("An invalid value is in the min buffer", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_FREQUENCY_OUT_OF_RANGE:
                    _IUserDialogService.ShowError("When calculating the frequency for phase conversion, the frequency is greater than that supported by the current variant", "Pico");
                    break;

                case StatusCodes.PICO_EEPROM2_CORRUPT:
                    _IUserDialogService.ShowError("The device's EEPROM is corrupt. Contact Pico Technology support: https://www.picotech.com/tech-support", "Pico");
                    break;

                case StatusCodes.PICO_EEPROM2_FAIL:
                    _IUserDialogService.ShowError("The EEPROM has failed", "Pico");
                    break;

                case StatusCodes.PICO_SERIAL_BUFFER_TOO_SMALL:
                    _IUserDialogService.ShowError("The serial buffer is too small for the required information", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_TRIGGER_AND_EXTERNAL_CLOCK_CLASH:
                    _IUserDialogService.ShowError("The signal generator trigger and the external clock have both been set. This is not allowed", "Pico");
                    break;

                case StatusCodes.PICO_WARNING_SIGGEN_AUXIO_TRIGGER_DISABLED:
                    _IUserDialogService.ShowError("The AUX trigger was enabled and the external clock has been enabled, so the AUX has been automatically disabled", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_GATING_AUXIO_NOT_AVAILABLE:
                    _IUserDialogService.ShowError("The AUX I/O was set as a scope trigger and is now being set as a signal generator gating trigger. This is not allowed", "Pico");
                    break;

                case StatusCodes.PICO_SIGGEN_GATING_AUXIO_ENABLED:
                    _IUserDialogService.ShowError(" The AUX I/O was set by the signal generator as a gating trigger and is now being set as a scope trigger. This is not allowed", "Pico");
                    break;

                case StatusCodes.PICO_RESOURCE_ERROR:
                    _IUserDialogService.ShowError("A resource has failed to initialize", "Pico");
                    break;

                case StatusCodes.PICO_TEMPERATURE_TYPE_INVALID:
                    _IUserDialogService.ShowError("The temperature type is out of range", "Pico");
                    break;

                case StatusCodes.PICO_TEMPERATURE_TYPE_NOT_SUPPORTED:
                    _IUserDialogService.ShowError("A requested temperature type is not supported on this device", "Pico");
                    break;

                case StatusCodes.PICO_TIMEOUT:
                    _IUserDialogService.ShowError("A read/write to the device has timed out", "Pico");
                    break;

                case StatusCodes.PICO_DEVICE_NOT_FUNCTIONING:
                    _IUserDialogService.ShowError("The device cannot be connected correctly", "Pico");
                    break;

                case StatusCodes.PICO_INTERNAL_ERROR:
                    _IUserDialogService.ShowError("The driver has experienced an unknown error and is unable to recover from this error", "Pico");
                    break;

                case StatusCodes.PICO_MULTIPLE_DEVICES_FOUND:
                    _IUserDialogService.ShowError("Used when opening units via IP and more than multiple units have the same ip address", "Pico");
                    break;

                case StatusCodes.PICO_WARNING_NUMBER_OF_SEGMENTS_REDUCED:
                    _IUserDialogService.ShowError("Number of segments reduced", "Pico");
                    break;

                case StatusCodes.PICO_CAL_PINS_STATES:
                    _IUserDialogService.ShowError("The calibration pin states argument is out of range", "Pico");
                    break;

                case StatusCodes.PICO_CAL_PINS_FREQUENCY:
                    _IUserDialogService.ShowError("The calibration pin frequency argument is out of range", "Pico");
                    break;

                case StatusCodes.PICO_CAL_PINS_AMPLITUDE:
                    _IUserDialogService.ShowError("The calibration pin amplitude argument is out of range", "Pico");
                    break;

                case StatusCodes.PICO_CAL_PINS_WAVETYPE:
                    _IUserDialogService.ShowError("The calibration pin wavetype argument is out of range", "Pico");
                    break;

                case StatusCodes.PICO_CAL_PINS_OFFSET:
                    _IUserDialogService.ShowError("The calibration pin offset argument is out of range", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_FAULT:
                    _IUserDialogService.ShowError("The probe's identity has a problem", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_IDENTITY_UNKNOWN:
                    _IUserDialogService.ShowError("The probe has not been identified", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_POWER_DC_POWER_SUPPLY_REQUIRED:
                    _IUserDialogService.ShowError("Enabling the probe would cause the device to exceed the allowable current limit", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_NOT_POWERED_WITH_DC_POWER_SUPPLY:
                    _IUserDialogService.ShowError("The DC power supply is connected; enabling the probe would cause the device to exceed the allowable current limit", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_CONFIG_FAILURE:
                    _IUserDialogService.ShowError("Failed to complete probe configuration", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_INTERACTION_CALLBACK:
                    _IUserDialogService.ShowError("Failed to set the callback function, as currently in current callback function", "Pico");
                    break;

                case StatusCodes.PICO_UNKNOWN_INTELLIGENT_PROBE:
                    _IUserDialogService.ShowError("The probe has been verified but not know on this driver", "Pico");
                    break;

                case StatusCodes.PICO_INTELLIGENT_PROBE_CORRUPT:
                    _IUserDialogService.ShowError("The intelligent probe cannot be verified", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_COLLECTION_NOT_STARTED:
                    _IUserDialogService.ShowError("The callback is null, probe collection will only start when first callback is a none null pointer", "Pico");
                    break;

                case StatusCodes.PICO_PROBE_POWER_CONSUMPTION_EXCEEDED:
                    _IUserDialogService.ShowError("The current drawn by the probe(s) has exceeded the allowed limit", "Pico");
                    break;

                case StatusCodes.PICO_WARNING_PROBE_CHANNEL_OUT_OF_SYNC:
                    _IUserDialogService.ShowError("The channel range limits have changed due to connecting or disconnecting a probe the channel has been enabled", "Pico");
                    break;

                case StatusCodes.PICO_DEVICE_TIME_STAMP_RESET:
                    _IUserDialogService.ShowError("The time stamp per waveform segment has been reset", "Pico");
                    break;

                case StatusCodes.PICO_WATCHDOGTIMER:
                    _IUserDialogService.ShowError("An internal error has occurred and a watchdog timer has been called", "Pico");
                    break;

                case StatusCodes.PICO_IPP_NOT_FOUND:
                    _IUserDialogService.ShowError("The picoipp.dll has not been found", "Pico");
                    break;

                case StatusCodes.PICO_IPP_NO_FUNCTION:
                    _IUserDialogService.ShowError("A function in the picoipp.dll does not exist", "Pico");
                    break;

                case StatusCodes.PICO_IPP_ERROR:
                    _IUserDialogService.ShowError("The Pico IPP call has failed", "Pico");
                    break;

                case StatusCodes.PICO_SHADOW_CAL_NOT_AVAILABLE:
                    _IUserDialogService.ShowError("Shadow calibration is not available on this device", "Pico");
                    break;

                case StatusCodes.PICO_SHADOW_CAL_DISABLED:
                    _IUserDialogService.ShowError("Shadow calibration is currently disabled", "Pico");
                    break;

                case StatusCodes.PICO_SHADOW_CAL_ERROR:
                    _IUserDialogService.ShowError("Shadow calibration error has occurred", "Pico");
                    break;

                case StatusCodes.PICO_SHADOW_CAL_CORRUPT:
                    _IUserDialogService.ShowError("The shadow calibration is corrupt", "Pico");
                    break;

                case StatusCodes.PICO_DEVICE_MEMORY_OVERFLOW:
                    _IUserDialogService.ShowError("The memory onboard the device has overflowed", "Pico");
                    break;
            }
        }

        public Pico(IUserDialogService IUserDialogService)
        {
            _IUserDialogService = IUserDialogService;
        }

        /// <summary>
        /// Connects to device, sets default settings
        /// </summary>
        /// <returns> 0 if succesfull</returns>
        ///
        public void Connect()
        {
            Status = Imports.OpenUnit(out _handle, null);

            Console.WriteLine("Handle: {0}", _handle);

            GetDeviceInfo();

            _channelSettings = new ChannelSettings[__MAX_CHANNELS];
            _channelCount = 2;

            // turn on channels A and B
            _channelSettings[0].enabled = true;
            _channelSettings[1].enabled = true;

            _channelSettings[0].couplingType = Imports.CouplingType.PS2000A_DC;
            _channelSettings[0].range = Imports.Range.Range_5V;

            _channelSettings[1].couplingType = Imports.CouplingType.PS2000A_DC;
            _channelSettings[1].range = Imports.Range.Range_5V;

            //isEnabled = Status == StatusCodes.PICO_OK;
        }

        /****************************************************************************
        * Adc_to_mv
        *
        * Convert an 16-bit ADC count into millivolts
        ****************************************************************************/

        private int Adc_to_mv(int raw, int ch)
        {
            return raw * inputRanges[ch] / _maxValue;
        }

        /****************************************************************************
        * BlockCallback
        * used by data block collection calls, on receipt of data.
        * used to set global flags etc checked by user routines
        ****************************************************************************/

        private void BlockCallback(short handle, uint status, IntPtr pVoid)
        {
            // flag to say done reading data
            _ready = true;
        }

        /// <summary>
        /// Reads voltage on both channels
        /// </summary>
        /// <returns>voltage [mV]</returns>
        private double[] ReadVoltage()
        {
            int[][] voltageData = ReadData(2, 200000);

            double ChannelA = voltageData[0].Average();
            double ChannelB = voltageData[1].Average();

            return new double[] { ChannelA, ChannelB };
        }

        public bool CheckFrequency(double minimum, double maximum, string testName)
        {
            StartTest(testName);
            int[][] voltageData = ReadData(2, 4096);

            Complex[] samples = new Complex[4096];

            for (int i = 0; i < 4096; i++)
            {
                samples[i] = new Complex(voltageData[0][i], 0);
            }

            Fourier.Forward(samples);

            Imports.Stop(_handle);

            float ds = 125 / (float)4096; // frequency resolution = sample f./N
            float freq = 0;

            double highest = 0;

            for (int i = 5; i < 2048; i++)
            {
                double magnitude = samples[i].Magnitude;

                if (magnitude > 500 && magnitude > highest)
                {
                    highest = magnitude;
                    freq = i * ds;
                }
            }
            Measured = freq;

            IsPassed = Measured >= minimum && Measured <= maximum;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) CheckFrequency(minimum, maximum, testName);
            return IsPassed; ;
        }

        private int[][] ReadData(uint timebase, uint sampleCount)
        {
            for (int i = 0; i < _channelCount; i++) // reset channels to most recent settings
            {
                uint statusA = Imports.SetChannel(_handle,
                                   Imports.Channel.ChannelA + i,
                                   (short)(_channelSettings[(int)(Imports.Channel.ChannelA + i)].enabled ? 1 : 0),
                                   _channelSettings[(int)(Imports.Channel.ChannelA + i)].couplingType,
                                   _channelSettings[(int)(Imports.Channel.ChannelA + i)].range,
                                   0);
            }
            /* Trigger disabled	*/
            SetTrigger(null, 0, null, 0, null, null, 0, 0, 0, null, 0);

            int offset = 0;

            PinnedArray<short>[] minPinned = new PinnedArray<short>[_channelCount];
            PinnedArray<short>[] maxPinned = new PinnedArray<short>[_channelCount];
            PinnedArray<short>[] digiPinned = new PinnedArray<short>[_digitalPorts];

            int[][] voltageData = new int[][] { new int[sampleCount], new int[sampleCount] };

            uint status = StatusCodes.PICO_OK; // PICO_OK

            for (int i = 0; i < _channelCount; i++)
            {
                short[] minBuffers = new short[sampleCount];
                short[] maxBuffers = new short[sampleCount];

                minPinned[i] = new PinnedArray<short>(minBuffers);
                maxPinned[i] = new PinnedArray<short>(maxBuffers);

                Status = Imports.SetDataBuffers(_handle, (Imports.Channel)i, maxBuffers, minBuffers, (int)sampleCount, 0, Imports.RatioMode.None);

                //if (status != StatusCodes.PICO_OK)
                //{
                //    Console.WriteLine("BlockDataHandler:ps2000aSetDataBuffer Channel {0} Status = 0x{1:X6}", (char)('A' + i), status);
                //}
            }

            /* Start it collecting, then wait for completion*/
            _ready = false;
            _callbackDelegate = BlockCallback;

            Imports.RunBlock(
                _handle,
                0,
                (int)sampleCount,
                timebase,
                _oversample,
                out int timeIndisposed,
                0,
                _callbackDelegate,
                IntPtr.Zero);

            //Wait for device
            while (!_ready)
            {
                Thread.Sleep(100);
            }

            if (_ready)
            {
                Imports.GetValues(_handle, 0, ref sampleCount, 1, Imports.DownSamplingMode.None, 0, out short overflow);

                for (int i = offset; i < offset + sampleCount; i++)
                {
                    for (int ch = 0; ch < _channelCount; ch++)
                    {
                        if (_channelSettings[ch].enabled)
                        {
                            voltageData[ch][i] = Adc_to_mv(maxPinned[ch].Target[i], (int)_channelSettings[(int)(Imports.Channel.ChannelA + ch)].range);
                        }
                    }
                }
            }

            Imports.Stop(_handle);

            foreach (PinnedArray<short> p in minPinned)
            {
                p?.Dispose();
            }

            foreach (PinnedArray<short> p in maxPinned)
            {
                p?.Dispose();
            }
            return voltageData;
        }

        private void SetSignalGenerator(bool ON, uint pkToPk, uint frequency)
        {
            //uint pkToPk = 1000000; // +/- 500 mV
            uint waveformSize = 0;
            Imports.ExtraOperations operation = Imports.ExtraOperations.PS2000A_ES_OFF;
            int offset = 0;
            //uint frequency = 150000;

            // Find the maximum AWG buffer size
            Imports.SigGenArbitraryMinMaxValues(_handle, out short minArbitraryWaveformValue, out short maxArbitraryWaveformValue, out uint minArbitraryWaveformSize, out uint maxArbitraryWaveformSize);

            short[] arbitraryWaveform = new short[maxArbitraryWaveformSize];

            Imports.WaveType waveform = Imports.WaveType.PS2000A_SINE;

            if (!ON)  // If we're going to 'turn off' the sig gen
            {
                waveform = Imports.WaveType.PS2000A_DC_VOLTAGE;
                pkToPk = 0;				// 0V
                waveformSize = 0;
                operation = Imports.ExtraOperations.PS2000A_ES_OFF;
            }

            if (waveformSize > 0)
            {
                // Find phase from the frequency
                Imports.SigGenFrequencyToPhase(_handle, frequency, Imports.IndexMode.PS2000A_SINGLE, waveformSize, out uint phase);

                Status = Imports.SetSigGenArbitrary(_handle,
                                                    0,
                                                    pkToPk,
                                                    phase,
                                                    phase,
                                                    0,
                                                    0,
                                                    arbitraryWaveform,
                                                    (int)waveformSize,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0);

                //Console.WriteLine(status != StatusCodes.PICO_OK ? "SetSigGenArbitrary: Status Error 0x%x " : "", status);		// If status != PICO_OK, show the error
            }
            else
            {
                Status = Imports.SetSigGenBuiltIn(_handle, offset, pkToPk, (short)waveform, frequency, frequency, 0, 0, 0, operation, 1, 0, 0, 0, 0);
                //Console.WriteLine(status != StatusCodes.PICO_OK ? "SetSigGenBuiltIn: Status Error 0x%x " : "", status);		// If status != PICO_OK, show the error
            }
        }

        private void GetDeviceInfo()
        {
            string[] description = {
                "Driver Version    ",
                "USB Version       ",
                "Hardware Version  ",
                "Variant Info      ",
                "Serial            ",
                "Cal Date          ",
                "Kernel Ver        ",
                "Digital Hardware  ",
                "Analogue Hardware "
            };

            StringBuilder line = new(80);

            // Default settings
            _digitalPorts = 0;

            if (_handle >= 0)
            {
                for (int i = 0; i < description.Length; i++)
                {
                    Imports.GetUnitInfo(_handle, line, 80, out short requiredSize, i);

                    Console.WriteLine("{0}: {1}", description[i], line);
                }

                // Find max ADC count
                Imports.MaximumValue(_handle, out _maxValue);
            }
        }

        /****************************************************************************
       *  SetTrigger
       *  this function sets all the required trigger parameters, and calls the
       *  triggering functions
       ****************************************************************************/

        private uint SetTrigger(Imports.TriggerChannelProperties[] channelProperties,
                        short nChannelProperties,
                        Imports.TriggerConditions[] triggerConditions,
                        short nTriggerConditions,
                        Imports.ThresholdDirection[] directions,
                        Pwq pwq,
                        uint delay,
                        short auxOutputEnabled,
                        int autoTriggerMs,
                        Imports.DigitalChannelDirections[] digitalDirections,
                        short nDigitalDirections)
        {
            if ((Status = Imports.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties, auxOutputEnabled, autoTriggerMs)) != StatusCodes.PICO_OK)
            {
                return Status;
            }

            if ((Status = Imports.SetTriggerChannelConditions(_handle, triggerConditions, nTriggerConditions)) != StatusCodes.PICO_OK)
            {
                return Status;
            }

            directions ??= new Imports.ThresholdDirection[]
            {
                Imports.ThresholdDirection.None,
                Imports.ThresholdDirection.None,
                Imports.ThresholdDirection.None,
                Imports.ThresholdDirection.None,
                Imports.ThresholdDirection.None,
                Imports.ThresholdDirection.None
            };

            if ((Status = Imports.SetTriggerChannelDirections(
                _handle,
                directions[(int)Imports.Channel.ChannelA],
                directions[(int)Imports.Channel.ChannelB],
                directions[(int)Imports.Channel.ChannelC],
                directions[(int)Imports.Channel.ChannelD],
                directions[(int)Imports.Channel.External],
                directions[(int)Imports.Channel.Aux])) != StatusCodes.PICO_OK)
            {
                return Status;
            }

            if ((Status = Imports.SetTriggerDelay(_handle, delay)) != StatusCodes.PICO_OK)
            {
                return Status;
            }

            if (pwq == null) pwq = new Pwq(null, 0, Imports.ThresholdDirection.None, 0, 0, Imports.PulseWidthType.None);

            Status = Imports.SetPulseWidthQualifier(
                _handle, pwq.conditions,
                pwq.nConditions, pwq.direction,
                pwq.lower, pwq.upper, pwq.type);

            if (_digitalPorts > 0 && (Status = Imports.SetTriggerDigitalPort(_handle, digitalDirections, nDigitalDirections)) != StatusCodes.PICO_OK)
            {
                return Status;
            }

            return Status;
        }

        public bool CheckVoltage(double minimum, double maximum, string valueName, string channel)
        {
            StartTest(valueName);
            double[] val = ReadVoltage();
            switch (channel)
            {
                case "A":
                    Measured = (float)val[0] / 1000;
                    break;

                case "B":
                    Measured = (float)val[1] / 1000;
                    break;

                default:
                    _IUserDialogService.ShowError("Wrong channel", "Voltage measurement");
                    break;
            }
            IsPassed = Measured >= minimum && Measured <= maximum;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) CheckVoltage(minimum, maximum, valueName, channel);
            return IsPassed;
        }
    }
}