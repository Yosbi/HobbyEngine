using System.Diagnostics;
using System.Runtime.Serialization;

namespace HobbyEditor.Components
{
    [DataContract]
    public class Component : Common.ViewModelBase
    {
        [DataMember]
        public GameEntity Owner { get; private set; }

        public Component(GameEntity owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }
}
