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

        public async Task<CmsTreeItem?> CreateOrUpdateCmsTreeNode(string cmsTreeType, string slug, CmsTreeNode node, string? lang, string? currentUser)
        {
            var document = await dataProvider.Read<CmsTreeItem>(cmsTreeType, Guid.Empty, lang);
            document ??= new CmsTreeItem();

            List<string> slugParts = GetSlugList(slug);

            document.Root = CreateOrUpdateNode(document.Root, slugParts, node);

            await dataProvider.Write<CmsTreeItem>(document, cmsTreeType, Guid.Empty, lang, currentUser);

            return document;
        }

        private static List<string> GetSlugList(string slug)
        {
            var slugParts = slug.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            slugParts.Insert(0, "/");
            return slugParts;
        }

        private CmsTreeNode CreateOrUpdateNode(CmsTreeNode? currentNode, IEnumerable<string> slugParts, CmsTreeNode nodeToAdd)
        {
            if (currentNode == null)
                currentNode = new CmsTreeNode { Name = slugParts.FirstOrDefault() };

            var newSlug = slugParts.Skip(1);

            if (newSlug.Any())
            {
                CmsTreeNode? childNode = currentNode.Children.Where(x => x.Name == newSlug.FirstOrDefault()).FirstOrDefault();
                CmsTreeNode newChildNode = CreateOrUpdateNode(childNode, newSlug, nodeToAdd);

                if(childNode == null)
                    currentNode.Children.Add(newChildNode);
            }
            else
            {
                currentNode = nodeToAdd;
                currentNode.Name = slugParts.FirstOrDefault();
            }

            return currentNode;
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

            List<string> slugParts = GetSlugList(slug);

            if (!slugParts.Any())
                return treeItem.Root;

            var childNode = FindChildNode(treeItem.Root, slugParts);
            
            return childNode;
        }

        private CmsTreeNode? FindChildNode(CmsTreeNode node, IEnumerable<string> slugParts)
        {
            if (!slugParts.Any())
                return null;

           var currentPart = slugParts.First();
            if (node.Name == currentPart)
            {
                var nextSlugs = slugParts.Skip(1);
                if(nextSlugs.Any())
                {
                    var childNode = node.Children.Where(x => x.Name == nextSlugs.First()).FirstOrDefault();

                    if (childNode == null)
                        return null;

                    return FindChildNode(childNode, nextSlugs);
                }

                return node;
            }

            return null;
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
