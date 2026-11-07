namespace KuaforRandevu.Models;

public sealed class Employee
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Title { get; set; } = "";
    public string Image { get; set; } = "";
    public string Bio { get; set; } = "";
    public int Years { get; set; }
    public List<string> ServiceIds { get; set; } = [];
}
