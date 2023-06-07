using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;

namespace TankGame.Assets.Scripts
{
    internal class TurretScript : ScriptableGameObject
    {
        private TransformComponent _Transform;
        private TransformComponent PivotTransform;
        private TransformComponent BodyTransform;

        public bool MouseControls = false;
        private Colour Colour = new Colour(152, 196, 92);

        private const float RotationSpeed = 1.5f;
        private const float Scale = 1.0f;

        public override void OnCreate()
        {
            var sprite = AddComponent<SpriteComponent>(Colour);
            sprite.Texture = Resources.Textures.TankTurret;

            Vector2 tankTextSize = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
            tankTextSize.Normalize();

            // These values were grabbed when using the debugger editing tools
            Transform.Translation = new Vector2(0.2f, 0.01f);
            Transform.Scale = tankTextSize * Scale;

            _Transform = Transform;
            GameObject? tankBody = Scene.GetWithTag("Tank Body");
            PivotTransform = tankBody.GetChildrenWithTag("Turret Pivot").GetComponent<TransformComponent>();
            if (tankBody is not null)
                BodyTransform = tankBody.GetComponent<TransformComponent>();
        }

        public override void OnUpdate(Timestep ts)
        {
            // Rotation
            if (MouseControls)
            {
                Vector3 mousePos = Input.GetMousePosition();
                // The turret pivot is always dead center of the tanks body. If it was not you can get its position by
                // multiplying the BodyTransform and the PivotTransform and then get the translation of the result
                Vector3 tankPos = BodyTransform.Translation;
                Quaternion turretRotation = new Quaternion(BodyTransform.Rotation + PivotTransform.Rotation);

                Vector3 dirToMouse = mousePos - tankPos;
                dirToMouse.Normalize();

                Matrix4 tankMatrix = (Matrix4)turretRotation * Matrix4.Translation(tankPos);
                Matrix4 inverseTankMatrix = tankMatrix.Inverse();

                Vector3 mousePosTankSpace = inverseTankMatrix * dirToMouse;
                mousePosTankSpace.Normalize();

                float angleToMouse = (float)Math.Atan2(mousePosTankSpace.y, mousePosTankSpace.x);

                if (Math.Abs(angleToMouse) < (RotationSpeed * ts))
                    PivotTransform.Rotation.z += angleToMouse * RotationSpeed * ts;
                else if (angleToMouse < 0.0f)
                    PivotTransform.Rotation.z -= RotationSpeed * ts;
                else if (angleToMouse > 0.0f)
                    PivotTransform.Rotation.z += RotationSpeed * ts;

                if (Input.IsMouseButtonClicked(Mouse.MOUSE_BUTTON_LEFT))
                    Shoot();
            }
            else
            {
                if (Input.IsKeyPressed(Key.KEY_Q))
                    PivotTransform.Rotation.z -= RotationSpeed * ts;
                if (Input.IsKeyPressed(Key.KEY_E))
                    PivotTransform.Rotation.z += RotationSpeed * ts;

                // Shoot
                if (Input.IsKeyTyped(Key.KEY_SPACE))
                    Shoot();
            }

        }

        private void Shoot()
        {
            GameObject bullet = new GameObject("Bullet");
            float angle = BodyTransform.Rotation.z + PivotTransform.Rotation.z;
            bullet.AddComponent<ScriptComponent>().Bind<BulletScript>(angle);

            var bulletTransform = bullet.GetComponent<TransformComponent>() = (Matrix4)BodyTransform * PivotTransform * _Transform;
            float turretLength = (BodyTransform.Scale * _Transform.Scale).x + _Transform.Translation.x;
            Vector2 offset = new Vector2((float)Math.Cos(angle) * turretLength, (float)Math.Sin(angle) * turretLength) / 2;
            bulletTransform.Translation += offset;
        }
    }
}
