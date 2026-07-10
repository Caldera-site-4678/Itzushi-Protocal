using Content.Server.Telephone;
using Content.Shared.Telephone;
using Content.Trauma.Shared.Phones.Components;
using Content.Trauma.Shared.Phones.Systems;
using Robust.Shared.Timing;

namespace Content.Server._Itzushi.Yokai;

/// <summary>
/// Makes an entity (e.g. Teke-Teke) periodically call every telephone within range. After a set number of
/// "ringing events" the aura goes dormant until <see cref="PhoneRingerAuraComponent.CallResetInterval"/> passes.
/// Rings both TelephoneComponents and RotaryPhoneComponents from Trauma Station which are unrelated systems.
/// </summary>
public sealed partial class PhoneRingerAuraSystem : EntitySystem
{
    [Dependency] private TelephoneSystem _telephone = default!;
    [Dependency] private SharedRotaryPhoneSystem _rotaryPhone = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PhoneRingerAuraComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<PhoneRingerAuraComponent> entity, ref MapInitEvent args)
    {
        entity.Comp.NextRingTime = _timing.CurTime + entity.Comp.RingerInterval; // stagger the first ring slightly
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<PhoneRingerAuraComponent>();

        while (query.MoveNext(out var uid, out var aura))
        {
            // reset the call counter once enough time has passed
            if (aura.CallsMade >= aura.CallsToMake)
            {
                if (curTime < aura.LastTargetSeen + aura.CallResetInterval)
                    continue;

                aura.CallsMade = 0;
                aura.NextRingTime = curTime + aura.RingerInterval;
                continue;
            }

            if (curTime < aura.NextRingTime)
                continue;

            CancelPendingRotaryRings(uid, aura);

            var rangTelephone = RingNearbyTelephones(uid, aura);
            var rangRotary = RingNearbyRotaryPhones(uid, aura);

            if (!rangTelephone && !rangRotary)
                continue;

            aura.CallsMade++;
            aura.LastTargetSeen = curTime;
            aura.NextRingTime = curTime + aura.RingerInterval;
        }
    }

    private bool RingNearbyTelephones(EntityUid uid, PhoneRingerAuraComponent aura)
    {
        if (!TryComp<TelephoneComponent>(uid, out var telephone))
            return false;

        var source = new Entity<TelephoneComponent>(uid, telephone);

        if (_telephone.IsTelephoneEngaged(source))
            _telephone.TerminateTelephoneCalls(source);

        var receivers = new HashSet<Entity<TelephoneComponent>>();

        foreach (var candidate in _lookup.GetEntitiesInRange<TelephoneComponent>(Transform(uid).Coordinates, aura.Radius))
        {
            if (candidate.Owner == uid)
                continue;

            if (!_telephone.IsTelephonePowered(candidate) || _telephone.IsTelephoneEngaged(candidate))
                continue;

            if (candidate.Comp.CurrentState == TelephoneState.Ringing)
                continue;

            receivers.Add(candidate);
        }

        if (receivers.Count == 0)
            return false;

        _telephone.BroadcastCallToTelephones(source, receivers, uid);
        return true;
    }

    private bool RingNearbyRotaryPhones(EntityUid uid, PhoneRingerAuraComponent aura)
    {
        var rangAny = false;

        foreach (var candidate in _lookup.GetEntitiesInRange<RotaryPhoneComponent>(Transform(uid).Coordinates, aura.Radius))
        {
            if (candidate.Owner == uid)
                continue;

            if (!_rotaryPhone.TryRingPhone(uid, candidate))
                continue;

            aura.RungRotaryPhones.Add(candidate.Owner);
            rangAny = true;
        }

        return rangAny;
    }

    private void CancelPendingRotaryRings(EntityUid uid, PhoneRingerAuraComponent aura)
    {
        if (aura.RungRotaryPhones.Count == 0)
            return;

        foreach (var phoneUid in aura.RungRotaryPhones)
        {
            if (Deleted(phoneUid) || !TryComp<RotaryPhoneComponent>(phoneUid, out var phoneComp))
                continue;

            _rotaryPhone.CancelRing(new Entity<RotaryPhoneComponent>(phoneUid, phoneComp), uid);
        }

        aura.RungRotaryPhones.Clear();
    }
}
