using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using QMS.Core.Models;
using QMS.Core.Services;
using QMS.Storage.Interfaces;

namespace QMS.Core.Web.Helpers
{
	public abstract class CmsTreeRoutes : DynamicRouteValueTransformer
	{
		private readonly CmsTreeService cmsTreeService;
		private readonly IReadCmsItem readCmsItemService;

		public abstract string CmsTreeType { get; }

		public CmsTreeRoutes(CmsTreeService cmsTreeService, DataProviderWrapperService dataProviderService)
		{
			this.cmsTreeService = cmsTreeService;
			this.readCmsItemService = dataProviderService;
		}

		public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
		{
			var displayUrl = httpContext.Request.GetDisplayUrl();
			var uri = ConvertStringToUri(displayUrl);
			if (uri != null)
			{
				CmsTreeNode? cmsTreeNode = await cmsTreeService.GetCmsTreeNode(CmsTreeType, uri.AbsolutePath, null).ConfigureAwait(false);

				if (cmsTreeNode != null && cmsTreeNode.CmsItemId.HasValue)
				{
					var cmsItem = await readCmsItemService.Read<CmsItem>(CmsTreeType, cmsTreeNode.CmsItemId.Value, null).ConfigureAwait(false);

					if (cmsItem != null)
					{
						return new RouteValueDictionary()
							{
								{  "controller", cmsTreeNode.CmsItemType },
								{  "action", "Index" },
								{  "cmsItem", cmsItem },
							};
					}
				}
			}

			return new RouteValueDictionary();
		}

		private Uri? ConvertStringToUri(string urlOrPath)
		{
			Uri? uri;
			if (Uri.TryCreate(urlOrPath, UriKind.RelativeOrAbsolute, out uri))
			{
				if (!uri.IsAbsoluteUri)
					uri = new Uri(new Uri("http://localhost"), uri);
			}

			return uri;
		}
	}

	public class TestModel
	{
		public string Name { get; set; }
	}
}
