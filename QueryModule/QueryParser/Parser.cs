using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    enum NodeType
    {

    }
    class Node
    {
        NodeType nodeType;
        List<Node> Children;
    }
    class ParserResult
    {
        List<Node> selectNodes;
        Node fromNode;
        Node whereNode;
    }
    class Parser
    {
        public static ParserResult parse(List<Token> tokens)
        {
            return null;
        }
    }
}
