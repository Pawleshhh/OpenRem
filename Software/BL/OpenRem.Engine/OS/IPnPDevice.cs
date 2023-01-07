namespace OpenRem.Engine.OS;

internal interface IPnPDevice
{
    IEnumerable<string> GetDevices();
}