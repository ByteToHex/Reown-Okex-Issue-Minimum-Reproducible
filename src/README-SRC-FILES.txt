README-SRC-FILES

These are files used with Reown Appkit v1.0.1 for manual installation.

Specifically referred to are the following dependencies:

    "com.reown.appkit.unity": "file:./../src/Reown.AppKit.Unity",
    "com.reown.core": "file:./../src/Reown.Core",
    "com.reown.core.common": "file:./../src/Reown.Core.Common",
    "com.reown.core.crypto": "file:./../src/Reown.Core.Crypto",
    "com.reown.core.network": "file:./../src/Reown.Core.Network",
    "com.reown.core.storage": "file:./../src/Reown.Core.Storage",
    "com.reown.sign": "file:./../src/Reown.Sign",
    "com.reown.sign.nethereum": "file:./../src/Reown.Sign.Nethereum",
    "com.reown.sign.nethereum.unity": "file:./../src/Reown.Sign.Nethereum.Unity",
    "com.reown.sign.unity": "file:./../src/Reown.Sign.Unity",
    "com.reown.unity.dependencies": "file:./../src/Reown.Unity.Dependencies",
	
These correspond to the folders which can be found in commit `bc6d7880fdc1518542aa893e8e13278ba25d158f`

	Boxx\src\Reown.AppKit.Unity
	Boxx\src\Reown.Core
	Boxx\src\Reown.Core.Common
	Boxx\src\Reown.Core.Crypto
	Boxx\src\Reown.Core.Network
	Boxx\src\Reown.Core.Network.WebSocket
	Boxx\src\Reown.Core.Storage
	Boxx\src\Reown.Sign
	Boxx\src\Reown.Sign.Nethereum
	Boxx\src\Reown.Sign.Nethereum.Unity
	Boxx\src\Reown.Sign.Unity
	Boxx\src\Reown.Unity.Dependencies
	Boxx\src\Reown.WalletKit
	
You may refer to the following sample `manifest.json` to understand implementation.

=== === ===

{
  "dependencies": {
    "com.cdm.figma": "https://github.com/cdmvision/unity-figma-importer.git#1.9.0",
    "com.cdm.figma.ui": "https://github.com/cdmvision/unity-figma-importer.git#1.9.0-ugui",
    "com.github-glitchenzo.nugetforunity": "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity",
    "com.nethereum.unity": "4.19.2",
    "com.nobi.roundedcorners": "https://github.com/kirevdokimov/Unity-UI-Rounded-Corners.git",
    "com.reown.appkit.unity": "file:./../src/Reown.AppKit.Unity",
    "com.reown.core": "file:./../src/Reown.Core",
    "com.reown.core.common": "file:./../src/Reown.Core.Common",
    "com.reown.core.crypto": "file:./../src/Reown.Core.Crypto",
    "com.reown.core.network": "file:./../src/Reown.Core.Network",
    "com.reown.core.storage": "file:./../src/Reown.Core.Storage",
    "com.reown.sign": "file:./../src/Reown.Sign",
    "com.reown.sign.nethereum": "file:./../src/Reown.Sign.Nethereum",
    "com.reown.sign.nethereum.unity": "file:./../src/Reown.Sign.Nethereum.Unity",
    "com.reown.sign.unity": "file:./../src/Reown.Sign.Unity",
    "com.reown.unity.dependencies": "file:./../src/Reown.Unity.Dependencies",
    "com.unity.2d.sprite": "1.0.0",
    "com.unity.ai.navigation": "1.1.5",
    "com.unity.cinemachine": "2.10.1",
    "com.unity.collab-proxy": "2.4.4",
    "com.unity.ide.rider": "3.0.31",
    "com.unity.ide.visualstudio": "2.0.22",
    "com.unity.ide.vscode": "1.2.5",
    "com.unity.inputsystem": "1.10.0",
    "com.unity.mobile.android-logcat": "1.4.2",
    "com.unity.nuget.mono-cecil": "1.11.4",
    "com.unity.postprocessing": "3.4.0",
    "com.unity.render-pipelines.universal": "14.0.11",
    "com.unity.test-framework": "1.1.33",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.timeline": "1.7.6",
    "com.unity.toolchain.win-x86_64-linux-x86_64": "2.0.10",
    "com.unity.ugui": "1.0.0",
    "com.unity.visualscripting": "1.9.5",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  },
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.reown",
        "com.nethereum"
      ]
    }
  ]
}
