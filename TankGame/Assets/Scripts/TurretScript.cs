using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Assets.Scripts
{
    internal class TurretScript : ScriptableGameObject
    {
        private TransformComponent _Transform;
        private TransformComponent BodyTransform;

        public bool MouseControls = false;

        private const float RotationSpeed = 1.5f;

        public override void OnCreate()
        {
            AddComponent<SpriteComponent>(new Colour(0, 0, 255));

            Transform.Scale = new Vector2(1.5f, 0.5f);

            _Transform = Transform;
            GameObject? tankBody = Scene.GetWithTag("Tank Body");
            if (tankBody is not null)
                BodyTransform = tankBody.GetComponent<TransformComponent>();
        }

        public override void OnUpdate(Timestep ts)
        {
            // Rotation
            if (MouseControls)
            {
                Vector3 mousePos = Input.GetMousePosition();
                Vector3 tankPos = BodyTransform.Translation;
                Quaternion tankRotation = new Quaternion(BodyTransform.Rotation + _Transform.Rotation);

                Vector3 dirToObject = (mousePos - tankPos);
                dirToObject.Normalize();

                Matrix4 tankMatrix = (Matrix4)tankRotation * Matrix4.Translation(tankPos);
                Matrix4 inverseTankMatrix = tankMatrix.Inverse();

                Vector3 mousePosTankSpace = inverseTankMatrix * dirToObject;
                mousePosTankSpace.Normalize();

                float angleToMouse = (float)Math.Atan2(mousePosTankSpace.y, mousePosTankSpace.x);

                _Transform.Rotation.z += angleToMouse * RotationSpeed * ts;

                if (Input.IsMouseButtonClicked(Mouse.MOUSE_BUTTON_LEFT))
                    Shoot();
            }
            else
            {
                if (Input.IsKeyPressed(Key.KEY_Q))
                    _Transform.Rotation.z -= RotationSpeed * ts;
                if (Input.IsKeyPressed(Key.KEY_E))
                    _Transform.Rotation.z += RotationSpeed * ts;

                // Shoot
                if (Input.IsKeyTyped(Key.KEY_SPACE))
                    Shoot();
            }

        }

        private void Shoot()
        {
            GameObject bullet = new GameObject("Bullet");
            float angle = BodyTransform.Rotation.z + _Transform.Rotation.z;
            bullet.AddComponent<ScriptComponent>().Bind<BulletScript>(angle);

            var bulletTransform = bullet.GetComponent<TransformComponent>() = (Matrix4)BodyTransform * _Transform;
            float turretLength = (BodyTransform.Scale * _Transform.Scale).x;
            Vector2 offset = new Vector2((float)Math.Cos(angle) * turretLength, (float)Math.Sin(angle) * turretLength) / 2;
            bulletTransform.Translation += offset;
        }
    }
}
