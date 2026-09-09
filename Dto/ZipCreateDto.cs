namespace MST_4G.Dtos;

public class ZipCreateDto
{
    public string ZipNo { get; set; } = string.Empty;
    public string? ZipName { get; set; } = string.Empty;
    public DateTime EffDateFrom { get; set; }
    public DateTime EffDateTo { get; set; }
    public int CountyId { get; set; }
    public int ZoId { get; set; }
    public int DoId { get; set; }
}