using JsonKnownTypes;
using Newtonsoft.Json;

namespace Core.Interactions
{
    [JsonConverter(typeof(JsonKnownTypesConverter<GameCondition>))]
    public abstract class GameCondition
    {
        public abstract bool Execute();
    }
}
