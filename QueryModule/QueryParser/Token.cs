using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    enum TokenType
    {
        SELECT, OP, ID, NUM, FROM, WHERE, L_PARA, R_PARA, COMMA, AND, OR, STRING, EXCEPT, NOT
    }
    class Token
    {
        public TokenType tokenType;
        public string lexemes;
        public Token()
        { }
        public Token(string txt, TokenType type)
        {
            lexemes = txt;
            tokenType = type;
        }
        public TokenType get_type(string s)
        {
            s = s.ToLower();
            if (s == "and") return TokenType.AND;
            if (s == "or") return TokenType.OR;
            if (s == "not") return TokenType.NOT;
            else if (s[0] >= 'a' && s[0] <= 'z')
            {
                for (int i = 0; i < s.Count(); i++)
                {
                    if (s[i] != '_' && (s[i] < 'a' || s[i] > 'z') && (s[i] < '0' || s[i] > '9')) return TokenType.EXCEPT;
                }
                return TokenType.ID;
            }

            else if (s[0] >= '0' && s[0] <= '9')
            {
                int cnt = 0;
                for (int i = 0; i < s.Count(); i++)
                {
                    if (s[i] == '.') cnt++;
                    if (s[i] != '.' && (s[i] < '0' || s[i] > '9')) return TokenType.EXCEPT;
                }
                if (cnt > 1) return TokenType.EXCEPT;
                return TokenType.NUM;
            }
            else if (s.Count() > 1 && ((s[0] == '\'' && s[s.Count() - 1] == '\'') || (s[0] == '\"' && s[s.Count() - 1] == '\"')))
                return TokenType.STRING;
            return TokenType.EXCEPT;
        }
    }

}
