public static class GameContext
{
    public static ServiceLocator PersistentServices { get; internal set; }
    public static ServiceLocator SceneServices { get; internal set; }

    public static SceneEventBus SceneEvents { get; internal set; }
}
