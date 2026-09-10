using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MST_4G.Models;

[Table("zip_junction")]
[PrimaryKey(nameof(ZipNo), nameof(CountyNo), nameof(ZoNo), nameof(DoNo))]
public class ZipJunction
{
    public string ZipNo { get; set; } = string.Empty;
    public string CountyNo { get; set; } = string.Empty;
    public string ZoNo { get; set; } = string.Empty;
    public string DoNo { get; set; } = string.Empty;

    public Zip? Zip { get; set; }
    public County? County { get; set; }
    public Zo? Zo { get; set; }
    public DistrictOffice? DistrictOffice { get; set; }
}