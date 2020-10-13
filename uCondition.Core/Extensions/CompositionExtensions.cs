using uCondition.Models;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace uCondition.Core.Extensions
{
    public static class CompositionExtensions
    {
        public static Composition RegisterPredicate<TPredicate>(this Composition composition, Lifetime lifetime = Lifetime.Singleton)
            where TPredicate : Predicate
        {
            composition.Register<TPredicate>(lifetime);
            return composition;
        }

        public static Composition RegisterPredicate<TPredicate>(this Composition composition, TPredicate instance, Lifetime lifetime = Lifetime.Singleton)
            where TPredicate : Predicate
        {
            composition.Register(_ => instance, lifetime);
            return composition;
        }
    }
}