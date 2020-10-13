﻿using LightInject;
using uCondition.Core.Components;
using uCondition.Core.Data;
using uCondition.Core.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace uCondition.Core.Composers
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class UConditionComposer : ComponentComposer<UConditionComponent>
    {
        public override void Compose(Composition composition)
        {
            base.Compose(composition);

            var lightInjectContainer = composition.Concrete as IServiceContainer;

            composition.Register(_ => lightInjectContainer);

            composition.Register<IDependencyResolver, DependencyResolver>();

            composition.RegisterUnique<IGlobalConditionsRepository, GlobalConditionsRepository>();
            composition.RegisterUnique<IRegisteredPredicateRepository, RegisteredPredicateRepository>();

            composition.Register<IPredicateManager, PredicateManager>();
        }
    }
}