using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;
using TankGame.Components;

namespace TankGame.Assets.Scripts
{
    internal class BulletScript : ScriptablePhysicsGameObject
    {
        private float Angle = 0.0f;
        private Vector2 Velocity;
        private TransformComponent _Transform;

        private const float Speed = 1500.0f;
        private const float Size = 5;

        private bool Collided = false;

        public BulletScript(float angle)
        {
            Angle = angle;
            Velocity = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            _Transform = new TransformComponent();
        }

        public override void OnCreate()
        {
            AddComponent<SpriteComponent>(new Colour(128, 128, 128));
            AddComponent<Rigidbody2D>();

            _Transform = Transform;
            _Transform.Scale = new Vector2(Size);
        }

        public override void OnUpdate(Timestep ts)
        {
            _Transform.Translation += Velocity * (Speed * ts);

            Window window = Application.Instance.GetWindow();
            Vector2 size = new Vector2(window.GetWidth(), window.GetHeight());
            Vector2 pos = _Transform.Translation;
            if (pos.x < 0 || pos.y < 0 || pos.x > size.x || pos.y > size.y)
                Scene.RemoveGameObject(Self);

            if (Collided)
                Scene.RemoveGameObject(Self);
        }

        public override void OnCollision2D(GameObject other)
        {
            Console.WriteLine($"{Self.GetTag()} colliding with {other.GetTag()}");
            Collided = true;
        }
    }
}
