using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reown.AppKit.Unity;
using Reown.Core.Controllers;

public class ChainMapper {

    private static readonly Dictionary<int, SupportedChains> ChainMap = new() {
        { 1, SupportedChains.ETH },
        { 1030, SupportedChains.CFX },
        { 56, SupportedChains.BSC }
    };

    private static readonly Dictionary<SupportedChains, int> ChainIdMap = new() {
        { SupportedChains.ETH, 1 },
        { SupportedChains.CFX, 1030 },
        { SupportedChains.BSC, 56 }
    };

    private static readonly Dictionary<BookChainType, SupportedChains> BookChainMap = new() {
        { BookChainType.ethereum, SupportedChains.ETH },
        { BookChainType.cfx, SupportedChains.CFX },
        { BookChainType.bnb, SupportedChains.BSC }
    };

    private static readonly Dictionary<int, BookChainType> ChainBookMap = new() {
        { 1, BookChainType.ethereum },
        { 1030, BookChainType.cfx },
        { 56, BookChainType.bnb }
    };

    //private static readonly Dictionary<int, string> RpcUrlMap = new() {
    //    { 1, $"https://holesky.infura.io/v3/{EnvironmentUtil.GetInfuraRpcPrivateKey()}" },
    //    { 1030, "https://evm.confluxrpc.com/" },
    //    { 56, $"https://rpc.ankr.com/bsc/{EnvironmentUtil.GetAnkrApiKey()}" }
    //};

    private static readonly Dictionary<int, Chain> ReownChainMap = new() {
        { 1, ChainConstants.Chains.Ethereum },
        { 1030, new Chain(
            ChainConstants.Namespaces.Evm,
            chainReference: "1030",
            name: "Conflux eSpace",
            nativeCurrency: new Currency("Conflux", "CFX", 18),
            blockExplorer: new BlockExplorer("ConfluxScan", "https://evm.confluxscan.org/"),
			rpcUrl: "https://evm.confluxrpc.com",
            isTestnet: false,
            imageUrl: "https://game-1330837229.cos.ap-hongkong.myqcloud.com/icon/reown-espace.png"
        )},
        { 56, new Chain(
            ChainConstants.Namespaces.Evm,
            chainReference: "56",
            name: "Binance Smart Chain",
            nativeCurrency: new Currency("Binance Coin", "BNB", 18),
            blockExplorer: new BlockExplorer("BscScan", "https://bscscan.com"),
			rpcUrl: "https://bsc-dataseed.binance.org", // Internal JSON-RPC issue even with Ankr
            isTestnet: false,
            imageUrl: "https://game-1330837229.cos.ap-hongkong.myqcloud.com/icon/reown-bnb.png"
        )}
    };

    private static readonly Dictionary<int, int> _uaiMap = new() { // https://github.com/trustwallet/wallet-core/blob/master/docs/registry.md
        { 1, 60 },
        { 1030, 1030 },
        { 56, 20000714 }
    };

    public static SupportedChains GetInternalChainEnum(int chainId) =>
        ChainMap.TryGetValue(chainId, out var chain) ? chain : throw new KeyNotFoundException($"Unsupported chain ID: {chainId}");
    public static SupportedChains GetInternalChainEnum(BookChainType bookChainType) =>
        BookChainMap.TryGetValue(bookChainType, out var chain) ? chain : throw new KeyNotFoundException($"Unsupported Book chain type: {bookChainType}");

    public static int GetChainId(SupportedChains chainName) =>
        ChainIdMap.TryGetValue(chainName, out var chainId) ? chainId : throw new KeyNotFoundException($"Unsupported chain: {chainName}");

    public static int GetChainId(BookChainType bookChainType) =>
        ChainIdMap.TryGetValue(BookChainMap[bookChainType], out var chainId) ? chainId : throw new KeyNotFoundException($"Unsupported Book chain type: {bookChainType}");

    public static BookChainType GetBookChainType(int chainId) =>
        ChainBookMap.TryGetValue(chainId, out var bookChainType) ? bookChainType : throw new KeyNotFoundException($"Unsupported chain ID: {chainId}");

    //public static string GetRpcUrl(int chainId) =>
    //    RpcUrlMap.TryGetValue(chainId, out var rpcUrl) ? rpcUrl : throw new KeyNotFoundException($"Unsupported chain ID: {chainId}");

    public static Chain GetReownChain(int chainId) =>
        ReownChainMap.TryGetValue(chainId, out var chain) ? chain : throw new KeyNotFoundException($"Unsupported chain ID: {chainId}");

    public static int GetUai(int chainId) =>
        _uaiMap.TryGetValue(chainId, out var uai) ? uai : throw new KeyNotFoundException($"Unsupported chain ID: {chainId}");
}
