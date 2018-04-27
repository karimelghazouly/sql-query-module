using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    enum TokenType
    {
        SELECT, OP, ID, NUM, FROM, WHERE, L_PARA, R_PARA, COMMA, AND, OR, STRING, EXCEPT, NOT, IN, EOF
    }
    class Token
    {
        public static TokenType getType(string s)
        {
            s = s.ToLower();
            if (s == "and") return TokenType.AND;
            if (s == "or") return TokenType.OR;
            if (s == "not") return TokenType.NOT;
            if (s == "in") return TokenType.IN;
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
                if (cnt > 1)
                    return TokenType.EXCEPT;
                return TokenType.NUM;
            }
            else if (s.Count() > 1 && ((s[0] == '\'' && s[s.Count() - 1] == '\'') || (s[0] == '\"' && s[s.Count() - 1] == '\"')))
                return TokenType.STRING;
            return TokenType.EXCEPT;
	}

        internal TokenType tokenType;
        internal string lexeme;

        internal Token(TokenType type, string lex)
        {
            tokenType = type;
            lexeme = casedLexeme(lex);
        }

        internal string casedLexeme(string lexeme)
        {
            if(tokenType == TokenType.SELECT || tokenType == TokenType.OP || 
                tokenType == TokenType.FROM || tokenType == TokenType.WHERE || 
                tokenType == TokenType.R_PARA || tokenType == TokenType.COMMA || 
                tokenType == TokenType.AND || tokenType == TokenType.OR || 
                tokenType == TokenType.STRING || tokenType == TokenType.NOT || 
                tokenType == TokenType.IN)
            {
                return lexeme.ToLower();
            }
            return lexeme;
        }
        internal bool isComparison()
        {
            return lexeme == "<" || lexeme == "<=" || lexeme == ">" || lexeme == ">=" || lexeme == "=" || lexeme == "!=";
        }

        internal bool isAddition()
        {
            return lexeme == "+" || lexeme == "-";
        }

        internal bool isMultiplication()
        {
            return lexeme == "*" || lexeme == "/";
        }

        internal bool isLogical()
        {
            return tokenType == TokenType.AND || tokenType == TokenType.OR;
        }
    }

}
