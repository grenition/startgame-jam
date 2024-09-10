using VContainer;
using VContainer.Unity;

namespace Core.Networking.NetworkObjectsFactory
{
    public class FactoryContainerOverrider : IInitializable
    {
        private IObjectResolver _objectResolver;
        private INetworkObjectsFactory _networkObjectsFactory;
        
        [Inject]
        public FactoryContainerOverrider(
            IObjectResolver objectResolver,
            INetworkObjectsFactory networkObjectsFactory)
        {
            _objectResolver = objectResolver;
            _networkObjectsFactory = networkObjectsFactory;
        }
        
        public void Initialize()
        {
            _networkObjectsFactory.OvveridedContainer = () => _objectResolver;
        }
    }
}
