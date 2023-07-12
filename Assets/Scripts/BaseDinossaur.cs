using Assets.Scripts;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseDinossaur : INetworkSerializable, System.IEquatable<BaseDinossaur>
{
    public abstract string Name { get; protected set; }

    protected virtual float Health { get; set; } = 1000f;

    protected float Stamina { get; set; } = 100f;
    protected virtual float StaminaLoss { get; set; } = 1f;

    public virtual float TopWalkingSpeed { get; protected set; } = 8f;
    public virtual float TopSprintSpeed { get; protected set; } = 15f;
    public virtual float Weight { get; protected set; } = 7f;
    public virtual float Accelaration { get; protected set; } = 1200f;
    public virtual float GroundDrag { get; protected set; } = 3f;

    public float Thirst = 1200f;
    private const float MaxThirst  = 1200f;
    public float ThirstLoss { get; set; } = 20f;

    private const float MaxHunger = 1200f;

    public float Hunger = 1200f;
    public float HungerLoss { get; set; } = 20f;

    protected float BloodPool { get; set; } = 200f;
    protected virtual float BloodLoss { get; set; } = 1f;

    public bool CanJump { get; set; } = true;
    public virtual float JumpHeight { get; protected set; } = 70f;
    protected int StatusTimer { get; set; } = 0;
    protected virtual int StopBleedRestingDuration { get; set; } = 10;
    protected bool IsBleeding { get; set; } = false; 
    protected bool HasBrokenBone { get; set; } = false;
    protected bool Blinded { get; set; } = false;
    protected PlayerStatus currentState = PlayerStatus.Normal;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Hunger);
            reader.ReadValueSafe(out Thirst);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Hunger);
            writer.WriteValueSafe(Thirst);
        }
    }

    public bool Equals(BaseDinossaur other)
    {
        return Hunger == other.Hunger && Thirst == other.Thirst;
    }


}
