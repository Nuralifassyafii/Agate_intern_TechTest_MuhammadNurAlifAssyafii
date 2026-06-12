using UnityEngine;

public abstract class SceneInstallerBase : ServiceInstaller
{
    private SceneEventBus _eventBus = new SceneEventBus();
    public SceneEventBus EventBus => _eventBus;

    protected override void Awake()
    {
        GameContext.SceneServices = Services;
        GameContext.SceneEvents = _eventBus;

        base.Awake();
    }

    protected virtual void OnDestroy()
    {
        Services.Clear();
        _eventBus.Clear();

        if (GameContext.SceneServices == Services)
        {
            GameContext.SceneServices = null;
        }

        if (GameContext.SceneEvents == _eventBus)
        {
            GameContext.SceneEvents = null;
        }

    }
}
