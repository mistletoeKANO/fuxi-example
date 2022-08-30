using System.Threading;
using Cysharp.Threading.Tasks;

public static class AppStart
{
    public static void Start()
    {
        UniTaskScheduler.UnobservedTaskException += e =>
        {
            Debugger.LogError(e.ToString());
        };
        SynchronizationContext.SetSynchronizationContext(new UniTaskSynchronizationContext());
        
        AsyncStart();
    }

    private static async void AsyncStart()
    {
        EventManager.Instance.Init();
        await UIManager.Instance.Init();
        GameSceneManager.Instance.Init();
        UpdaterManager.Instance.Init();
        TimerManager.Instance.Init();

        TypeManager.Instance.Init();

        GameSceneManager.Instance.LoadScene<LoginScene>();
    }
}