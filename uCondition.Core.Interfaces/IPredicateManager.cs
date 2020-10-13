using System.Collections.Generic;
using uCondition.Models;

namespace uCondition.Core.Interfaces
{
    public interface IPredicateManager
    {
        IEnumerable<Predicate> GetPredicates(bool withPredicates = true);

        Predicate GetPredicate(string alias);
    }
}