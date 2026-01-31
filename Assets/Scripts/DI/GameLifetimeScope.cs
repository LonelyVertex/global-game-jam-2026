using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private FadeController fadeController;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(fadeController);
    }
}
