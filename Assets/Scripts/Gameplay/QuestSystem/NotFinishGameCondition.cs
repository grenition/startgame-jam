using Core.Interactions;
using System;
using System.Runtime.Serialization;
using UnityEngine;
using VContainer;

namespace Gameplay.QuestSystem.Conditions
{
    [DataContract, Serializable]
    public class NotFinishGameCondition : GameCondition
    {
        [DataMember, SerializeField] private ActivityInfo _activity;

        private CompletedTasks _tasks;

        [Inject]
        private void Construct(CompletedTasks tasks)
        {
            _tasks = tasks;
        }

        public override bool Execute()
        {
            return !_tasks.Tasks.Contains(_activity);
        }
    }
}
