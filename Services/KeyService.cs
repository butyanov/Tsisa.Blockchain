using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tsisa.Blockchain.Services;

public class KeyService
{
    private static readonly HttpClient _httpClient = new ();
    private readonly string _baseUri = "http://itislabs.ru/ts";

    public async Task<TimestampResponse> GetPrivateArbiterKey(string hashHex)
    {
        var response = await _httpClient.GetAsync($"{_baseUri}?digest={hashHex}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get timestamp: {response.StatusCode} {response.ReasonPhrase}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var timestampResponse = JsonSerializer.Deserialize<TimestampResponse>(json);

        if (timestampResponse is not { Status: 0 })
            throw new Exception($"Timestamp service error: {timestampResponse!.StatusString}");
        
        return timestampResponse;
    }
    
    public async Task<RSA> GetPublicArbiterKey()
    {
        var arbiterRsa = RSA.Create();
        
        var response = await _httpClient.GetStringAsync($"{_baseUri}/public");
        var arbiterPublicKey = Convert.FromHexString(response);
        
        arbiterRsa.ImportSubjectPublicKeyInfo(arbiterPublicKey, out _);
        return arbiterRsa;
    }
    
    public static void GetBlockOwnerKeyPair(RSA privateRsa, RSA publicRsa)
    {
        using var rsa = RSA.Create(2048);

        var privateKey = rsa.ExportRSAPrivateKey();
        var publicKey = rsa.ExportRSAPublicKey();

        privateRsa.ImportRSAPrivateKey(privateKey, out _);
        publicRsa.ImportRSAPublicKey(publicKey, out _);
    }

    public class TimestampResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        
        [JsonPropertyName("statusString")]
        public string StatusString { get; set; }
        [JsonPropertyName("timeStampToken")]
        public TimeStampTokenResponse TimeStampToken { get; set; }

        public class TimeStampTokenResponse
        {
            [JsonPropertyName("ts")]
            public string Ts { get; set; }
            
            [JsonPropertyName("signature")]
            public string Signature { get; set; }
        }
    }
}