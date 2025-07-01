using Reown.AppKit.Unity;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class MinimumReproducible : MonoBehaviour {
    private bool resumed;
    private bool isWalletConnected;
    private AppKitConfig _AppKitConfig;
    [SerializeField] private Button _connectBtn;

    private async void Awake() {
        _connectBtn.interactable = false;
        _AppKitConfig = new AppKitConfig(
                 projectId: "da8823ff41610d0ab13bc637415f96df",
                 metadata: new Metadata(
                     name: "Boxx",
                     description: "Boxx/Book Integration",
                     url: "https://meta-graffiti-wall.ona.social/",
                     iconUrl: BoxxConfig.BoxxIcon
                 )
         ) {
            includedWalletIds = new[] {
                WalletMapper.GetWalletId(SupportedWallets.okex), //Okex
                WalletMapper.GetWalletId(SupportedWallets.metamask),  //Metamask
                WalletMapper.GetWalletId(SupportedWallets.trust), //Trust
                WalletMapper.GetWalletId(SupportedWallets.tokenpocket)  //Token Pocket
            },
            supportedChains = new[] {
                ChainMapper.GetReownChain(1),
                ChainMapper.GetReownChain(56),
                ChainMapper.GetReownChain(1030)
            }
        };
        await Init();
        SetupEvents();
        _connectBtn.interactable = true;
    }
    void Start() {}

    private void SetupEvents() {
        AppKit.ModalController.OpenStateChanged += OnModalStateChanged;

        AppKit.AccountConnected -= OnAccountConnected;
        AppKit.AccountConnected += OnAccountConnected;

        AppKit.AccountDisconnected -= OnAccountDisconnected;
        AppKit.AccountDisconnected += OnAccountDisconnected;

        AppKit.AccountChanged -= OnAccountChanged;
        AppKit.AccountChanged += OnAccountChanged;
    }

    private void CleanupEvents() {
        AppKit.ModalController.OpenStateChanged -= OnModalStateChanged;

        AppKit.AccountConnected -= OnAccountConnected;
        AppKit.AccountDisconnected -= OnAccountDisconnected;
        AppKit.AccountChanged -= OnAccountChanged;
    }

    private void OnDestroy() {
        CleanupEvents();
    }

    // Update is called once per frame
    void Update() {

    }

    private async Task Init() {
        if (!AppKit.IsInitialized) {
            isWalletConnected = false;

            Debug.Log("[AppKit] Init AppKit...");  //Reference Dapp.cs
            try {
                await AppKit.InitializeAsync(_AppKitConfig);
                resumed = await AppKit.ConnectorController.TryResumeSessionAsync();
                Debug.Log($"[SDK-Log] AppKit.ConnectorController.TryResumeSessionAsync() returned: {resumed}");
            }
            catch (Exception e) {
                Debug.LogError($"[AppKit] Initialization failed: {e.Message}");
                Debug.LogError($"[DEBUG] Full exception:\n{e}");
            }
        }
        else {
            Debug.Log("[AppKit] AppKit already initialized!");
        }

        if (isActiveAndEnabled && !resumed)
            ConnectWallet();
    }

    public void ConnectWallet() {
        print("Connecting Wallet...");
        if (Application.isMobilePlatform)
            AppKit.OpenModal();
        else
            AppKit.OpenModal(ViewType.QrCode);
    }

    private async void OnModalStateChanged(object sender, ModalOpenStateChangedEventArgs ev) {
        // Sometimes its not unsubscribed? Just return if gameObject is alr destroyed
        if (!gameObject)
            return;
        print("ev.IsOpen: " + ev.IsOpen);
    }

    private void OnAccountConnected(object sender, Connector.AccountConnectedEventArgs ev) {
        Debug.Log("[SDK-Log] Appkit Event fired: AccountConnected");
    }

    private void OnAccountDisconnected(object sender, Connector.AccountDisconnectedEventArgs ev) {
        Debug.Log("[SDK-Log] Appkit Event fired: AccountDisconnected");
    }

    private void OnAccountChanged(object sender, Connector.AccountChangedEventArgs ev) {
        Debug.Log("[SDK-Log] Appkit Event fired: AccountChanged");
    }

}
