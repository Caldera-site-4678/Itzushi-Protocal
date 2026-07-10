using Robust.Shared.GameStates;
using System.Numerics;

namespace Content.Shared._Itzushi.Gravity;

/// <summary>
/// client component that makes an entity's sprite bob up and down.
/// does not affect physics, collision, or entity's position.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FloatBobVisualsComponent : Component
{
    // how far the sprite be bobbing
    [DataField]
    public float Amplitude = 0.08f;

    // how fast the bobbing animation cycles
    [DataField]
    public float Frequency = 1.5f;

    [ViewVariables]
    public Vector2 BaseOffset;

    [ViewVariables]
    public bool BaseOffsetSet;
}
