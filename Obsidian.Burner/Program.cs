using System;
using System.Linq;
using NBitcoin;

namespace Obsidian.Burner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Lets burn!");
	        var network = NetworkSpec.ObsidianMain();
	        var bitcoinPrivateKey = new  BitcoinSecret("cSZjE4aJNPpBtU6xvJ6J4iBzDgTmzTjbq8w2kqnYvAprBCyTsG4x");
	        var address = bitcoinPrivateKey.GetAddress();
			// todo: create the raw transaction

		}
    }
}
