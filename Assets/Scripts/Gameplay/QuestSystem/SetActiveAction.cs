using Core.Interactions;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Gameplay.QuestSystem.Actions
{
    [Serializable, DataContract]
    public class SetActiveAction : GameAction
    {
        [SerializeField, DataMember] private GameObject _obj;
        [SerializeField, DataMember] private bool _active;

        public override void Execute()
        {
            _obj.SetActive(_active);
        }
    }
}
