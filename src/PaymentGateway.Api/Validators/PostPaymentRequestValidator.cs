using System.Globalization;

using FluentValidation;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators
{
    public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
    {
        private static readonly HashSet<string> SupportedCurrencies = new(StringComparer.OrdinalIgnoreCase)
        {
            "EUR",
            "GBP",
            "USD",
        };

        public PostPaymentRequestValidator()
        {
            RuleFor(x => x.CardNumber)
                        .NotEmpty()
                        .WithMessage("Card number is required.")
                        .Length(14, 19)
                        .WithMessage("Card number must be between 14 and 19 characters.")
                        .Matches(@"^\d+$")
                        .WithMessage("Card number must contain only numeric characters.");

            RuleFor(x => x.ExpiryDate)
             .NotEmpty()
             .Must(HaveValidExpiryDate)
             .WithMessage("Expiry date must be a valid future date in MM/yyyy format.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.")
                .Length(3)
                .WithMessage("Currency must be exactly 3 characters.")
                .Must(currency => SupportedCurrencies.Contains(currency))
                .WithMessage("Unsupported currency.");

            RuleFor(x => x.Amount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Amount is required.")
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Cvv)
                .NotEmpty()
                .WithMessage("CVV is required.")
                .Length(3, 4)
                .WithMessage("CVV must be between 3 and 4 characters.")
                .Matches(@"^\d+$")
                .WithMessage("CVV must contain only numeric characters.");
        }

        private static bool HaveValidExpiryDate(string? value)
        {
            if (!DateTime.TryParseExact(
                                   value,
                                   "MM/yyyy",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None,
                                   out var expiry))
            {
                return false;
            }

            var current = DateTime.UtcNow;

            return expiry.Year > current.Year ||
                   (expiry.Year == current.Year && expiry.Month >= current.Month);
        }
    }
}
