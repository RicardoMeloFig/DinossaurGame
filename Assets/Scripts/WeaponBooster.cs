using Unity.Netcode;

namespace Assets.Scripts
{
    public struct WeaponBooster : INetworkSerializable, System.IEquatable<WeaponBooster>
    {
        public float PowerAmplifier;
        public float Duration;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out PowerAmplifier);
                reader.ReadValueSafe(out Duration);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(PowerAmplifier);
                writer.WriteValueSafe(Duration);
            }
        }

        public bool Equals(WeaponBooster other)
        {
            return PowerAmplifier == other.PowerAmplifier && Duration == other.Duration;
        }
    }
}
