using System.Security.Cryptography;
using System.Text;

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

    public Block(int index, string previousHash, string data)
    {
        Index = index;
        PreviousHash = previousHash;
        Data = data;
        Timestamp = DateTime.UtcNow;
        Hash = CalculateHash();
    }
    
    public bool VerifySignatures(RSA publicKey)
    {
        var hash = Encoding.UTF8.GetBytes(Hash);
        var hashSignatureBytes = Convert.FromBase64String(HashSignature);
        
        var data = Encoding.UTF8.GetBytes(Data);
        var dataSignatureBytes = Convert.FromBase64String(DataSignature);
        
        return publicKey.VerifyData(hash, hashSignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1) && 
               publicKey.VerifyData(data, dataSignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public string CalculateHash()
    {
        using var sha256 = SHA256.Create();
        var input = $"{Index}{PreviousHash}{Timestamp}{Data}";
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    public void SignBlock(RSA privateKey)
    {
        var hashSignatureBytes = Encoding.UTF8.GetBytes(Hash);
        var dataSignatureBytes =  Encoding.UTF8.GetBytes(Data);
        var hashSignature = privateKey.SignData(hashSignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var dataSignature = privateKey.SignData(dataSignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        HashSignature = Convert.ToBase64String(hashSignature);
        DataSignature = Convert.ToBase64String(dataSignature);
    }
    
    public override string ToString()
    {
        return $"Block Index: {Index}\n" +
               $"Previous Hash: {PreviousHash}\n" +
               $"Data: {Data}\n" +
               $"Timestamp: {Timestamp}\n" +
               $"Hash: {Hash}\n" +
               $"Hash Signature: {HashSignature}\n" +
               $"Data Signature: {DataSignature}";
    }
}