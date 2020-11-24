﻿using Owin;
using System.Linq;
using uCondition.ConditionalPublicAccess.Data;
using uCondition.ConditionalPublicAccess.Middlewares;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace uCondition.ConditionalPublicAccess.Composers
{
    public class UConditionPublicAccessComponent : IComponent
    {
        private readonly IMediaService _mediaService;
        private readonly IDomainService _domainService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public UConditionPublicAccessComponent(IMediaService mediaService, IDomainService domainService, IUmbracoContextFactory umbracoContextFactory)
        {
            _mediaService = mediaService;
            _domainService = domainService;
            _umbracoContextFactory = umbracoContextFactory;
        }

        public void Initialize()
        {
            TreeControllerBase.TreeNodesRendering += ContentTreeController_TreeNodesRendering;
            TreeControllerBase.MenuRendering += ContentTreeController_MenuRendering;

            UmbracoDefaultOwinStartup.MiddlewareConfigured += (_, args) => ConfigureMiddleware(args.AppBuilder);
        }

        public void Terminate()
        {
            TreeControllerBase.TreeNodesRendering -= ContentTreeController_TreeNodesRendering;
            TreeControllerBase.MenuRendering -= ContentTreeController_MenuRendering;
        }

        private void ContentTreeController_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (sender.TreeAlias != "content" && sender.TreeAlias != "media")
            {
                return;
            }

            var menuItem = new MenuItem("uConditionAccess", "Conditional access")
            {
                Icon = "code",
                OpensDialog = true
            };

            menuItem.AdditionalData.Add("nodeId", e.NodeId);

            var protectMenuItemIndex = e.Menu.Items.FindIndex(x => x.Alias == "protect");
            e.Menu.Items.Insert(protectMenuItemIndex + 1, menuItem);
        }

        private void ContentTreeController_TreeNodesRendering(TreeControllerBase sender, TreeNodesRenderingEventArgs e)
        {
            if (sender.TreeAlias != "content" && sender.TreeAlias != "media")
            {
                return;
            }

            var protectedPages = new ProtectedPageProvider().Load();

            using (var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var node in e.Nodes)
                {
                    if (!int.TryParse((string)node.Id, out var nodeId))
                    {
                        continue;
                    }

                    var itemPath = sender.TreeAlias == "content"
                                   ? umbracoContextReference.UmbracoContext.Content.GetById(nodeId)?.Path
                                   : umbracoContextReference.UmbracoContext.Media.GetById(nodeId)?.Path;

                    if (itemPath == null || string.IsNullOrWhiteSpace(itemPath))
                    {
                        continue;
                    }

                    if (protectedPages.Pages.Any(c => c.Id == nodeId))
                    {
                        node.CssClasses.Add("uConditionAccess");
                    }
                    else if (protectedPages.Pages.Any(c => itemPath.IndexOf(c.Id.ToString()) >= 0))
                    {
                        node.CssClasses.Add("uConditionAccess");
                        node.CssClasses.Add("inheritedAccess");
                    }
                }
            }
        }

        private void ConfigureMiddleware(IAppBuilder appBuilder)
        {
            appBuilder.Use<ConditionalAccessMiddleware>(_umbracoContextFactory, _mediaService, _domainService);
        }
    }
}