using System.Net.Http.Json;
using KuaforRandevu.Models;

namespace KuaforRandevu.Services;

public sealed class SalonContentService
{
    private readonly HttpClient _http;
    private Task? _load;
    private SalonProfile _salon = new();
    private IReadOnlyList<Testimonial> _testimonials = [];
    private IReadOnlyList<GalleryShot> _gallery = [];

    public SalonContentService(HttpClient http)
    {
        _http = http;
    }

    public async Task<SalonProfile> GetSalonAsync()
    {
        await EnsureLoadedAsync();
        return _salon;
    }

    public async Task<IReadOnlyList<Testimonial>> GetTestimonialsAsync()
    {
        await EnsureLoadedAsync();
        return _testimonials;
    }

    public async Task<IReadOnlyList<GalleryShot>> GetGalleryAsync()
    {
        await EnsureLoadedAsync();
        return _gallery;
    }

    private Task EnsureLoadedAsync() => _load ??= LoadAsync();

    private async Task LoadAsync()
    {
        var salonTask = _http.GetFromJsonAsync<SalonProfile>("data/salon.json");
        var quotesTask = _http.GetFromJsonAsync<List<Testimonial>>("data/testimonials.json");
        var galleryTask = _http.GetFromJsonAsync<List<GalleryShot>>("data/gallery.json");
        await Task.WhenAll(salonTask, quotesTask, galleryTask);

        _salon = await salonTask ?? new SalonProfile();
        _testimonials = await quotesTask ?? [];
        _gallery = await galleryTask ?? [];
    }
}
