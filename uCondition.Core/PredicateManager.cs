using System.Collections.Generic;
using System.Linq;
using uCondition.Core.Data;
using uCondition.Core.Interfaces;
using uCondition.Models;

namespace uCondition.Core
{
    public class PredicateManager : IPredicateManager
    {
        private readonly IGlobalConditionsRepository _globalConditionsRepository;
        private readonly IDependencyResolver _dependencyResolver;

        public PredicateManager(IGlobalConditionsRepository globalConditionsRepository, IDependencyResolver dependencyResolver)
        {
            _globalConditionsRepository = globalConditionsRepository;
            _dependencyResolver = dependencyResolver;
        }

        public IEnumerable<Predicate> GetPredicates(bool withGlobalPredicates = true)
        {
            var predicates = _dependencyResolver.ResolveMany<Predicate>();

            return withGlobalPredicates
                ? predicates.Concat(_globalConditionsRepository
                    .GetAll()
                    .Select(c => new Models.GlobalPredicate(Models.Mappers.DataToModel(c)))
                    .ToList())
                : predicates.ToList();
        }

        public Predicate GetPredicate(string alias)
        {
            var predicates = GetPredicates();

            var predicate = predicates.FirstOrDefault(p => p.Alias == alias);

            if (predicate == null)
            {
                var globalCondition = _globalConditionsRepository.GetSingle(alias);

                if (globalCondition != null)
                {
                    predicate = new Models.GlobalPredicate(Models.Mappers.DataToModel(globalCondition));
                }
            }

            return predicate;
        }
    }
}