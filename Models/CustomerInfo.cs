using System.ComponentModel.DataAnnotations;

namespace KuaforRandevu.Models;

public sealed class CustomerInfo
{
    private const string PhoneError = "5 ile başlayan 10 rakam girin.";

    [Required(ErrorMessage = "Ad soyad boş bırakılamaz.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "Ad soyad 3 ile 80 karakter arasında olmalı.")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "E-posta boş bırakılamaz.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(120, ErrorMessage = "E-posta en fazla 120 karakter olabilir.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Telefon boş bırakılamaz.")]
    [Phone(ErrorMessage = PhoneError)]
    [StringLength(10, MinimumLength = 10, ErrorMessage = PhoneError)]
    [RegularExpression(@"^5[0-9]{9}$", ErrorMessage = PhoneError)]
    public string Phone { get; set; } = "";
}
