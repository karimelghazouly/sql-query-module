﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    class LexerException : Exception
    {
        public LexerException(string message) : base(message) { }
    }
    class lexer
    {
        public lexer()
        { }
        private static bool isoperator(char x)
        {
            return (x == '+' || x == '-' || x == '*' || x == '/' || x == '>' || x == '<' || x == '=' || x == '!'
                || x == '%');
        }

        private static Token getLastToken(string temp)
        {
            if (temp.Length > 0)
            {
                TokenType tt = Token.getType(temp);
                if (tt != TokenType.EXCEPT)
                {
                    Token neww = new Token(tt, temp);
                    return neww;
                }
                else
                {
                    throw new LexerException("Unexpected token " + temp);
                }
            }
            return null;
        }
        public static List<Token> Lex(string query)
        {
            query = query.Trim();
            int idx = -1;
            List<Token> ret = new List<Token>();
            string select_check = "";
            if(query.Length >5)
                select_check =  query.Substring(0, 6);
            select_check = select_check.ToLower();
            if (select_check.ToLower() != "select")
            {
                throw new LexerException("Expected SELECT");
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
                            throw new LexerException("Unexpected token " + temp);
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
                throw new LexerException("Expected from");
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
                        throw new LexerException("Unexpected token " + temp);
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
            if (temp.Length > 0&&idx2==-1)
            {
                TokenType t = Token.getType(temp);
                if (t != TokenType.EXCEPT)
                {
                    Token neww = new Token(t, temp);
                    ret.Add(neww);
                }
                else
                {
                    throw new LexerException("Unexpected token " + temp);
                }
            }
            temp = "";
            for (int i = idx2; i < query.Length && i != -1; i++)
            {
                if (query[i] == ' ' || isoperator(query[i]) || query[i] == '(' || query[i] == ')' || query[i] == ',')
                {
                    if (isoperator(query[i]))
                    {
                        Token toInsert = getLastToken(temp);
                        if (toInsert != null) ret.Add(toInsert);
                        temp = "";
                        string o = query[i].ToString();
                        if (i + 1 < query.Length && isoperator(query[i + 1]))
                        {
                            o += query[i + 1];
                            i++;
                        }
                        ret.Add(new Token(TokenType.OP, o));
                    }
                    else if (query[i] == '(')
                    {
                        Token toInsert = getLastToken(temp);
                        if (toInsert != null) ret.Add(toInsert);
                        temp = "";
                        ret.Add(new Token(TokenType.L_PARA, "("));
                    }
                    else if (query[i] == ',')
                    {
                        Token toInsert = getLastToken(temp);
                        if (toInsert != null) ret.Add(toInsert);
                        temp = "";
                        ret.Add(new Token(TokenType.COMMA, ","));
                    }
                    else if (query[i] == ')')
                    {
                        Token toInsert = getLastToken(temp);
                        if (toInsert != null) ret.Add(toInsert);
                        temp = "";
                        ret.Add(new Token(TokenType.R_PARA, ")"));
                    }

                    if (temp == "") continue;
                    TokenType t = Token.getType(temp);
                    if (t != TokenType.EXCEPT)
                    {
                        Token neww = new Token(t, temp);
                        ret.Add(neww);
                    }
                    else
                    {
                        throw new LexerException("Unexpected token " + temp);
                    }
                    temp = "";
                }
                else
                {
                    temp += query[i];
                }
            }

            if(temp.Length> 0)
            {
                TokenType t = Token.getType(temp);
                if (t != TokenType.EXCEPT)
                {
                    Token neww = new Token(t, temp);
                    ret.Add(neww);
                }
                else
                {
                    throw new LexerException("Unexpected token " + temp);
                }
            }

            return ret;
        }
        ~lexer()
        { }
    }
}
