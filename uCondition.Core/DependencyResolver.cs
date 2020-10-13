using LightInject;
using System.Collections.Generic;
using System.Linq;

namespace uCondition.Core
{
    public interface IDependencyResolver
    {
        TService Resolve<TService>();

        IEnumerable<TService> ResolveMany<TService>();
    }

    public class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceContainer _container;

        public DependencyResolver(IServiceContainer container)
            => _container = container;

        public TService Resolve<TService>()
        {
            using (var scope = _container.BeginScope())
            {
                return scope.TryGetInstance<TService>();
            }
        }

        public IEnumerable<TService> ResolveMany<TService>()
        {
            using (var scope = _container.BeginScope())
            {
                return scope.TryGetInstance<IEnumerable<TService>>()
                       ?? Enumerable.Empty<TService>();
            }
        }
    }
}