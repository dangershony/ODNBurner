using System;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.DataEncoders;

namespace Obsidian.Burner
{
	public class NetworkSpec
	{
		static Network _mainnet;

		// only need for the Base58 addresses in Blockexplorer,
		// the rest is not even accurate for Obsidian!
		public static Network ObsidianMain()
		{
			if (_mainnet != null)
				return _mainnet;

			NetworkConfig.UseSingleNetwork = true; // fix

			Block.BlockSignature = true; // ?
			Transaction.TimeStamp = true; // ?


			NetworkBuilder builder = new NetworkBuilder();


			Block mainGenesisBlock = CreateGenesisBlock(nTime: 1503532800,
				nNonce: 36151509, nBits: 0x1e0fffff, nVersion: 1, genesisReward: Money.Zero);
			var odnMainConsensus = new Consensus
			{
				SubsidyHalvingInterval = 210000,
				MajorityEnforceBlockUpgrade = 750,
				MajorityRejectBlockOutdated = 950,
				MajorityWindow = 1000,
				BIP34Hash = null,
				PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
				PowTargetTimespan = TimeSpan.FromSeconds(14 * 24 * 60 * 60), // two weeks, 20160 minutes
				PowTargetSpacing = TimeSpan.FromSeconds(10 * 60), // 10 minutes
				PowAllowMinDifficultyBlocks = false,
				PowNoRetargeting = false,
				RuleChangeActivationThreshold = 1916, // 95% of 2016
				MinerConfirmationWindow = 2016, // nPowTargetTimespan / nPowTargetSpacing
				CoinbaseMaturity = 100,
				HashGenesisBlock = mainGenesisBlock.GetHash(),
				GetPoWHash = null,
				LitecoinWorkCalculation = false,
				// PoS
				LastPOWBlock = 12500,
				ProofOfStakeLimit = new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
				ProofOfStakeLimitV2 = new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false))
			};

			odnMainConsensus.BIP34Hash = odnMainConsensus.HashGenesisBlock;
			odnMainConsensus.BuriedDeployments[BuriedDeployments.BIP34] = 227931;
			odnMainConsensus.BuriedDeployments[BuriedDeployments.BIP65] = 388381;
			odnMainConsensus.BuriedDeployments[BuriedDeployments.BIP66] = 363725;
			odnMainConsensus.BIP9Deployments[BIP9Deployments.TestDummy] = new BIP9DeploymentsParameters(28, 1199145601, 1230767999);
			odnMainConsensus.BIP9Deployments[BIP9Deployments.CSV] = new BIP9DeploymentsParameters(0, 1462060800, 1493596800);
			odnMainConsensus.BIP9Deployments[BIP9Deployments.Segwit] = new BIP9DeploymentsParameters(1, 0, 0);

			// Start copied from StratisMain
			var pchMessageStart = new byte[4];
			pchMessageStart[0] = 0x70;
			pchMessageStart[1] = 0x35;
			pchMessageStart[2] = 0x22;
			pchMessageStart[3] = 0x05;
			var magic = BitConverter.ToUInt32(pchMessageStart, 0); //0x5223570; 
			// End copied from StratisMain

			_mainnet = builder.SetConsensus(odnMainConsensus)

				.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { (75) }) // ODN / X
				.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { (125) })
				.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { (75 + 128) }) // ODN
				.SetBase58Bytes(Base58Type.ENCRYPTED_SECRET_KEY_NO_EC, new byte[] { 0x01, 0x42 })
				.SetBase58Bytes(Base58Type.ENCRYPTED_SECRET_KEY_EC, new byte[] { 0x01, 0x43 })
				.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { (0x04), (0x88), (0xB2), (0x1E) })
				.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { (0x04), (0x88), (0xAD), (0xE4) })
				.SetBase58Bytes(Base58Type.PASSPHRASE_CODE, new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 })
				.SetBase58Bytes(Base58Type.CONFIRMATION_CODE, new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A })
				.SetBase58Bytes(Base58Type.STEALTH_ADDRESS, new byte[] { 0x2a })
				.SetBase58Bytes(Base58Type.ASSET_ID, new byte[] { 23 })
				.SetBase58Bytes(Base58Type.COLORED_ADDRESS, new byte[] { 0x13 })
				.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, "bc")
				.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, "bc") // bc?

				.SetMagic(magic)
				.SetPort(0)
				.SetRPCPort(0)
				.SetName("odn-main")
				.AddAlias("odn-mainnet")
				.AddAlias("obsidian-main")
				.AddAlias("obsidian-mainnet")
				.AddDNSSeeds(new DNSSeedData[]
				{
					//new DNSSeedData("obsidianseednode1.westeurope.cloudapp.azure.com", "obsidianseednode1.westeurope.cloudapp.azure.com")
				})
				.SetGenesis(mainGenesisBlock)
				.BuildAndRegister();
			return _mainnet;
		}

		private static Block CreateGenesisBlock(uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
		{
			string pszTimestamp = "https://en.wikipedia.org/w/index.php?title=Brave_New_World&id=796766418";
			return CreateGenesisBlock(pszTimestamp, nTime, nNonce, nBits, nVersion, genesisReward);
		}

		private static Block CreateGenesisBlock(string pszTimestamp, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
		{
			Transaction txNew = new Transaction();
			txNew.Version = 1;
			txNew.Time = nTime;
			txNew.AddInput(new TxIn()
			{
				ScriptSig = new Script(Op.GetPushOp(0), new Op()
				{
					Code = (OpcodeType)0x1,
					PushData = new[] { (byte)42 }
				}, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
			});
			txNew.AddOutput(new TxOut()
			{
				Value = genesisReward,
			});
			Block genesis = new Block();
			genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
			genesis.Header.Bits = nBits;
			genesis.Header.Nonce = nNonce;
			genesis.Header.Version = nVersion;
			genesis.Transactions.Add(txNew);
			genesis.Header.HashPrevBlock = uint256.Zero;
			genesis.UpdateMerkleRoot();
			return genesis;
		}

	}
}
