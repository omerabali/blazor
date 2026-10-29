using System.ComponentModel.DataAnnotations;

namespace KuaforRandevu.Models;

public sealed class CustomerInfo
{
    private const string EmailError = "Geçerli bir Gmail adresi girin (ör. ad.soyad@gmail.com).";
    private const string PhoneError = "Telefon 5 ile başlamalı ve 10 rakamdan oluşmalı (ör. 5321234567).";

    [Required(ErrorMessage = "Ad soyad boş bırakılamaz.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "Ad soyad 3 ile 80 karakter arasında olmalı.")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "E-posta boş bırakılamaz.")]
    [EmailAddress(ErrorMessage = EmailError)]
    [RegularExpression(@"(?i)^(?!.*\.\.)[a-z0-9](?:[a-z0-9._+-]*[a-z0-9])?@gmail\.com$", ErrorMessage = EmailError)]
    [StringLength(120, ErrorMessage = "E-posta en fazla 120 karakter olabilir.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Telefon boş bırakılamaz.")]
    [Phone(ErrorMessage = PhoneError)]
    [StringLength(10, MinimumLength = 10, ErrorMessage = PhoneError)]
    [RegularExpression(@"^5\d{9}$", ErrorMessage = PhoneError)]
    public string Phone { get; set; } = "";
}
