using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class OfflineConnectingFailure<TStateId> : State<TStateId>
    {
        public override string Name => "OfflineConnectingFailure";
        public const float ReinitializationDelay = 10f;
        protected async override UniTask OnEnter()
        {
            Debug.Log($"Reinitializing relay in {ReinitializationDelay} seconds");
            await UniTask.WaitForSeconds(ReinitializationDelay);

            CompleteState();
        }
    }
}
