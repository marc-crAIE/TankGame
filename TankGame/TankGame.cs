using ImGuiNET;
using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using RayEngine.Graphics;
using RayEngine.ImGUI;
using RayEngine.Scenes;
using SharpMaths;
using System.Reflection;
using TankGame.Assets.Scripts;

namespace TankGame
{
    internal class TankGame : Application
    {
        private Scene Scene;
        private GameObject TankBody;
        private GameObject TankTurret;

        private TurretScript? TurretScript;

        private GameObject? SelectedGameObject = null;
        private float OverlayWidth = 0.0f;

        public TankGame(ApplicationSpecification specification) : base(specification)
        {
            Scene = new Scene();
            SceneManager.LoadScene(Scene);

            Scene.OnImGUIRender = OnImGuiRender;

            GameObject manager = new GameObject("Manager");
            manager.AddComponent<ScriptComponent>().Bind<ManagerScript>();

            TankBody = new GameObject("Tank Body");
            TankTurret = new GameObject("Tank Turret");
            TankTurret.SetParent(ref TankBody);

            TankBody.AddComponent<ScriptComponent>().Bind<TankScript>();
            TankTurret.AddComponent<ScriptComponent>().Bind<TurretScript>();
            TurretScript = (TurretScript?)TankTurret.GetComponent<ScriptComponent>().Instance;
        }

        private void OnImGuiRender()
        {
            DrawOverlay();
            DrawComponentsWindow();
        }

        private void DrawOverlay()
        {
            ImGuiWindowFlags overlayFlags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;

            var viewport = ImGui.GetMainViewport();
            Vector2 windowPos = new Vector2(viewport.WorkPos.X + 10, viewport.WorkPos.X + 10);
            ImGui.SetNextWindowPos(windowPos);
            ImGui.SetNextWindowViewport(viewport.ID);

            overlayFlags |= ImGuiWindowFlags.NoMove;

            ImGui.SetNextWindowBgAlpha(0.35f);
            ImGui.Begin("Overlay", overlayFlags);

            // Render stats
            ImGui.Text("FPS: " + Renderer.GetFPS());

            ImGui.SeparatorText("Settings");

            if (TurretScript is not  null)
                ImGui.Checkbox("Turret Mouse Controls", ref TurretScript.MouseControls);

            ImGui.SeparatorText("Object Hierarchy");

            Scene? scene = SceneManager.GetScene();
            if (scene is not null)
            {
                int index = 0;
                foreach (GameObject gameObject in scene.GetGameObjects())
                {
                    DrawGameObjectTreeNode(gameObject, ref index);
                    index++;
                }
            }

            OverlayWidth = ImGui.GetWindowWidth();

            ImGui.End();
        }

        private void DrawGameObjectTreeNode(GameObject gameObject, ref int index)
        {
            if (ImGui.TreeNode(gameObject.GetTag() + $"##node{index}"))
            {
                index++;
                SelectedGameObject = gameObject;
                foreach (GameObject child in gameObject.GetChildren())
                {
                    DrawGameObjectTreeNode(child, ref index);
                }
                ImGui.TreePop();
            }
        }

        private void DrawComponentsWindow()
        {
            if (SelectedGameObject is null)
                return;

            ImGuiWindowFlags overlayFlags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;

            var viewport = ImGui.GetMainViewport();
            Vector2 windowPos = new Vector2(viewport.WorkPos.X + OverlayWidth + 20, viewport.WorkPos.Y + 10);
            ImGui.SetNextWindowPos(windowPos);
            ImGui.SetNextWindowViewport(viewport.ID);

            overlayFlags |= ImGuiWindowFlags.NoMove;

            ImGui.SetNextWindowBgAlpha(0.35f);
            ImGui.Begin("Components", overlayFlags);

            ImGui.Text($"UUID: {SelectedGameObject.GetID()}");
            ImGui.Text($"Tag: {SelectedGameObject.GetTag()}");

            //TODO: Make tags editable without freaking out the tree
            //ImGui.SameLine();
            //ImGui.InputText("##Tag", ref SelectedGameObject.GetComponent<TagComponent>().Tag, 32);

            if (SelectedGameObject.HasComponent<TransformComponent>())
            {
                ImGui.SeparatorText("Transform Component");

                ref var transform = ref SelectedGameObject.GetComponent<TransformComponent>();
                System.Numerics.Vector3 translation = transform.Translation;
                System.Numerics.Vector3 rotation = SharpMath.ToDegrees(transform.Rotation);
                System.Numerics.Vector3 scale = transform.Scale;

                if (ImGui.DragFloat3("Translation", ref translation, 0.1f))
                    transform.Translation = translation;
                if (ImGui.DragFloat3("Rotation", ref rotation, 0.1f))
                    transform.Rotation = SharpMath.ToRadians((Vector3)rotation);
                if (ImGui.DragFloat3("Scale", ref scale, 0.1f))
                    transform.Scale = scale;

                ImGui.Spacing();
            }

            if (SelectedGameObject.HasComponent<SpriteComponent>())
            {
                ImGui.SeparatorText("Sprite Component");

                ref var sprite = ref SelectedGameObject.GetComponent<SpriteComponent>();
                System.Numerics.Vector4 colour = (Vector4)sprite.Colour;
                if (ImGui.ColorEdit4("Colour", ref colour))
                    sprite.Colour = (Vector4)colour;

                if (sprite.Texture is not null)
                {
                    ImGuiContext.ImageSize(sprite.Texture, 128, 128);
                }

                ImGui.Spacing();
            }

            if (SelectedGameObject.HasComponent<ScriptComponent>())
            {
                ImGui.SeparatorText("Script Component");

                var instance = SelectedGameObject.GetComponent<ScriptComponent>().Instance;
                ImGui.Text($"{instance?.GetType()}");


                if (instance is not null)
                {
                    var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

                    var fields = instance.GetType().GetFields(bindingFlags).ToArray();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        ImGui.Separator();

                        string name = fields[i].Name;
                        var value = fields[i].GetValue(instance);
                        if (value is not null)
                        {
                            switch (value)
                            {
                                case bool v:
                                    ImGui.Checkbox(name, ref v);
                                    break;
                                case GameObject gameObject:
                                    ImGui.Text($"{name}: {value}");
                                    ImGui.Text($"UUID: {gameObject.GetID()}");
                                    ImGui.Text($"Tag: {gameObject.GetTag()}");
                                    break;

                                default:
                                    ImGui.Text($"{name}: {value}");
                                    break;
                            }
                        }
                        else
                        {
                            ImGui.Text($"{name}");
                        }
                        ImGui.Spacing();
                    }
                }
                
                ImGui.Spacing();
            }

            ImGui.End();
        }
    }
}
