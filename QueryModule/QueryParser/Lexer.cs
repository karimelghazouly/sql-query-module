using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    class lexer
    {
        public lexer()
        { }
        bool isoperator(char x)
        {
            return (x == '+' || x == '-' || x == '*' || x == '/' || x == '>' || x == '<' || x == '=' || x == '!'
                || x == '%');
        }
        public List<Token> Lex(string query)
        {
            query = query.Trim();
            int idx = -1;
            List<Token> ret = new List<Token>();
            string select_check = query.Substring(0, 6);
            select_check = select_check.ToLower();
            if (select_check != "select")
            {
                // raise exepction
                Console.WriteLine("awl btngan");
                return null;
            }
            Token n = new Token(0, "select");
            ret.Add(n);
            string temp = "";
            for (int i = 6; i < query.Length; i++)
            {
                if (i + 3 < query.Length && query.Substring(i, 4).ToLower() == "from")
                {
                    idx = i + 4;
                    ret.Add(new Token(TokenType.FROM, "from"));
                    break;
                }
                if (query[i] == ' ' || query[i] == ',' || isoperator(query[i]) || query[i] == '(' || query[i] == ')')
                {
                    if (temp.Count() != 0)
                    {
                        TokenType t = Token.getType(temp);
                        if (t == TokenType.EXCEPT)
                        {
                            //raise btnagan
                            Console.WriteLine("tny btngan");
                            return null;
                        }
                        ret.Add(new Token(t, temp));
                    }

                    if (query[i] == '(')
                        ret.Add(new Token(TokenType.L_PARA, "("));
                    else if (query[i] == ')')
                        ret.Add(new Token(TokenType.R_PARA, ")"));

                    else if (isoperator(query[i]))
                    {
                        string o = query[i].ToString();
                        if (isoperator(query[i + 1]))
                        {
                            o += query[i + 1];
                            i++;
                        }
                        ret.Add(new Token(TokenType.OP, o));
                    }
                    else if (query[i] == ',')
                        ret.Add(new Token(TokenType.COMMA, ","));
                    temp = "";
                }
                else temp += query[i];
            }
            temp = "";
            int idx2 = -1;
            if (idx == -1)
            {
                //raise excpetion
                Console.WriteLine("hamada idx = -1");
                return null;
            }
            for (int i = idx; i < query.Count(); i++)
            {
                if (query[i] == ' ')
                {
                    if (temp == "") continue;
                    TokenType t = Token.getType(temp);
                    if (t != TokenType.EXCEPT)
                    {
                        Token neww = new Token(t, temp);
                        ret.Add(neww);
                    }
                    else
                    {
                        //raise exception
                        Console.WriteLine("henanasdasd;");
                        return null;
                    }
                    temp = "";
                }
                else
                {
                    temp += query[i];
                    if (i + 4 < query.Length && query.Substring(i, 5).ToLower() == "where")
                    {
                        idx2 = i + 5;
                        ret.Add(new Token(TokenType.WHERE, "where"));
                        break;
                    }
                }
            }
            temp = "";
            // select x , y+3+(5/8) , avg(x+((9*9)*(12/2))) from jaaa where bla == bla and NOT yyy=='qwe'
            // el ahna 3mleno fe el loop el tht da ghalat 3shan el case de hy5od heta de kolha id wahd
            for (int i = idx2; i < query.Length && i != -1; i++)
            {
                if (query[i] == ' ' || isoperator(query[i]) || query[i] == '(' || query[i] == ')')
                {
                    if (isoperator(query[i]))
                    {
                        string o = query[i].ToString();
                        if (i + 1 < query.Length && isoperator(query[i + 1]))
                        {
                            o += query[i + 1];
                            i++;
                        }
                        ret.Add(new Token(TokenType.OP, o));
                    }
                    else if (query[i] == '(')
                        ret.Add(new Token(TokenType.L_PARA, "("));
                    else if (query[i] == ')')
                        ret.Add(new Token(TokenType.R_PARA, ")"));

                    if (temp == "") continue;
                    TokenType t = Token.getType(temp);
                    if (t != TokenType.EXCEPT)
                    {
                        Token neww = new Token(t, temp);
                        ret.Add(neww);
                    }
                    else
                    {
                        //raise exception
                        Console.WriteLine("btngahagagaga");
                        return null;
                    }
                    temp = "";
                }
                else
                {
                    temp += query[i];
                }
            }

            return ret;
        }
        ~lexer()
        { }
    }
}
