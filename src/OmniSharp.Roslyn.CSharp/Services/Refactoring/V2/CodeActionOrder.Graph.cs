using Microsoft.CodeAnalysis.CodeActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSharp.Roslyn.CSharp.Services.Refactoring.V2
{
    class Graph
    {
        private Dictionary<string, List<CodeActionNode>> Nodes;
        public Graph(List<CodeActionNode> nodesList)
        {
            Nodes = new Dictionary<string, List<CodeActionNode>>();
            foreach (CodeActionNode node in nodesList)
            {
                if (! Nodes.ContainsKey(node.ProviderName))
                    Nodes[node.ProviderName] = new List<CodeActionNode>();
                Nodes[node.ProviderName].Add(node);
            }

            foreach(CodeActionNode node in nodesList)
            {
                foreach(var before in node.Before)
                {
                    if (Nodes.ContainsKey(before))
                    {
                        foreach( var beforeNode in Nodes[before])
                        {
                            beforeNode.NodesBeforeMeSet.Add(node);
                        }
                    }
                }

                foreach (var after in node.After)
                {
                    if (Nodes.ContainsKey(after))
                    {
                        foreach (var afterNode in Nodes[after])
                        {
                            node.NodesBeforeMeSet.Add(afterNode);
                        }
                    }     
                }
            } 
        }

        public List<CodeAction> TopologicalSort()
        {
            List<CodeAction> result = new List<CodeAction>();
            var seenNodes = new HashSet<CodeActionNode>();

            foreach(var nodes in Nodes.Values)
            {
                foreach(var node in nodes)
                    Visit(node, result, seenNodes);
            }

            return result;
        }

        private void Visit(CodeActionNode node, List<CodeAction> result, HashSet<CodeActionNode> seenNodes)
        {
            if(seenNodes.Add(node))
            {
                foreach (var before in node.NodesBeforeMeSet)
                {
                    Visit(before, result, seenNodes);
                }
                result.Add(node.CodeAction);
            }
        }
    }
}
