using System.Security.Cryptography;
using Tsisa.Blockchain.Persistence;
using Tsisa.Blockchain.Services;

public class Blockchain(BlockchainContext context, KeyService keyService)
{
    
    public async Task AddBlock(Block newBlock, RSA privateKey)
    {
        await newBlock.SignBlock(privateKey, keyService);
        
        if (context.Blocks.Any())
        {
            var lastBlock = context.Blocks.OrderByDescending(b => b.Index).First();
            
            if (lastBlock.Hash != newBlock.PreviousHash)
            {
                throw new InvalidOperationException("Invalid block: Previous hash does not match.");
            }
        }
        
        context.Blocks.Add(newBlock);
        await context.SaveChangesAsync();
    }
    
    public bool ValidateBlockchain(RSA publicKey, RSA publicArbiterKey)
    {
        var blocks = context.Blocks.OrderBy(b => b.Index).ToList();
        
        for (var i = 1; i < blocks.Count; i++)
        {
            var currentBlock = blocks[i];
            var previousBlock = blocks[i - 1];
            
            if (currentBlock.PreviousHash != previousBlock.Hash ||
                !currentBlock.VerifySignatures(publicKey, publicArbiterKey))
                return false;
        }

        return true;
    }
    
    public string? GetBlock(RSA publicKey, RSA publicArbiterKey, int blockIndex)
    {
        var block = context.Blocks.FirstOrDefault(x => x.Index == blockIndex);

        if (block is not null && block.VerifySignatures(publicKey, publicArbiterKey))
            return block.ToString();

        return null;
    }
}