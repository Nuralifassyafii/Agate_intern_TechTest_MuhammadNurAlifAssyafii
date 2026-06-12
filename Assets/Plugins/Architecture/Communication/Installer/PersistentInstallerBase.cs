using UnityEngine;

public abstract class PersistentInstallerBase : ServiceInstaller
{
    private static PersistentInstallerBase _instance;

    protected override void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning(
                "[PersistentInstaller] Duplicate persistent installer found. Destroying persistent installer."
            );
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        base.Awake();

        GameContext.PersistentServices = Services;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
