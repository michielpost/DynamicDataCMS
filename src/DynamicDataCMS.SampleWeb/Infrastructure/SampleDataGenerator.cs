using DynamicDataCms.Core.Models;
using DynamicDataCms.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicDataCms.Core.Services.Extensions;
using System.Dynamic;

namespace DynamicDataCms.SampleWeb.Infrastructure
{
    public class SampleDataGenerator
    {
        private readonly DataProviderWrapperService dataProviderService;
        private readonly JsonSchemaService jsonSchemaService;
        private readonly CmsTreeService cmsTreeService;

        public SampleDataGenerator(DataProviderWrapperService dataProviderService, JsonSchemaService jsonSchemaService, CmsTreeService cmsTreeService)
        {
            this.dataProviderService = dataProviderService;
            this.jsonSchemaService = jsonSchemaService;
            this.cmsTreeService = cmsTreeService;
        }

        public async Task ClearAndGenerate()
        {
            await ClearData();
            await CreateTestData();
        }

        public async Task ClearData()
        {
            var schemas = jsonSchemaService.GetCmsConfiguration();
            foreach(var menuItem in schemas.MenuItems)
            {
                if (menuItem.Key != null)
                {
                    var (allItems, _) = await dataProviderService.List(menuItem.Key, null, null);

                    foreach(var item in allItems)
                    {
                        await dataProviderService.Delete(menuItem.Key, item.Id, null, null);
                    }
                }
            }

            string cmsTreeType = PageTreeRoutes.PageTreeType;
            var tree = await cmsTreeService.GetCmsTreeItem(cmsTreeType, null);
            foreach(var item in tree.Nodes)
            {
                await cmsTreeService.ClearCmsTreeNode(cmsTreeType, item.NodeId, null, null);
            }


        }

        public async Task CreateTestData()
        {
            //Create a category
            var categoryId = await CreateCategory("Fruit");

            //Create a product
            await CreateProduct("Banana", 42);
            await CreateProduct("Apple", 3);
            await CreateProduct("Kiwi", 16);

            //Singleton page
            await CreateSingletonPage();


            string cmsTreeType = PageTreeRoutes.PageTreeType; ;
            var homepageId = await CreateTestPage("homepage", "Welcome to the sample homepage");
            await cmsTreeService.CreateOrUpdateCmsTreeNodeForSlug(cmsTreeType, "/", new CmsTreeNode {
                    CmsItemId = homepageId,
                    CmsItemType = "contentpage"
            }, null, null);

            var contentPageId = await CreateTestPage("test", "Welcome to a sample content page");
            await cmsTreeService.CreateOrUpdateCmsTreeNodeForSlug(cmsTreeType, "/test", new CmsTreeNode
            {
                CmsItemId = contentPageId,
                CmsItemType = "contentpage"
            }, null, null);

        }

        private async Task<Guid> CreateTestPage(string title, string intro)
        {
            dynamic page = new ExpandoObject();
            page.Id = Guid.NewGuid();
            page.CmsType = PageTreeRoutes.PageTreeType;
            page.title = title;
            page.intro = intro;

            await dataProviderService.Write<CmsItem>(((object)page).ToCmsItem(), page.CmsType, page.Id, null, null);

            return page.Id;
        }

        private async Task CreateSingletonPage()
        {
            dynamic page = new ExpandoObject();
            page.Id = Guid.NewGuid();
            page.CmsType = "singletonpage";
            page.title = "Product shop";
            page.intro = "Welcome to the shop page";

            await dataProviderService.Write<CmsItem>(((object)page).ToCmsItem(), page.CmsType, page.Id, null, null);
        }

        private async Task<Guid> CreateCategory(string name)
        {
            dynamic category = new ExpandoObject();
            category.Id = Guid.NewGuid();
            category.CmsType = "categoryitem";
            category.name = name;

            await dataProviderService.Write<CmsItem>(((object)category).ToCmsItem(), category.CmsType, category.Id, null, null);

            return category.Id;
        }

        private async Task CreateProduct(string name, int stock)
        {
            dynamic product = new ExpandoObject();
            product.Id = Guid.NewGuid();
            product.CmsType = "productitem";
            product.name = name;
            product.stock = stock;

            await dataProviderService.Write<CmsItem>(((object)product).ToCmsItem(), product.CmsType, product.Id, null, null);
        }
    }
}
