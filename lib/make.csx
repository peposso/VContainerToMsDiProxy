#load "_scripts.csx"
#if false
{
#endif

Directory.SetCurrentDirectory(GetExecutingScriptDirectory());

// https://github.com/hadashiA/VContainer
await BuildUnityPackageAssemblies(
        "jp.hadashikick.vcontainer",
        "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.11.1",
        new[] { "VContainer.dll" });
