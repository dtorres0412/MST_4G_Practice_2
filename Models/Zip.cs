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

    public ICollection<ZipJunction> ZipJunction { get; set; } = new List<ZipJunction>();
}