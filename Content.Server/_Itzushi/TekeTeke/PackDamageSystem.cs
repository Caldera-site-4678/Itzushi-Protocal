using System.Diagnostics;
using System.Linq;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Server._Itzushi.TekeTeke;

public sealed partial class PackDamageSystem : EntitySystem
{
    [Dependency] private IComponentFactory _componentFactory = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PackDamageComponent, GetMeleeDamageEvent>(OnGetMeleeDamage);
    }

    private void OnGetMeleeDamage(Entity<PackDamageComponent> ent, ref GetMeleeDamageEvent args)
    {
        var count = CountNearbyPackMembers(ent.Owner, ent.Comp.Radius, ent.Comp.PackComponents);

        if (count <= 1)
            return;

        var multiplier = 1f + (count - 1) * ent.Comp.DamagePerPackMember;

        args.Damage *= multiplier;

        Log.Info($"Count: {count}");
        Log.Info($"Damage: {args.Damage}");
    }

    private int CountNearbyPackMembers(EntityUid uid, float radius, List<ComponentRegistration>? packComponents)
    {
        var count = 1;

        packComponents ??= (List<ComponentRegistration>)[];

        if (packComponents.Count == 0 &&
            _componentFactory.TryGetRegistration(nameof(PackDamageComponent), out var reg))
        {
            packComponents.Add(reg);
        }

        foreach (var candidate in _lookup.GetEntitiesInRange<PackDamageComponent>(Transform(uid).Coordinates, radius))
        {
            if (candidate.Owner == uid)
                continue;

            if (packComponents.Count == 0)
            {
                count++;
                continue;
            }

            foreach (var comp in packComponents)
            {
                if (!HasComp(candidate.Owner, comp.Type))
                    continue;

                count++;
                break;
            }
        }

        return count;
    }
}
