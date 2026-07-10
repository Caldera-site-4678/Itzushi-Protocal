using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Random;

namespace Content.Server._Itzushi.ErrantSouls;

public sealed partial class ReagentOnMeleeHitSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedSolutionContainerSystem _solutionContainer = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ReagentOnMeleeHitComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnMeleeHit(EntityUid uid, ReagentOnMeleeHitComponent component, MeleeHitEvent args)
    {
        if (!args.IsHit)
            return;

        foreach (var target in args.HitEntities)
        {
            if (!_random.Prob(component.Chance))
                continue;
            // now we blast this mothafucka a belly full of chems 🫃 -pierow
            if (!_solutionContainer.TryGetInjectableSolution(target, out var solution, out _))
                continue;

            _solutionContainer.TryAddReagent(solution.Value, component.Reagent, component.Quantity);
        }
    }
}
