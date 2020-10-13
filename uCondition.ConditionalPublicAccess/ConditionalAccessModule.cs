﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using uCondition.ConditionalPublicAccess.Data;
using uCondition.ConditionalPublicAccess.Helpers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace uCondition.ConditionalPublicAccess
{
    public class ConditionalAccessModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(this.Context_BeginRequest);
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;

            UmbracoContext.EnsureContext((HttpContextBase)new HttpContextWrapper(context), ApplicationContext.Current, new WebSecurity((HttpContextBase)new HttpContextWrapper(context), ApplicationContext.Current));

            IEnumerable<int> ids;
            if (context.Request.Url.AbsolutePath.StartsWith("/media", true, CultureInfo.InvariantCulture))
            {
                var media = UmbracoContext.Current.Application.Services.MediaService.GetMediaByPath(context.Request.Url.AbsolutePath);
                if (media == null)
                    return;
                ids = media.Path.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => int.Parse(c));
            }
            else
            {
                IPublishedContent content = UmbracoContext.Current.ContentCache.GetByRoute(context.Request.Url.AbsolutePath, new bool?());
                if (content == null)
                    return;
                ids = content.Path.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => int.Parse(c));
            }

            if (ids == null && !ids.Any<int>())
                return;

            var protectedPage = new ProtectedPageProvider().LoadForPath(ids);

            if (protectedPage == null || ConditionalAccess.HasAccess(protectedPage))
                return;

            foreach (ProtectedPageCondition condition in protectedPage.Conditions)
            {
                if (ConditionalAccess.TestCondition(condition.Condition))
                    context.Response.Redirect("~" + new UmbracoHelper(UmbracoContext.Current).Url(condition.FalseActionNodeId) + "?returnUrl=" + context.Request.Url.AbsolutePath);
            }

            context.Response.Redirect("~" + new UmbracoHelper(UmbracoContext.Current).Url(protectedPage.FalseActionNodeId) + "?returnUrl=" + context.Request.Url.AbsolutePath);
        }
    }
}