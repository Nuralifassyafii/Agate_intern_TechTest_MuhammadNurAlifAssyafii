using System.Collections.Generic;
using UnityEngine;

public abstract class ServiceInstaller : MonoBehaviour
{
    [SerializeField]
    private List<MonoBehaviour> _monoServices;

    private ServiceLocator _services = new ServiceLocator();

    public ServiceLocator Services { get => _services; }

    protected virtual void Awake()
    {
        RegisterServices();
        RegisterMonoServices();
    }

    public abstract void RegisterServices();

    private void RegisterMonoServices()
    {
        foreach (MonoBehaviour monoService in _monoServices)
        {
            _services.Register(monoService);
        }
    }
}
