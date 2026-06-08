namespace Directfn.Custody.ApiFramework.Authentication
{
    public sealed class SigningCertificateOptions
    {
        public string? StoreName { get; init; }

        public string? StoreLocation { get; init; }

        public string? Thumbprint { get; init; }
    }
}