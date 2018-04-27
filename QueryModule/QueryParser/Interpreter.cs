using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    class InterpreterException : Exception
    {
        public InterpreterException(string message) : base(message) { }
    }
    class Interpreter
    {
        internal ParserResult pr;
        internal List<List<Entity> > table, rows;
        internal List<List<string> > ret;
        internal Interpreter(ParserResult PR)
        {
            pr = PR;
            rows = new List<List<Entity> >();
            ret = new List<List<string> >();
        }

        void getTable()
        {
            // call from xml
        }
        bool assertBinaryNode(Node cur)
        {
            foreach(Node child in cur.Children)
                if(!assertBinary(child))
                    throw new InterpreterException("Cannot apply " + cur.originalToken.lexeme + "operator to strings");
            return true;
        }
        bool assertAndOr(Node child)
        {
            return (child.nodeType != NodeType.STRING && child.nodeType != NodeType.WHERE &&
                child.nodeType != NodeType.SELECT && child.nodeType != NodeType.LIST &&
                child.nodeType != NodeType.FROM && child.nodeType != NodeType.LIST);
        }
        bool assertBinary(Node child)
        {
            return (child.nodeType != NodeType.STRING && child.nodeType != NodeType.WHERE &&
                child.nodeType != NodeType.SELECT && child.nodeType != NodeType.LIST &&
                child.nodeType != NodeType.FROM && child.nodeType != NodeType. IN &&
                child.nodeType != NodeType.LIST);
        }
        Entity excuteWhere(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            return excute(cur.Children[0], r, map);
        }
        Entity excuteAND(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (int)(excute(cur.Children[0], r, map).valueN) & (int)(excute(cur.Children[1], r, map).valueN);
            if (ret.valueN != 0.0) ret.valueN = 1;
            return ret;
        }
        Entity excuteOR(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (int)(excute(cur.Children[0], r, map).valueN) | (int)(excute(cur.Children[1], r, map).valueN);
            if (ret.valueN != 0.0) ret.valueN = 1;
            return ret;
        }
        Entity excutePlus(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (excute(cur.Children[0], r, map).valueN) + (excute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity excuteMinus(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (excute(cur.Children[0], r, map).valueN) - (excute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity excuteMul(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (excute(cur.Children[0], r, map).valueN) * (excute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity excuteDiv(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (excute(cur.Children[0], r, map).valueN) / (excute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity excuteEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            Entity ret = new Entity("", Type.NUM, 0.0);
            if(cur.Children[0].nodeType == NodeType.STRING && cur.Children[1].nodeType == NodeType.STRING)
            {
                if (cur.Children[0].originalToken.lexeme == cur.Children[1].originalToken.lexeme)
                    ret.valueN = 1;
                return ret;
            }
            if (!assertBinaryNode(cur))
                return null;
            bool x = (excute(cur.Children[0], r, map).valueN) == (excute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity excuteNotEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            Entity ret = new Entity("", Type.NUM, 0.0);
            if (cur.Children[0].nodeType == NodeType.STRING && cur.Children[1].nodeType == NodeType.STRING)
            {
                if (cur.Children[0].originalToken.lexeme != cur.Children[1].originalToken.lexeme)
                    ret.valueN = 1;
                return ret;
            }
            if (!assertBinaryNode(cur))
                return null;
            bool x = (excute(cur.Children[0], r, map).valueN) != (excute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity excuteLess(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (excute(cur.Children[0], r, map).valueN) < (excute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity excuteLessEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (excute(cur.Children[0], r, map).valueN) <= (excute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity excuteMore(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (excute(cur.Children[0], r, map).valueN) > (excute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity excuteMoreEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (excute(cur.Children[0], r, map).valueN) >= (excute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity excuteNot(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = excute(cur.Children[0], r,map);
            if (ret.valueN == 0.0) ret.valueN = 1;
            else ret.valueN = 0;
            return ret;
        }
        Entity excuteID(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            return map[cur.originalToken.lexeme];
        }
        Entity excuteNum(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            return new Entity(cur.originalToken.lexeme, Type.NUM, double.Parse(cur.originalToken.lexeme, System.Globalization.CultureInfo.InvariantCulture));
        }
        Entity excuteList(Node cur, List<Entity> r, Dictionary<string, Entity> map, double v)
        {
            Entity ret = new Entity("", Type.NUM, 0.0);
            foreach(Node child in cur.Children)
            {
                if (excute(child, r, map).valueN == v)
                {
                    ret.valueN = 1;
                    break;
                }
            }
            return ret;
        }
        Entity excuteIN(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {           
            Entity left = excute(cur.Children[0], r, map);
            return excuteList(cur.Children[1], r, map, left.valueN);
        }
        Entity excuteString(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            Entity ret = new Entity("", Type.STRING, cur.originalToken.lexeme);
            return ret;
        }
        Entity excute(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (cur.originalToken.lexeme == "-")
                return excuteMinus(cur, r, map);
            else if (cur.originalToken.lexeme == "+")
                return excutePlus(cur, r, map);
            else if (cur.originalToken.lexeme == "*")
                return excuteMul(cur, r, map);
            else if (cur.originalToken.lexeme == "/")
                return excuteDiv(cur, r, map);

            else if (cur.originalToken.lexeme == "<")
                return excuteLess(cur, r, map);
            else if (cur.originalToken.lexeme == "<=")
                return excuteLessEq(cur, r, map);
            else if (cur.originalToken.lexeme == ">")
                return excuteMore(cur, r, map);
            else if (cur.originalToken.lexeme == ">=")
                return excuteMoreEq(cur, r, map);

            else if (cur.originalToken.lexeme == "and")
                return excuteAND(cur, r, map);
            else if (cur.originalToken.lexeme == "or")
                return excuteOR(cur, r, map);

            else if (cur.originalToken.lexeme == "where")
                return excuteWhere(cur, r, map);

            else if (cur.originalToken.lexeme == "=")
                return excuteEq(cur, r, map);
            else if (cur.originalToken.lexeme == "!=")
                return excuteNotEq(cur, r, map);
            else if (cur.originalToken.lexeme == "!")
                return excuteNot(cur, r, map);

            else if (cur.nodeType == NodeType.NUMBER)
                return excuteNum(cur, r, map);
            else if (cur.nodeType == NodeType.ID)
                return excuteID(cur, r, map);
            else if (cur.nodeType == NodeType.STRING)
                return excuteString(cur, r, map);
            return null;
        }
        Dictionary<string, Entity> initMap(List<Entity> r)
        {
            Dictionary<string, Entity> ret = new Dictionary<string, Entity>();
            foreach (Entity i in r)
                ret[i.name] = i;
            return ret;
        }


        
        void selectRows()
        {
            Dictionary<string, Entity> map;
            foreach(List<Entity> r in table)
            {
                map = initMap(r);
                Entity x = excute(pr.whereNode, r, map);
                if(x.valueN != 0.0)
                    rows.Add(r);
            }
        }
        void getResult()
        {

        }
    }
}
