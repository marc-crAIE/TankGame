using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using RayEngine.Scenes;
using SharpMaths;
using TankGame.Assets.Scripts;

namespace TankGame
{
    internal class TankGame : Application
    {
        private Scene Scene;
        private GameObject TankBody;
        private GameObject TankTurret;

        public TankGame(ApplicationSpecification specification) : base(specification)
        {
            Scene = new Scene();
            SceneManager.LoadScene(Scene);

            GameObject tankBody = new GameObject("Tank Body");
            GameObject tankTurret = new GameObject("Tank Turret");
            tankTurret.SetParent(ref tankBody);

            tankBody.AddComponent<ScriptComponent>().Bind<TankScript>();
            tankTurret.AddComponent<ScriptComponent>().Bind<TurretScript>();
        }
    }
}
