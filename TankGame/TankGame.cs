using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    internal class TankGame : Application
    {
        private Scene Scene;
        private GameObject TankBody;

        public TankGame(ApplicationSpecification specification) : base(specification)
        {
            Scene = new Scene();
            SceneManager.LoadScene(Scene);

            TankBody = new GameObject();
        }
    }
}
