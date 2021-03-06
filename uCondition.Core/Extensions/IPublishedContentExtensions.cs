﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using uCondition.Core.Controllers;
using uCondition.Core.Models;
using uCondition.Core.Models.Converter;
using uCondition.ExpressionEngine;
using uCondition.Models;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace uCondition.Core.Extensions
{
    public static class IPublishedContentExtensions
    {
        public static bool IsExpressionTrue(this IPublishedContent content, string propertyName)
        {
            string json = content.GetPropertyValue<string>(propertyName);

            var modelConverter = new uConditionModelConverter();
            var model = modelConverter.Convert(json);

            if (model.PredicateGroups.Count >= 1 && model.PredicateGroups.First().Conditions.Any())
            {

                foreach (var swimlane in model.PredicateGroups)
                {
                    var compiler = new ExpressionCompiler();
                    var expressionTree = compiler.Compile(swimlane, new PredicateManager());
                    var analyser = new ExpressionAnalyser();
                    var result = analyser.Analyse(expressionTree);

                    return result;
                }
            }
            else
            {
                return true;
            }
            
            return false;
        }

        //public static HtmlString ExecuteExpression(this HtmlHelper htmlHelper, IPublishedContent content, string propertyName)
        //{
        //    string json = content.GetPropertyValue<string>(propertyName);

        //    var modelConverter = new uConditionModelConverter();
        //    var model = modelConverter.Convert(json);

        //    foreach (var swimlane in model.PredicateGroups)
        //    {
        //        var compiler = new ExpressionCompiler();
        //        var expressionTree = compiler.Compile(swimlane, new PredicateManager());
        //        var analyser = new ExpressionAnalyser();
        //        var result = analyser.Analyse(expressionTree);

        //        if (result)
        //        {
        //            var predicateManager = new PredicateManager();
        //            var action = predicateManager.GetAction(swimlane.Actions[0].Config.Alias);

        //            //render the bugger
        //            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
        //            var controllerContext = new GenericController();
        //            action.Do(new FieldValues(swimlane.Actions[0].Values.ToDictionary(k => k.Alias, k => k.Value)), htmlHelper);//.ExecuteResult(new ControllerContext(new HttpContextWrapper(HttpContext.Current), routeData, controllerContext));
        //        }
        //    }


        //    return new HtmlString("");
        //}
    }
}
