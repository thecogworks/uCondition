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
        private readonly IEnumerable<Predicate> _predicates;

        public PredicateManager(IGlobalConditionsRepository globalConditionsRepository, IEnumerable<Predicate> predicates)
        {
            _globalConditionsRepository = globalConditionsRepository;
            _predicates = predicates;
        }

        public IEnumerable<Predicate> GetPredicates(bool withGlobalPredicates = true)
        {
            return withGlobalPredicates
                ? _predicates.Concat(_globalConditionsRepository
                    .GetAll()
                    .Select(c => new Models.GlobalPredicate(Models.Mappers.DataToModel(c)))
                    .ToList())
                : _predicates.ToList();
        }

        public Predicate GetPredicate(string alias)
        {
            var predicate = _predicates.FirstOrDefault(p => p.Alias == alias);

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

        //public List<IAction> GetActions()
        //{
        //    return ActionConfigs;
        //}

        //public IAction GetAction(string alias)
        //{
        //    var action = ActionConfigs.FirstOrDefault(a => a.Alias == alias);

        //    return action != null ? (uCondition.Models.Action)Activator.CreateInstance(action.GetType()) : null;
        //}
    }
}