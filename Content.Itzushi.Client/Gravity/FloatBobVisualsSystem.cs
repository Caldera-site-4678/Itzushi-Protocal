using Content.Itzushi.Shared.Gravity;
using Robust.Shared.Timing;

namespace Content.Itzushi.Client.Gravity;

/// <summary>
/// handles visual bobbing for floating entities
/// this only changes the sprite offset on the client ONLY
/// </summary>

public sealed partial class FloatBobVisualsSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SpriteSystem _sprite = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var time = (float) _timing.CurTime.TotalSeconds;
        var query = EntityQueryEnumerator<FloatBobVisualsComponent, SpriteComponent>();

        while (query.MoveNext(out var uid, out var bob, out var sprite))
        {
            if (!bob.BaseOffsetSet)
            {
                bob.BaseOffset = sprite.Offset;
                bob.BaseOffsetSet = true;
            }

            var yOffset = MathF.Sin(time * MathF.Tau * bob.Frequency) * bob.Amplitude;
            _sprite.SetOffset(uid, bob.BaseOffset + new Vector2(0f, yOffset));
        }
    }
}
