using System.Security.Cryptography;
using Tsisa.Blockchain.Persistence;

using var privateRsa = RSA.Create();
using var publicRsa = RSA.Create();

GetSigns(privateRsa, publicRsa);

var context = new BlockchainContext();
var blockchain = new Blockchain(context);

var genesisBlock = new Block(0, "0", "Genesis Block");
blockchain.AddBlock(genesisBlock, privateRsa);

var secondBlock = new Block(1, genesisBlock.Hash, "Second Block");
blockchain.AddBlock(secondBlock, privateRsa);

var block = blockchain.GetBlock(publicRsa, 1);
Console.WriteLine($"Block readonly: {block}");

var isValid = blockchain.ValidateBlockchain(publicRsa);
Console.WriteLine($"Blockchain valid: {isValid}");


static void GetSigns(RSA privateRsa, RSA publicRsa){
    using var rsa = RSA.Create(2048);

    var privateKey = rsa.ExportRSAPrivateKey();
    var publicKey = rsa.ExportRSAPublicKey();
    
    privateRsa.ImportRSAPrivateKey(privateKey, out _);
    
    publicRsa.ImportRSAPublicKey(publicKey, out _);
}