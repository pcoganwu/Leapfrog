namespace Leapfrog.Core.Enums.LoRaWANInterfaceEnums
{
    public enum SerialCommandType
    {
        SerialButtonPush = 0x08,        // Simulate a button push
        SerialConfigGet = 0x40,        // Retrieve configuration from a device
        SerialConfigSet = 0x41,		// Assign configuration to a device
    }
}
