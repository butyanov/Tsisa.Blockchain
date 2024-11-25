using System.Security.Cryptography;
using Tsisa.Blockchain.Persistence;

public class Blockchain(BlockchainContext context)
{
    public void AddBlock(Block newBlock, RSA privateKey)
    {
        newBlock.Hash = newBlock.CalculateHash();
        
        newBlock.SignBlock(privateKey);
        
        if (context.Blocks.Any())
        {
            var lastBlock = context.Blocks.OrderByDescending(b => b.Index).First();
            
            if (lastBlock.Hash != newBlock.PreviousHash)
            {
                throw new InvalidOperationException("Invalid block: Previous hash does not match.");
            }
        }
        
        context.Blocks.Add(newBlock);
        context.SaveChanges();
    }
    
    public bool ValidateBlockchain(RSA publicKey)
    {
        var blocks = context.Blocks.OrderBy(b => b.Index).ToList();
        
        for (var i = 1; i < blocks.Count; i++)
        {
            var currentBlock = blocks[i];
            var previousBlock = blocks[i - 1];
            
            if (currentBlock.PreviousHash != previousBlock.Hash ||
                !currentBlock.VerifySignatures(publicKey))
            {
                return false;
            }
        }

        return true;
    }
    
    public string? GetBlock(RSA publicKey, int blockIndex)
    {
        var block = context.Blocks.FirstOrDefault(x => x.Index == blockIndex);

        if (block is not null && block.VerifySignatures(publicKey))
            return block.ToString();

        return null;
    }
}