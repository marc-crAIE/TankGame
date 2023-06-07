using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.Scenes;
using Raylib_cs;
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
            if (!ImGuiLayer.IsEnabled())
                Raylib.DrawText("Press F1 to open the debug editor", 10, 10, 14, Color.RAYWHITE);

            if (Input.IsKeyTyped(Key.KEY_F1))
                ImGuiLayer.Enable(!ImGuiLayer.IsEnabled());
        }
    }
}
