using System;
using VContainer;
using VContainer.Unity;

namespace Core.Networking.NetworkObjectsFactory
{
    public class FactoryContainerOverrider : IInitializable, IDisposable
    {
        private IObjectResolver _objectResolver;
        private IObjectsFactory _networkObjectsFactory;
        
        [Inject]
        public FactoryContainerOverrider(
            IObjectResolver objectResolver,
            IObjectsFactory networkObjectsFactory)
        {
            _objectResolver = objectResolver;
            _networkObjectsFactory = networkObjectsFactory;
        }

        public void Dispose()
        {
            _networkObjectsFactory.OvveridedContainer = null;
        }

        public void Initialize()
        {
            _networkObjectsFactory.OvveridedContainer = () => _objectResolver;
        }
    }
}
