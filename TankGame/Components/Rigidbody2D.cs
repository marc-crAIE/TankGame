using SharpMaths;

namespace TankGame.Components
{
    internal class Rigidbody2D
    {
        public Vector2 Position = new Vector2(0.0f);
        // Rotation is done only along the z-axis
        public float Rotation = 0f;
        public Vector2 Scale = new Vector2(1.0f);

        public float Mass = 1.0f;
        public Vector2 Velocity = new Vector2(0.0f);
        public float AngularVelocity = 0.0f;

        // Already done physics with this
        internal bool Dirty = false;
    }
}
