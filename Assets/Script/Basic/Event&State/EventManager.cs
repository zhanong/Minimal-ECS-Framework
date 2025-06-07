namespace ECSFramework
{
    public class EventManager : IBasicManager
    {
        public static EventManager Singleton;

        public void Initialize()
        {
            Singleton = this;
            BasicEventManager.singleton.onEventManagerCreated?.Invoke();
        }

        public void OnDestroy()
        {
            Singleton = null;
        }

        IBasicManager IBasicManager.OnNewScene()
        {
            Singleton = new EventManager();
            BasicEventManager.singleton.onEventManagerCreated?.Invoke();
            return Singleton;
        }

        public string Stamp { get; set; }

    }
}
