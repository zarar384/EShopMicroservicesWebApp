namespace Ordering.Domain.ValueObjects
{
    public record Payment
    {
        public string? CardName { get; } = default!;
        public string CardNumber { get; } = default!;
        public string Expiration { get; } = default!;
        public string CVV { get; } = default!;
        public int PaymentMethod { get; } = default!;
        public string CardName1 { get; }
        public string CardNumber1 { get; }
        public string Expiration1 { get; }
        public string Cvv { get; }
        public string PaymentMethod1 { get; }

        protected Payment() { }

        public Payment(string cardName, string cardNumber, string expiration, string cvv, string paymentMethod)
        {
            CardName1 = cardName;
            CardNumber1 = cardNumber;
            Expiration1 = expiration;
            Cvv = cvv;
            PaymentMethod1 = paymentMethod;
        }

        public static Payment Of(string cardName, string cardNumber, string expiration, string cvv, string paymentMethod)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName);
            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(cvv);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cvv.Length, 3);

            return new Payment(cardName, cardNumber, expiration, cvv, paymentMethod);
        }
    }
}
