using NAudio.CoreAudioApi;

namespace AudioController
{
    public static class Extensions
    {
        public static string GetCustomName(this MMDevice device)
        {
            return $"[{device.DataFlow.ToString()}] {device.DeviceFriendlyName}";
        }
    }
}
