using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using uCondition.Core.Interfaces;
using uCondition.Interfaces;
using uCondition.Models;

namespace uCondition.Core.Models
{
    public class GlobalPredicate : uCondition.Models.Predicate
    {
        protected GlobalConditionModel Model { get; }

        public GlobalPredicate(GlobalConditionModel model)
        {
            Name = model.Name;
            Alias = model.Guid;
            Category = "Global Conditions";
            Icon = "icon-umb-translation";
            Fields = new List<EditableProperty>();
            Model = model;
        }

        public override bool Validate(IFieldValues fieldValues)
        {
            var predicateManager = System.Web.Mvc.DependencyResolver.Current.GetService<IPredicateManager>();
            var expressionCompiler = new ExpressionEngine.ExpressionCompiler();
            var compiledExpression = expressionCompiler.Compile(Model.Condition.PredicateGroups.First(), predicateManager);
            var expressionAnalyser = new ExpressionEngine.ExpressionAnalyser();
            return expressionAnalyser.Analyse(compiledExpression);
        }
    }
}