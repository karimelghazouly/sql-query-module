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
    class EmptyQueryException : Exception
    {
        public EmptyQueryException(string message) : base(message) { }
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

        public void getTable()
        {
            table = QueryModule.FileManager.XMLParser.getTable(pr.fromNode.Children[0].originalToken.lexeme);
        }
        #region Where
        bool assertBinaryNode(Node cur)
        {
            foreach(Node child in cur.Children)
            {
                if(cur.originalToken.tokenType == TokenType.AND || cur.originalToken.tokenType == TokenType.OR)
                {
                    if (!assertAndOr(child))
                        throw new InterpreterException("Cannot apply " + cur.originalToken.lexeme + "operator to strings");
                    continue;
                }
                if(!assertBinary(child))
                    throw new InterpreterException("Cannot apply " + cur.originalToken.lexeme + "operator to strings");
            }
            return true;
        }
        bool assertAndOr(Node child)
        {
            return (child.nodeType != NodeType.STRING && child.nodeType != NodeType.WHERE &&
                child.nodeType != NodeType.SELECT && child.nodeType != NodeType.LIST &&
                child.nodeType != NodeType.FROM);
        }
        bool assertBinary(Node child)
        {
            return (child.nodeType != NodeType.STRING && child.nodeType != NodeType.WHERE &&
                child.nodeType != NodeType.SELECT && child.nodeType != NodeType.LIST &&
                child.nodeType != NodeType.FROM && child.nodeType != NodeType. IN);
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
            bool x1 = Math.Abs(excute(cur.Children[0], r, map).valueN) < 1e-8;
            bool x2 = Math.Abs(excute(cur.Children[1], r, map).valueN) < 1e-8;
            if (x1 && x2)
                ret.valueN = 1.0;
            else
                ret.valueN = 0.0;           
            return ret;
        }
        Entity excuteOR(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x1 = Math.Abs(excute(cur.Children[0], r, map).valueN) < 1e-8;
            bool x2 = Math.Abs(excute(cur.Children[1], r, map).valueN) < 1e-8;
            if (x1 || x2)
                ret.valueN = 1.0;
            else
                ret.valueN = 0.0;
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
            if (map.ContainsKey(cur.originalToken.lexeme))
                return map[cur.originalToken.lexeme];
            else
                throw new EmptyQueryException(cur.originalToken.lexeme + "is not a valid column");
        }
        Entity excuteNum(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            return new Entity(cur.originalToken.lexeme, Type.NUM, double.Parse(cur.originalToken.lexeme, System.Globalization.CultureInfo.InvariantCulture));
        }
        Entity excuteList(Node cur, List<Entity> r, Dictionary<string, Entity> map, Entity v)
        {
            Entity ret = new Entity("", Type.NUM, 0.0);            
            foreach(Node child in cur.Children)
            {
                Entity x = excute(child, r, map);
                if (x.type != v.type)
                    continue;                
                if(x.type == Type.STRING && x.valueS == v.valueS)
                {
                    ret.valueN = 1;
                    break;
                }
                else if (x.type == Type.NUM && x.valueN == v.valueN)
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
            return excuteList(cur.Children[1], r, map, left);
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
            else if (cur.originalToken.lexeme == "in")
                return excuteIN(cur, r, map);
            else if (cur.nodeType == NodeType.NUMBER)
                return excuteNum(cur, r, map);
            else if (cur.nodeType == NodeType.ID)
                return excuteID(cur, r, map);
            else if (cur.nodeType == NodeType.STRING)
                return excuteString(cur, r, map);
            throw new InterpreterException("Unexpected token " + cur.originalToken.lexeme);
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
            foreach (List<Entity> r in table)
            {
                map = initMap(r);
                Entity x = excute(pr.whereNode, r, map);
                if (x.valueN != 0.0)
                    rows.Add(r);
            }
        }
        #endregion
        #region Select
        bool assertBinaryNodeS(Node cur)
        {
            foreach (Node child in cur.Children)
            {
                if (!assertBinaryS(child))
                    throw new InterpreterException("Cannot apply " + cur.originalToken.lexeme + "operator to strings");
            }
            return true;
        }
        bool assertBinaryS(Node child)
        {
            return (child.nodeType != NodeType.STRING && child.nodeType != NodeType.WHERE &&
                child.nodeType != NodeType.SELECT && child.nodeType != NodeType.LIST &&
                child.nodeType != NodeType.FROM && child.nodeType != NodeType.IN&&!child.originalToken.isLogical()
                &&!child.originalToken.isComparison());
        }
        List <List<Entity> >excuteSelect(Node cur, Dictionary<string, List<Entity> >map)
        {
            List<List<Entity> >ret = new List< List<Entity> >();
            for (int i = 0; i < cur.Children.Count; i++)           
                ret.Add(excuteS(cur.Children[i], map));
            // transpose
            List<List<Entity> > rett = new List<List<Entity> >();
            for (int i = 0; i < rows.Count; i++)
            {
                rett.Add(new List<Entity>());
            }
            for(int i = 0; i < rows.Count; i++)
            {
                for(int j = 0; j < ret.Count; j++)
                {                    
                    rett[i].Add(ret[j][i]);
                }
            }
            return rett;
        }

        List<Entity> excutePlusS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = excuteS(cur.Children[0], map);
            List<Entity> right = excuteS(cur.Children[1], map);
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < left.Count; i++)
            {
                Entity neww = new Entity("", Type.NUM, left[i].valueN + right[i].valueN);
                ret.Add(neww);
            }
            return ret;
        }
        List<Entity> sum(Node cur, Dictionary<string, List<Entity>> map)
        {
            Entity ret = new Entity("", Type.NUM, 0);
            List<Entity> l = excuteS(cur.Children[0], map);           
            foreach (var i in l)
                ret.valueN += i.valueN;
            List<Entity> list = new List<Entity>();
            for (int i = 0; i < l.Count; i++)
                list.Add(ret);
            return list;
        }
        List<Entity> avg(Node cur, Dictionary<string, List<Entity>> map)
        {
            Entity ret = new Entity("", Type.NUM, 0);
            List<Entity> l = excuteS(cur.Children[0], map);
            foreach (var i in l)
                ret.valueN += i.valueN;
            ret.valueN /= l.Count;
            List<Entity> list = new List<Entity>();
            for (int i = 0; i < l.Count; i++)
            {
                list.Add(ret);
            }
            return list;
        }
        List<Entity> min(Node cur, Dictionary<string, List<Entity>> map)
        {
            Entity ret = new Entity("", Type.NUM, 1000000000.0);
            List<Entity> l = excuteS(cur.Children[0], map);
            foreach (var i in l)            
                if (ret.valueN > i.valueN)
                    ret.valueN = i.valueN;            
            List<Entity> list = new List<Entity>();
            for (int i = 0; i < l.Count; i++)
            {
                list.Add(ret);
            }
            return list;
        }
        List<Entity> max(Node cur, Dictionary<string, List<Entity>> map)
        {
            Entity ret = new Entity("", Type.NUM, -1000000000.0);
            List<Entity> l = excuteS(cur.Children[0], map);
            foreach (var i in l)
                if (ret.valueN < i.valueN)
                    ret.valueN = i.valueN;
            List<Entity> list = new List<Entity>();
            for (int i = 0; i < l.Count; i++)
            {
                list.Add(ret);
            }
            return list;
        }
        List<Entity> excuteMinusS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = excuteS(cur.Children[0], map);
            List<Entity> right = excuteS(cur.Children[1], map);
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < left.Count; i++)
            {
                Entity neww = new Entity("", Type.NUM, left[i].valueN - right[i].valueN);
                ret.Add(neww);
            }
            return ret;
        }
        List<Entity> excuteMulS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = excuteS(cur.Children[0], map);
            List<Entity> right = excuteS(cur.Children[1], map);
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < left.Count; i++)
            {
                Entity neww = new Entity("", Type.NUM, left[i].valueN * right[i].valueN);
                ret.Add(neww);
            }
            return ret;
        }
        List<Entity> excuteDivS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = excuteS(cur.Children[0], map);
            List<Entity> right = excuteS(cur.Children[1], map);
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < left.Count; i++)
            {
                Entity neww = new Entity("", Type.NUM, left[i].valueN / right[i].valueN);
                ret.Add(neww);
            }
            return ret;
        }
        List<Entity> excuteIDS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (map.ContainsKey(cur.originalToken.lexeme))
                return map[cur.originalToken.lexeme];
            else
                throw new EmptyQueryException(cur.originalToken.lexeme + "is not a valid Column");
        }
        List<Entity> excuteNumS(Node cur, Dictionary<string, List<Entity>> map)
        {
            List<Entity> ret = new List<Entity>();
            for(int i=0;i<rows.Count;i++)
                ret.Add(new Entity(cur.originalToken.lexeme, Type.NUM, double.Parse(cur.originalToken.lexeme, System.Globalization.CultureInfo.InvariantCulture)));
            return ret;
        }
        List<Entity> excuteStringS(Node cur, Dictionary<string, List<Entity>> map)
        {
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < rows.Count; i++)
                ret.Add(new Entity("",Type.STRING,cur.originalToken.lexeme));
            return ret;
        }
        List<Entity> excuteS(Node cur, Dictionary<string, List<Entity> > map)
        {
            if (cur.originalToken.lexeme == "-")
                return excuteMinusS(cur, map);
            else if (cur.originalToken.lexeme == "+")
                return excutePlusS(cur, map);
            else if (cur.originalToken.lexeme == "*")
                return excuteMulS(cur, map);
            else if (cur.originalToken.lexeme == "/")
                return excuteDivS(cur, map);
            else if (cur.originalToken.lexeme == "avg")
                return avg(cur, map);
            else if (cur.originalToken.lexeme == "sum")
                return sum(cur, map);
            else if (cur.originalToken.lexeme == "min")
                return min(cur, map);
            else if (cur.originalToken.lexeme == "max")
                return max(cur, map);
            else if (cur.nodeType == NodeType.NUMBER)
                return excuteNumS(cur, map);
            else if (cur.nodeType == NodeType.ID)
                return excuteIDS(cur, map);
            else if (cur.nodeType == NodeType.STRING)
                return excuteStringS(cur, map);
            throw new InterpreterException("Unknown token " + cur.originalToken.lexeme);
        }
        Dictionary<string, List<Entity> > initMapS()
        {
            Dictionary<string, List<Entity> > ret = new Dictionary<string, List<Entity> >();
            foreach(List<Entity> r in rows)
                foreach(Entity e in r)
                {
                    if(!ret.ContainsKey(e.name))
                    {
                        ret[e.name] = new List<Entity>();
                    }
                    ret[e.name].Add(e);
                }

            return ret;
        }
        public List<List<string> > getResult()
        {
            selectRows();
            Dictionary<string, List<Entity>> map = initMapS();
            List<List<Entity> > res = excuteSelect(pr.selectNode,map);
            List<List<string> > ret = new List<List<string> >();
            foreach(var i in res)
            {
                List<string> add = new List<string>();
                foreach(var j in i)
                {
                    if (j.type == Type.NUM)
                        add.Add(j.valueN.ToString());
                    else
                        add.Add(j.valueS);
                }
                ret.Add(add);
            }
            return ret;
        }
        #endregion
    }
}
