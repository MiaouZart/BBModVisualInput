# BBVisualInput

This is a .NET Framework 4.7.2 class library intended for use with the game **BETON BRUTAL**.  
It provides a visual input system that integrates with Unity and the BBModMenu modding framework to enhance input visualization and interactivity.

## Requirements

To successfully build this project, the following libraries must be placed in the `Lib` folder:

- `0Harmony.dll`
- `Assembly-CSharp.dll` – Extracted from the game (BETON BRUTAL)
- `BBModMenu.dll` – Built from the BBModMenu project
- `Facepunch.Steamworks.Win64.dll`
- `MelonLoader.dll`
- `Unity.InputSystem.dll`
- `Unity.Postprocessing.Runtime.dll`
- `Unity.TextMeshPro.dll`
- `UnityEngine.dll`
- `UnityEngine.AIModule.dll`
- `UnityEngine.AnimationModule.dll`
- `UnityEngine.AssetBundleModule.dll`
- `UnityEngine.AudioModule.dll`
- `UnityEngine.CoreModule.dll`
- `UnityEngine.ImageConversionModule.dll`
- `UnityEngine.IMGUIModule.dll`
- `UnityEngine.InputLegacyModule.dll`
- `UnityEngine.InputModule.dll`
- `UnityEngine.JSONSerializeModule.dll`
- `UnityEngine.ParticleSystemModule.dll`
- `UnityEngine.Physics2DModule.dll`
- `UnityEngine.PhysicsModule.dll`
- `UnityEngine.SharedInternalsModule.dll`
- `UnityEngine.TextCoreFontEngineModule.dll`
- `UnityEngine.TextCoreTextEngineModule.dll`
- `UnityEngine.TextRenderingModule.dll`
- `UnityEngine.UI.dll`
- `UnityEngine.UIElementsModule.dll`
- `UnityEngine.UIModule.dll`
- `UnityEngine.UnityWebRequestModule.dll`
- `UnityEngine.UnityWebRequestTextureModule.dll`
- `UnityEngine.VRModule.dll`
- `UnityEngine.XRModule.dll`

> 💡 **All DLLs should be placed in the `Lib` folder at the root of the project to ensure successful compilation.**

## Build Output

The output of the project is a DLL file:
- Located in `bin\Debug\` or `bin\Release\` depending on the build configuration.
- After build, the DLL is **automatically copied** to the game's mod directory:

