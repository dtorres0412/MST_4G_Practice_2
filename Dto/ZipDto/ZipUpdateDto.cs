namespace MST_4G.Dtos;

public class ZipUpdateDto
{
    public string ZipNo { get; set; } = string.Empty;
    public string? ZipName { get; set; } = string.Empty;
    public DateTime EffDateFrom { get; set; }
    public DateTime EffDateTo { get; set; }
    public string CountyNo { get; set; } = string.Empty;
    public string ZoNo { get; set; } = string.Empty;
    public string DoNo { get; set; } = string.Empty;
}