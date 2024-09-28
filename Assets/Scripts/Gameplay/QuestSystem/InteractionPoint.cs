using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using Core.Interactions;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Gameplay.QuestSystem
{
    [Serializable]
    public class ActionWrapper
    {
        public InteractionSendTo sendTo;
        [SerializeReference] public GameAction action;
    }
    
    [Serializable]
    public class ConditionWrapper
    {
        [FormerlySerializedAs("conditions")]
        [SerializeReference] public GameCondition condition;
    }

    public enum ConditionConnection
    {
        And,
        Or
    }
    
    [Serializable]
    public class ConditionsSet
    {
        public List<ConditionsSet> dependentConditionsSet;
        public List<ConditionWrapper> conditions;
        public ConditionConnection connection;
    }
    
    public class InteractionPoint : MonoBehaviour
    {
        [SerializeField] private ConditionsSet _conditions;
        [SerializeField] private List<ActionWrapper> _actions;

        private IInteractionService _interactionService;
        [Inject]
        private void Construct(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public bool CheckConditions()
        {
            return CheckConditions(_conditions);
        }

        public bool CheckConditions(ConditionsSet conditionsSet)
        {
            var result = false;

            switch (conditionsSet.connection)
            {
                case ConditionConnection.And:
                    result = conditionsSet.conditions
                        .Where(x => x != null && x.condition != null)
                        .All(x => _interactionService.GetCondition(x.condition));

                    if (conditionsSet.dependentConditionsSet.Count != 0 && result)
                        result = conditionsSet.dependentConditionsSet.All(x => CheckConditions(x));

                    break;
                case ConditionConnection.Or:
                    result = conditionsSet.conditions
                        .Where(x => x != null && x.condition != null)
                        .Any(x => _interactionService.GetCondition(x.condition));

                    if (conditionsSet.dependentConditionsSet.Count != 0 && !result)
                        result = conditionsSet.dependentConditionsSet.Any(x => CheckConditions(x));

                    break;
            }

            return result;
        }

        [Button]
        public void Interact()
        {
            if (CheckConditions(_conditions))
            {
                foreach (var actionWrapper in _actions)
                {
                    if (actionWrapper.action != null)
                        _interactionService.DoAction(actionWrapper.action, actionWrapper.sendTo);
                }
            }
        }
    }
}
