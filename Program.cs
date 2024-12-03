using System.Security.Cryptography;
using Tsisa.Blockchain.Persistence;
using Tsisa.Blockchain.Services;

var context = new BlockchainContext();
var keyService = new KeyService();
var blockchain = new Blockchain(context, keyService);

using var privateRsa = RSA.Create();
using var publicRsa = RSA.Create();
KeyService.GetBlockOwnerKeyPair(privateRsa, publicRsa);
using var publicArbiterRsa = await keyService.GetPublicArbiterKey();

var genesisBlock = new Block(0, "0", "Genesis Block");
await blockchain.AddBlock(genesisBlock, privateRsa);

var secondBlock = new Block(1, genesisBlock.Hash, "Second Block");
await blockchain.AddBlock(secondBlock, privateRsa);

var block = blockchain.GetBlock(publicRsa, publicArbiterRsa, 1);
Console.WriteLine($"Block readonly: {block}");

var isValid = blockchain.ValidateBlockchain(publicRsa, publicArbiterRsa);
Console.WriteLine($"Blockchain valid: {isValid}");



