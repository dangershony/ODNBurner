using System;
using System.Linq;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Obsidian.Burner
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Lets burn!");

			// a sample unspent:

			var unspent_address = "XHF95jgCxhhiWweUGU8PeVYeUN5HrifbKU";
			var unspent_scriptPubKey = "76a91440a9722d2891b7cea50fb6c97476d42e5023d5e988ac";
			var unspent_amount = 464638.27963184m;
			var unspent_confirmations = 106;


			var unspent_txid = "31d225ceaa9b2c35936f759f19012abef2dcf11e3b4206a26f79462c39314891";
			var unspent_vout = 0;
			decimal amountToBurn = 2m;

			var tx = new Transaction
			{
				Version = 1,
				Time = (uint)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
			};

			var input = new TxIn
			{
				PrevOut = new OutPoint(uint256.Parse(unspent_txid), unspent_vout),
				// bo unlocking script
			};
			tx.Inputs.Add(input);

			var output = new TxOut();
			output.Value = Money.Coins(amountToBurn);

			// locking script
			output.ScriptPubKey = new Script(
				new Op { Code = OpcodeType.OP_RETURN }, new Op { Code = (OpcodeType)0xf9 },
				Op.GetPushOp(Encoders.ASCII.DecodeData("Burn ODN")));
			tx.Outputs.Add(output);

			tx.Outputs.Add(new TxOut(Money.Coins(0.5m), BitcoinAddress.Create("XFPaXvPwFA4WpPm5feEJ9MkLsd57japSy2", NetworkSpec.ObsidianMain()).ScriptPubKey));

			string rawtransaction = tx.ToHex();
			Console.WriteLine(rawtransaction);

			var network = NetworkSpec.ObsidianMain();
		}
	}
}
