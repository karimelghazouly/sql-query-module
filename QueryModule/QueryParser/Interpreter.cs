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
        Entity executeWhere(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            return execute(cur.Children[0], r, map);
        }
        Entity executeAND(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x1 = Math.Abs(execute(cur.Children[0], r, map).valueN) >= 1e-8;
            bool x2 = Math.Abs(execute(cur.Children[1], r, map).valueN) >= 1e-8;
            if (x1 && x2)
                ret.valueN = 1.0;
            else
                ret.valueN = 0.0;           
            return ret;
        }
        Entity executeOR(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x1 = Math.Abs(execute(cur.Children[0], r, map).valueN) >= 1e-8;
            bool x2 = Math.Abs(execute(cur.Children[1], r, map).valueN) >= 1e-8;
            if (x1 || x2)
                ret.valueN = 1.0;
            else
                ret.valueN = 0.0;
            return ret;
        }
        Entity executePlus(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (execute(cur.Children[0], r, map).valueN) + (execute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity executeMinus(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (execute(cur.Children[0], r, map).valueN) - (execute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity executeMul(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (execute(cur.Children[0], r, map).valueN) * (execute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity executeDiv(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            ret.valueN = (execute(cur.Children[0], r, map).valueN) / (execute(cur.Children[1], r, map).valueN);
            return ret;
        }
        Entity executeEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
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
            bool x = (execute(cur.Children[0], r, map).valueN) == (execute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity executeNotEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
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
            bool x = (execute(cur.Children[0], r, map).valueN) != (execute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity executeLess(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (execute(cur.Children[0], r, map).valueN) < (execute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity executeLessEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (execute(cur.Children[0], r, map).valueN) <= (execute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity executeMore(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (execute(cur.Children[0], r, map).valueN) > (execute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity executeMoreEq(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = new Entity("", Type.NUM, 0.0);
            bool x = (execute(cur.Children[0], r, map).valueN) >= (execute(cur.Children[1], r, map).valueN);
            if (x) ret.valueN = 1;
            return ret;
        }
        Entity executeNot(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (!assertBinaryNode(cur))
                return null;
            Entity ret = execute(cur.Children[0], r,map);
            if (ret.valueN == 0.0) ret.valueN = 1;
            else ret.valueN = 0;
            return ret;
        }
        Entity executeID(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            if (map.ContainsKey(cur.originalToken.lexeme))
                return map[cur.originalToken.lexeme];
            else
                throw new EmptyQueryException(cur.originalToken.lexeme + "is not a valid column");
        }
        Entity executeNum(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            return new Entity(cur.originalToken.lexeme, Type.NUM, double.Parse(cur.originalToken.lexeme, System.Globalization.CultureInfo.InvariantCulture));
        }
        Entity executeList(Node cur, List<Entity> r, Dictionary<string, Entity> map, Entity v)
        {
            Entity ret = new Entity("", Type.NUM, 0.0);            
            foreach(Node child in cur.Children)
            {
                Entity x = execute(child, r, map);
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
        Entity executeIN(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {           
            Entity left = execute(cur.Children[0], r, map);
            return executeList(cur.Children[1], r, map, left);
        }
        Entity executeString(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            Entity ret = new Entity("", Type.STRING, cur.originalToken.lexeme);
            return ret;
        }
        Entity execute(Node cur, List<Entity> r, Dictionary<string, Entity> map)
        {
            Entity ret;
            if (cur.originalToken.lexeme == "-")
                ret = executeMinus(cur, r, map);
            else if (cur.originalToken.lexeme == "+")
                ret = executePlus(cur, r, map);
            else if (cur.originalToken.lexeme == "*")
                ret = executeMul(cur, r, map);
            else if (cur.originalToken.lexeme == "/")
                ret = executeDiv(cur, r, map);

            else if (cur.originalToken.lexeme == "<")
                ret = executeLess(cur, r, map);
            else if (cur.originalToken.lexeme == "<=")
                ret = executeLessEq(cur, r, map);
            else if (cur.originalToken.lexeme == ">")
                ret = executeMore(cur, r, map);
            else if (cur.originalToken.lexeme == ">=")
                ret = executeMoreEq(cur, r, map);

            else if (cur.originalToken.lexeme == "and")
                ret = executeAND(cur, r, map);
            else if (cur.originalToken.lexeme == "or")
                ret = executeOR(cur, r, map);

            else if (cur.originalToken.lexeme == "where")
                ret = executeWhere(cur, r, map);

            else if (cur.originalToken.lexeme == "=")
                ret = executeEq(cur, r, map);
            else if (cur.originalToken.lexeme == "!=")
                ret = executeNotEq(cur, r, map);
            else if (cur.originalToken.lexeme == "in")
                ret = executeIN(cur, r, map);
            else if (cur.originalToken.lexeme == "not")
                ret = executeNot(cur, r, map);
            else if (cur.nodeType == NodeType.NUMBER)
                ret = executeNum(cur, r, map);
            else if (cur.nodeType == NodeType.ID)
                ret = executeID(cur, r, map);
            else if (cur.nodeType == NodeType.STRING)
                ret = executeString(cur, r, map);
            else
                throw new InterpreterException("Unexpected token " + cur.originalToken.lexeme);
            if (ret == null)
                throw new InterpreterException("Invlaid Operands for " + cur.originalToken.lexeme);
            return ret;
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
                Entity x = execute(pr.whereNode, r, map);
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
        List <List<Entity> >executeSelect(Node cur, Dictionary<string, List<Entity> >map)
        {
            List<List<Entity> >ret = new List< List<Entity> >();
            for (int i = 0; i < cur.Children.Count; i++)           
                ret.Add(executeS(cur.Children[i], map));
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

        List<Entity> executePlusS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = executeS(cur.Children[0], map);
            List<Entity> right = executeS(cur.Children[1], map);
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
            List<Entity> l = executeS(cur.Children[0], map);           
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
            List<Entity> l = executeS(cur.Children[0], map);
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
            List<Entity> l = executeS(cur.Children[0], map);
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
            List<Entity> l = executeS(cur.Children[0], map);
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
        List<Entity> executeMinusS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = executeS(cur.Children[0], map);
            List<Entity> right = executeS(cur.Children[1], map);
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < left.Count; i++)
            {
                Entity neww = new Entity("", Type.NUM, left[i].valueN - right[i].valueN);
                ret.Add(neww);
            }
            return ret;
        }
        List<Entity> executeMulS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = executeS(cur.Children[0], map);
            List<Entity> right = executeS(cur.Children[1], map);
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < left.Count; i++)
            {
                Entity neww = new Entity("", Type.NUM, left[i].valueN * right[i].valueN);
                ret.Add(neww);
            }
            return ret;
        }
        List<Entity> executeDivS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (!assertBinaryNodeS(cur))
                return null;
            List<Entity> left = executeS(cur.Children[0], map);
            List<Entity> right = executeS(cur.Children[1], map);
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < left.Count; i++)
            {
                Entity neww = new Entity("", Type.NUM, left[i].valueN / right[i].valueN);
                ret.Add(neww);
            }
            return ret;
        }
        List<Entity> executeIDS(Node cur, Dictionary<string, List<Entity>> map)
        {
            if (map.ContainsKey(cur.originalToken.lexeme))
                return map[cur.originalToken.lexeme];
            else
                throw new EmptyQueryException(cur.originalToken.lexeme + "is not a valid Column");
        }
        List<Entity> executeNumS(Node cur, Dictionary<string, List<Entity>> map)
        {
            List<Entity> ret = new List<Entity>();
            for(int i=0;i<rows.Count;i++)
                ret.Add(new Entity(cur.originalToken.lexeme, Type.NUM, double.Parse(cur.originalToken.lexeme, System.Globalization.CultureInfo.InvariantCulture)));
            return ret;
        }
        List<Entity> executeStringS(Node cur, Dictionary<string, List<Entity>> map)
        {
            List<Entity> ret = new List<Entity>();
            for (int i = 0; i < rows.Count; i++)
                ret.Add(new Entity("",Type.STRING,cur.originalToken.lexeme));
            return ret;
        }
        List<Entity> executeS(Node cur, Dictionary<string, List<Entity> > map)
        {
            List<Entity> ret;
            if (cur.originalToken.lexeme == "-")
                ret = executeMinusS(cur, map);
            else if (cur.originalToken.lexeme == "+")
                ret = executePlusS(cur, map);
            else if (cur.originalToken.lexeme == "*")
                ret = executeMulS(cur, map);
            else if (cur.originalToken.lexeme == "/")
                ret = executeDivS(cur, map);
            else if (cur.originalToken.lexeme == "avg")
                ret = avg(cur, map);
            else if (cur.originalToken.lexeme == "sum")
                ret = sum(cur, map);
            else if (cur.originalToken.lexeme == "min")
                ret = min(cur, map);
            else if (cur.originalToken.lexeme == "max")
                ret = max(cur, map);
            else if (cur.nodeType == NodeType.NUMBER)
                ret = executeNumS(cur, map);
            else if (cur.nodeType == NodeType.ID)
                ret = executeIDS(cur, map);
            else if (cur.nodeType == NodeType.STRING)
                ret = executeStringS(cur, map);
            else
                throw new InterpreterException("Unknown token " + cur.originalToken.lexeme);
            if (ret == null)
                throw new InterpreterException("Invalid Operands for" + cur.originalToken.lexeme);
            return ret;
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
            List<List<Entity> > res = executeSelect(pr.selectNode,map);
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
