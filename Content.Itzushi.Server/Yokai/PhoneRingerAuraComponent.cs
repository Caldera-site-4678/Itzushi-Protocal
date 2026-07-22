namespace Content.Itzushi.Server.Yokai;

/// <summary>
/// PhoneRingerAura Entities require a TelephoneComponent
/// </summary>
[RegisterComponent]
public sealed partial class PhoneRingerAuraComponent : Component
{
    /// <summary>
    /// How far to search for telephones.
    /// </summary>
    [DataField]
    public float Radius = 100f;

    /// <summary>
    /// How long to wait between ringing events.
    /// </summary>
    [DataField]
    public TimeSpan RingerInterval = TimeSpan.FromSeconds(20);

    /// <summary>
    ///  Maximum number of ringing events before the aura goes dormant.
    /// </summary>
    [DataField]
    public int CallsToMake = 6;

    /// <summary>
    ///  Time to wait before the number of ringing events resets.
    /// </summary>
    [DataField]
    public TimeSpan CallResetInterval = TimeSpan.FromSeconds(120);

    /// <summary>
    ///  Tracked because RotaryPhoneComponent has no ring-timeout of its own.
    /// </summary>
    public HashSet<EntityUid> RungRotaryPhones = new();

    public TimeSpan NextRingTime;
    public TimeSpan LastTargetSeen;
    public int CallsMade;
}
