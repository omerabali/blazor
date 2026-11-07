namespace KuaforRandevu.Models;

public sealed class SalonProfile
{
    public string Name { get; set; } = "Atölye";
    public string Tagline { get; set; } = "";
    public string Story { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Instagram { get; set; } = "";
    public string Hours { get; set; } = "";
    public string Founded { get; set; } = "";
    public List<SalonFaq> Faqs { get; set; } = [];
}

public sealed class SalonFaq
{
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
}

public sealed class Testimonial
{
    public string Name { get; set; } = "";
    public string Service { get; set; } = "";
    public string Text { get; set; } = "";
    public int Rating { get; set; }
}

public sealed class GalleryShot
{
    public string Image { get; set; } = "";
    public string Alt { get; set; } = "";
    public string Caption { get; set; } = "";
}
