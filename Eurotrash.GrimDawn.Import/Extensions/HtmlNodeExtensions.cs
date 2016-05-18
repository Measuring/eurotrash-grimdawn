using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Eurotrash.GrimDawn.Import.Extensions
{
    /// <summary>
    /// Additional helper functions for HtmlAgilityPack.
    /// </summary>
    internal static class HtmlNodeExtensions
    {
        internal static IEnumerable<HtmlNode> Find(this HtmlNode node, string element)
        {
            return node.Descendants().Where(item => item.Name == element);
        }

        internal static IEnumerable<HtmlNode> NextSiblings(this HtmlNode node)
        {
            while(node.NextSibling != null)
            {
                node = node.NextSibling;

                yield return node;
            }
        }

        internal static IEnumerable<HtmlNode> PreviousSiblings(this HtmlNode node)
        {
            while (node.PreviousSibling != null)
            {
                node = node.PreviousSibling;

                yield return node;
            }
        }
    }
}
