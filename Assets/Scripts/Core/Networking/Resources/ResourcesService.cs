using System.Collections.Generic;
using System.Linq;
using Core.SceneManagement;
using UnityEngine;
using VContainer.Unity;

namespace Core.Networking.Settings
{
    public class ResourcesService : IInitializable
    {
        public NetworkSettings NetworkSettings { get; private set; }
        public Dictionary<string, SceneConfig> SceneConfigs { get; private set; } = new();
        public SceneTransitionsConfig SceneTransitions { get; private set; }
        
        public void Initialize()
        {
            NetworkSettings = Resources.Load<NetworkSettings>("Core/NetworkSettings");
            SceneConfigs = Resources.LoadAll<SceneConfig>("Data/Scenes")
                .ToDictionary(x => x.Scene, x => x);
            SceneTransitions = Resources.Load<SceneTransitionsConfig>("Data/SceneTransitionsConfig");
        }
    }
}
