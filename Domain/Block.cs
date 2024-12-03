using System.Security.Cryptography;
using System.Text;
using Tsisa.Blockchain.Services;

public class Block
{
    public int Id { get; set; }
    public int Index { get; set; }
    public string PreviousHash { get; set; }
    public string Data { get; set; }
    public DateTime Timestamp { get; set; }
    public string Hash { get; set; }
    public string HashSignature { get; set; }
    public string DataSignature { get; set; }
    public string ArbiterTimestampString { get; set; }
    public string ArbiterSignature { get; set; }

    public Block(int index, string previousHash, string data)
    {
        Index = index;
        PreviousHash = previousHash;
        Data = data;
        Timestamp = DateTime.UtcNow;
        Hash = CalculateHash();
    }

    public async Task SignBlock(RSA privateKey, KeyService timestampService)
    {
        var hashBytes = Encoding.UTF8.GetBytes(Hash);
        var dataBytes = Encoding.UTF8.GetBytes(Data);

        var hashSignature = privateKey.SignData(hashBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var dataSignature = privateKey.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        HashSignature = Convert.ToHexString(hashSignature);
        DataSignature = Convert.ToHexString(dataSignature);
        
        var timestampResponse = await timestampService.GetPrivateArbiterKey(Convert.ToHexString(hashBytes));

        ArbiterTimestampString = timestampResponse.TimeStampToken.Ts;
        ArbiterSignature = timestampResponse.TimeStampToken.Signature;
    }
    
    public string CalculateHash()
    {
        using var sha256 = SHA256.Create();
        var input = $"{Index}{PreviousHash}{Timestamp}{Data}";
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    public bool VerifySignatures(RSA publicKey, RSA arbiterPublicKey)
    {
        var hashBytes = Encoding.UTF8.GetBytes(Hash);
        var hashSignatureBytes = Convert.FromHexString(HashSignature);

        var dataBytes = Encoding.UTF8.GetBytes(Data);
        var dataSignatureBytes = Convert.FromHexString(DataSignature);

        var arbiterInput = Encoding.UTF8.GetBytes(ArbiterTimestampString).Concat(Encoding.UTF8.GetBytes(Hash))
            .ToArray();
        var arbiterSignatureBytes = Convert.FromHexString(ArbiterSignature);

        var areSignaturesValid =
            publicKey.VerifyData(hashBytes, hashSignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1) &&
            publicKey.VerifyData(dataBytes, dataSignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1) &&
            arbiterPublicKey.VerifyData(arbiterInput, arbiterSignatureBytes, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

        return areSignaturesValid;
    }
    
    public override string ToString()
    {
        return $"Block Index: {Index}\n" +
               $"Previous Hash: {PreviousHash}\n" +
               $"Data: {Data}\n" +
               $"Timestamp: {Timestamp}\n" +
               $"Hash: {Hash}\n" +
               $"Hash Signature: {HashSignature}\n" +
               $"Data Signature: {DataSignature}\n" +
               $"Hash Signature: {HashSignature}\n" +
               $"Arbiter Hash Signature: {ArbiterSignature}";
    }
}
