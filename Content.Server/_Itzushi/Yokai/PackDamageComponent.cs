namespace Content.Server._Itzushi.Yokai;

[RegisterComponent]
public sealed partial class PackDamageComponent : Component
{
    /// <summary>
    /// How far to search for nearby PackDamage entities.
    /// </summary>
    [DataField]
    public float Radius = 20f;

    /// <summary>
    /// What factor of additional damage to deal per each nearby PackDamage component owner (multiplier).
    /// </summary>
    [DataField]
    public float DamagePerPackMember = 2f;

    /// <summary>
    /// Only applies to PackDamage component owners who also have ANY of these components.
    /// Set to PackDamageComponent or null to apply to any PackDamage component owner.
    /// </summary>
    [DataField]
    public List<ComponentRegistration> PackComponents = new();
}

