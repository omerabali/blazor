using System.ComponentModel.DataAnnotations;

namespace KuaforRandevu.Models;

public sealed class CustomerInfo
{
    private const string PhoneError = "5 ile başlayan 10 rakam girin.";

    [Required(ErrorMessage = "Ad soyad boş bırakılamaz.")]
    [StringLength(80, ErrorMessage = "Ad soyad en fazla 80 karakter olabilir.")]
    [TrimmedLength(3, 80, EmptyMessage = "Ad soyad boş bırakılamaz.", ErrorMessage = "Ad soyad 3 ile 80 karakter arasında olmalı.")]
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

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class TrimmedLengthAttribute : ValidationAttribute
{
    public int MinimumLength { get; }
    public int MaximumLength { get; }
    public string? EmptyMessage { get; set; }

    public TrimmedLengthAttribute(int minimumLength, int maximumLength)
    {
        MinimumLength = minimumLength;
        MaximumLength = maximumLength;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string text || text.Length == 0)
            return ValidationResult.Success;

        var trimmed = text.Trim();
        if (trimmed.Length == 0)
            return Fail(EmptyMessage ?? ErrorMessage, validationContext);

        if (trimmed.Length < MinimumLength || trimmed.Length > MaximumLength)
            return Fail(ErrorMessage, validationContext);

        return ValidationResult.Success;
    }

    private static ValidationResult Fail(string? message, ValidationContext validationContext)
    {
        var members = string.IsNullOrEmpty(validationContext.MemberName)
            ? null
            : new[] { validationContext.MemberName };
        return new ValidationResult(message, members);
    }
}
