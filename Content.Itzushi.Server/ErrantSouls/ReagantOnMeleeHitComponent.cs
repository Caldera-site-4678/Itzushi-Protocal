using Content.Shared.FixedPoint;

namespace Content.Itzushi.Server.ErrantSouls;

[RegisterComponent]
public sealed partial class ReagentOnMeleeHitComponent : Component
{
    [DataField(required: true)]
    public string Reagent = string.Empty;

    [DataField]
    public FixedPoint2 Quantity = FixedPoint2.New(5);

    [DataField]
    public float Chance = 1f;
}
