using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

namespace voteApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //The URL endpoint for the blockchain network.
            string url = "HTTP://localhost:8545";

            //The contract address.
            string address = "0x5d6f25344c660f506B5Ced585bCE527705315428";

            //The ABI for the contract.
            string ABI = @"[{'constant':true,'inputs':[],'name':'candidate1','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'candidate','type':'uint256'}],'name':'castVote','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'candidate2','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'voted','outputs':[{'name':'','type':'bool'}],'payable':false,'stateMutability':'view','type':'function'}]";

            //Creates the connecto to the network and gets an instance of the contract.
            Web3 web3 = new Web3(url);
            Contract voteContract = web3.Eth.GetContract(ABI, address);

            //Reads the vote count for Candidate 1 and Candidate 2
            Task<BigInteger> candidate1Function = voteContract.GetFunction("candidate1").CallAsync<BigInteger>();
            candidate1Function.Wait();
            int candidate1 = (int)candidate1Function.Result;
            Task<BigInteger> candidate2Function = voteContract.GetFunction("candidate2").CallAsync<BigInteger>();
            candidate2Function.Wait();
            int candidate2 = (int)candidate2Function.Result;
            Console.WriteLine("Candidate 1 votes: {0}", candidate1);
            Console.WriteLine("Candidate 2 votes: {0}", candidate2);

            //Prompts for the account address.
            Console.Write("Enter the address of your account: ");
            string accountAddress = Console.ReadLine();

            //Prompts for the users vote.
            int vote = 0;
            Console.Write("Press 1 to vote for candidate 1, Press 2 to vote for candidate 2: ");
            Int32.TryParse(Convert.ToChar(Console.Read()).ToString(), out vote);
            Console.WriteLine("You pressed {0}", vote);

            //Executes the vote on the contract.
            try
            {
                HexBigInteger gas = new HexBigInteger(new BigInteger(400000));
                HexBigInteger value = new HexBigInteger(new BigInteger(0));
                Task<string> castVoteFunction = voteContract.GetFunction("castVote").SendTransactionAsync(accountAddress, gas, value, vote);
                castVoteFunction.Wait();
                Console.WriteLine("Vote Cast!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
