namespace SwitchBotMqttApp.Models.SwitchBotApi
{
    public class StatusCode
    {
        /// <summary>
        /// Success status code.
        /// </summary>
        public const int Success = 100;

        /// <summary>
        /// Device type error status code.
        /// </summary>
        public const int DeviceTypeError = 151;

        /// <summary>
        /// Device not found status code.
        /// </summary>
        public const int DeviceNotFound = 152;

        /// <summary>
        /// Unsupported command status code.
        /// </summary>
        public const int UnsupportedCommand = 160;

        /// <summary>
        /// Device offline status code.
        /// </summary>
        public const int DeviceOffline = 161;

        /// <summary>
        /// Hub device offline status code.
        /// </summary>
        public const int HubDeviceOffline = 171;

        /// <summary>
        /// System error status code.
        /// </summary>
        public const int SystemError = 190;
    }
}
