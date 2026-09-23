namespace MST_4G.Dtos;

public class ZipProcessResumeDto
{
    public ZipReadDto? OriItem { get; set; }
    public ZipCreateDto NewItem { get; set; } = null!;
    public List<ZipReadDto> VitaeList {get; set;} = new();
}