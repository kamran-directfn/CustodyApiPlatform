namespace Directfn.Custody.ApiFramework.Authentication;

public sealed class TokenFingerprintResult
{
    public string Fingerprint { get; init; } = default!;

    public string FingerprintHash { get; init; } = default!;
}