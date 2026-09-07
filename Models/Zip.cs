namespace MST_4G.Models;
public class Zip
{
    public string ZipNo { get; set; } = string.Empty;
    public string ZipName { get; set; } = string.Empty;
    public DateTime EffDateFrom { get; set; }
    public DateTime? EffDateTo { get; set; }

    public string CountyNo { get; set; } = string.Empty;
    public string ZoNo { get; set; } = string.Empty;
    public string DoNo { get; set; } = string.Empty;

    public County? County { get; set; }
    public Zo? Zo { get; set; }
    public DistrictOffice? DistrictOffice { get; set; }
}