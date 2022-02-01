using Domain.Enums;

namespace Domain.Entities;

public class VideoFile : MediaFile
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public ulong Duration { get; set; }
    public BitRateMode BitRateMode { get; set; }
    public ulong BitRate { get; set; }
    public FrameRateMode FrameRateMode { get; set; }
    public double FrameRate { get; set; }
}