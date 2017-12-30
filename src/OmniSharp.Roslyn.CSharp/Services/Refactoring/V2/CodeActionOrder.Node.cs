using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;


namespace OmniSharp.Roslyn.CSharp.Services.Refactoring.V2
{
    class CodeActionNode
    {
        public CodeAction CodeAction { get; set; }
        public List<string> Before { get; set; }
        public List<string> After { get; set; }
        public string ProviderName { get; set; }
        public HashSet<CodeActionNode> NodesBeforeMeSet { get; set; }
        
        public CodeActionNode(CodeAction codeAction, string providerName)
        {
            CodeAction = codeAction;
            ProviderName = providerName;
            Before = new List<string>();
            After = new List<string>();
            NodesBeforeMeSet = new HashSet<CodeActionNode>();
        }

        public void AddAttribute(ExtensionOrderAttribute attribute)
        {
            if(attribute.Before != null)
                Before.Add(attribute.Before);
            if (attribute.After != null)
                After.Add(attribute.After);
        }
    }
}
