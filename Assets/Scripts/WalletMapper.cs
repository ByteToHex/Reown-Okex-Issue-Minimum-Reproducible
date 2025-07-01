using System.Collections.Generic;
using Reown.AppKit.Unity;
using Reown.AppKit.Unity.Model;

public class WalletMapper {
    //private static readonly Dictionary<SupportedWallets, Wallet> WalletMap = new() {};
    private static readonly Dictionary<SupportedWallets, string> WalletIdMap = new() {
        { SupportedWallets.okex, "971e689d0a5be527bac79629b4ee9b925e82208e5168b733496a09c0faed0709" },
        { SupportedWallets.metamask, "c57ca95b47569778a828d19178114f4db188b89b763c899ba0be274e97267d96" },
        { SupportedWallets.trust, "4622a2b2d6af1c9844944291e5e7351a6aa24cd7b23099efac1b2fd875da31a0" },
        { SupportedWallets.tokenpocket, "20459438007b75f4f4acb98bf29aa3b800550309646d375da5fd4aac6c2a2c66" }
    };

    public static string GetWalletId(SupportedWallets wallet) =>
        WalletIdMap.TryGetValue(wallet, out var id) ? id : throw new KeyNotFoundException($"Unsupported wallet: {wallet}");

    //public async static void DirectConnectWallet(SupportedWallets wallet) => await AppKit.ConnectAsync(); // Seems not included in this version of Reown; was intended for use with WalletPrompt (https://docs.reown.com/appkit/unity/core/actions)
}