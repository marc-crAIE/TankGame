using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Assets.Scripts
{
    internal class ManagerScript : ScriptableGameObject
    {
        Layer ImGuiLayer;

        public override void OnCreate()
        {
            Scene? scene = SceneManager.GetScene();
            ImGuiLayer = scene.GetLayers().GetLayer("ImGUI");
        }

        public override void OnUpdate(Timestep ts)
        {
            if (Input.IsKeyTyped(Key.KEY_F1))
                ImGuiLayer.Enable(!ImGuiLayer.IsEnabled());
        }
    }
}
