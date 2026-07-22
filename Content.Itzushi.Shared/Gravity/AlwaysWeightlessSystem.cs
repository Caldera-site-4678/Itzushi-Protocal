using Content.Shared.Gravity;

namespace Content.Itzushi.Shared.Gravity;

/// <summary>
/// forces any entity with AlwaysWeightlessComponent to be treated as weightless,
/// regardless of whether the map currently has gravity enabled
/// this is useful for ghost-like mobs such as errant souls that should float
/// no matter what
/// </summary>

public sealed partial class AlwaysWeightlessSystem : EntitySystem
{
    [Dependency] private SharedGravitySystem _gravity = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AlwaysWeightlessComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<AlwaysWeightlessComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<AlwaysWeightlessComponent, IsWeightlessEvent>(OnIsWeightless);
    }

    private void OnMapInit(EntityUid uid, AlwaysWeightlessComponent component, MapInitEvent args)
    {
        // this makess sure weightlessness is applied whether the entity was map-loaded,
        // spawned, or modified at startup
        _gravity.RefreshWeightless(uid);
    }

    private void OnStartup(EntityUid uid, AlwaysWeightlessComponent component, ComponentStartup args)
    {
        // tell the gravity system this entity is always weightless
        _gravity.RefreshWeightless(uid);
    }

    private void OnIsWeightless(EntityUid uid, AlwaysWeightlessComponent component, ref IsWeightlessEvent args)
    {
        args.IsWeightless = true;
        args.Handled = true;
    }
}
