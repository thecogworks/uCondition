﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uCondition.Interfaces;
using uCondition.Models;

namespace uCondition.Core.Models
{
    public class GlobalPredicate : uCondition.Models.Predicate
    {
        protected GlobalConditionModel Model { get; }

        public GlobalPredicate(GlobalConditionModel model)
        {
            this.Name = model.Name;
            this.Alias = model.Guid;
            this.Category = "Global Conditions";
            this.Icon = "icon-umb-translation";
            this.Fields = new List<EditableProperty>();
            this.Model = model;
        }

        public override bool Validate(IFieldValues fieldValues)
        {
            var expressionCompiler = new ExpressionEngine.ExpressionCompiler();
            var compiledExpression = expressionCompiler.Compile(Model.Condition.PredicateGroups.First(), new PredicateManager());
            var expressionAnalyser = new ExpressionEngine.ExpressionAnalyser();
            return expressionAnalyser.Analyse(compiledExpression);
        }
    }
}
