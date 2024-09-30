using Core.Interactions;
using System;
using System.Runtime.Serialization;

namespace Gameplay.QuestSystem.Actions
{
    [DataContract, Serializable]
    public class OpenSettingsAction : GameAction
    {
        public override void Execute()
        {
            UnityEngine.Object.FindObjectOfType<SettingsModel>()?.Open();
        }
    }
}