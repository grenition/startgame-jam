using JsonKnownTypes;
using Newtonsoft.Json;

namespace Core.Interactions
{
    [JsonConverter(typeof(JsonKnownTypesConverter<GameAction>))]
    public abstract class GameAction
    {
        public abstract void Execute();
    }
}
