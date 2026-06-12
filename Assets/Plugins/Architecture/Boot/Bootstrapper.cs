using UnityEngine;

[DefaultExecutionOrder(-999)]
public static class Bootstrapper
{
    private static bool _initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        BootConfig bootConfig = Resources.Load<BootConfig>("BootConfig");

        if (bootConfig != null && bootConfig.PersistentInstaller != null)
        {
            GameObject instance = Object.Instantiate(bootConfig.PersistentInstaller.gameObject);
            Object.DontDestroyOnLoad(instance);
        }
        else
        {
            Debug.LogError("GamePersistent.prefab not found in Resources folder!");
        }
    }
}
