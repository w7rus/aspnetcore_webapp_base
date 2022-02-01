using Domain.Enums;

namespace Domain.Entities;

public class AudioFile : MediaFile
{
    public ulong Duration { get; set; }
    public BitRateMode BitRateMode { get; set; }
    public ulong BitRate { get; set; }
    
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public int Year { get; set; }
    public string Comment { get; set; }
    public int Genre { get; set; }
    public int Part { get; set; }
    public int PartTotal { get; set; }
    public int Position { get; set; }
    public int PositionTotal { get; set; }
    public uint BPM { get; set; }
}