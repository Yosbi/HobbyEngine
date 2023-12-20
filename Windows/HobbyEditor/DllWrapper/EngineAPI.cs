using HobbyEditor.Components;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace HobbyEditor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1,1,1);   
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
    }
}

namespace HobbyEditor.DllWrapper
{
    static class EngineAPI
    {
        private const string _dllName = "EngineDLL.dll";

        [DllImport(_dllName)]
        public static extern int CreateGameEntity(EngineAPIStructs.GameEntityDescriptor desc);
        public static int CreateGameEntity(GameEntity gameEntity)
        {
            var desc = new EngineAPIStructs.GameEntityDescriptor();

            // transform component
            {
                var component = gameEntity.GetComponent<Transform>();
                Debug.Assert(component != null);
                if (component != null)
                { 
                    desc.Transform.Position = component.Position;
                    desc.Transform.Rotation = component.Rotation;
                    desc.Transform.Scale = component.Scale;
                }

                return CreateGameEntity(desc);
            }
        }

        [DllImport(_dllName)]
        public static extern void RemoveGameEntity(int entityId);
        public static void RemoveGameEntity(GameEntity gameEntity)
        {
            RemoveGameEntity(gameEntity.EntityId);
        }
    }
}
