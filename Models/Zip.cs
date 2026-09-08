using System.ComponentModel.DataAnnotations.Schema;

namespace MST_4G.Models;

public class Zip
{
    [Column("zip_id")]
    public int ZipId { get; set; }

    [Column("zip_no")]
    public string ZipNo { get; set; } = string.Empty;

    [Column("zip_name")]
    public string ZipName { get; set; } = string.Empty;

    [Column("eff_date_from")]
    public DateTime EffDateFrom { get; set; }

    [Column("eff_date_to")]
    public DateTime EffDateTo { get; set; }

    [Column("county_id")]
    public int CountyId { get; set; }

    [Column("zo_id")]
    public int ZoId { get; set; }

    [Column("do_id")]
    public int DoId { get; set; }

    public County? County { get; set; }
    public Zo? Zo { get; set; }
    public DistrictOffice? DistrictOffice { get; set; }
}