using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QMS.Core.Models;

namespace QMS.Core.Services
{
    public class CmsTreeService
    {
        private readonly DataProviderWrapperService dataProvider;

        public CmsTreeService(DataProviderWrapperService dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public Task<CmsTreeItem?> GetCmsTreeItem(string cmsTreeType, string? lang)
        {
            return dataProvider.Read<CmsTreeItem>(cmsTreeType, Guid.Empty, lang);
        }

        public async Task<List<CmsTreeNode>> GetCmsTreeNodes(string cmsTreeType, Guid id, string? lang)
        {
            var treeItem = await GetCmsTreeItem(cmsTreeType, lang).ConfigureAwait(false);
            if (treeItem == null || treeItem.Root == null)
                return new List<CmsTreeNode>();

            return FindChildNodes(treeItem.Root, id);
        }

        public async Task<CmsTreeNode?> GetCmsTreeNode(string cmsTreeType, string slug, string? lang)
        {
            var treeItem = await GetCmsTreeItem(cmsTreeType, lang).ConfigureAwait(false);
            if (treeItem == null || treeItem.Root == null)
                return null;

            var slugParts = slug.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (!slugParts.Any())
                return treeItem.Root;

            var childNode = FindChildNode(treeItem.Root, slugParts);
            
            return childNode;
        }

        private CmsTreeNode? FindChildNode(CmsTreeNode node, IEnumerable<string> slugParts)
        {
            if (!slugParts.Any())
                return node;

           var currentPart = slugParts.First();
           var childNode = node.Children.Where(x => x.Name == currentPart).FirstOrDefault();

            if (childNode == null)
                return null;

            return FindChildNode(childNode, slugParts.Skip(1));
        }

        private List<CmsTreeNode> FindChildNodes(CmsTreeNode node, Guid id)
        {
            var result = new List<CmsTreeNode>();

            if (node.CmsItemId == id)
                result.Add(node);

            foreach (var child in node.Children)
            {
                result.AddRange(FindChildNodes(child, id));
            }

            return result;
        }
    }
}
