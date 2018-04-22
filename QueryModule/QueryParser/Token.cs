using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    enum TokenType
    {
        SELECT, OP, ID, NUM, FROM, WHERE, L_PARA, R_PARA, COMMA, AND, OR, STRING
    }
    class Token
    {
        internal TokenType tokenType;
        internal string lexeme;

        internal Token(TokenType type, string lex)
        {
            tokenType = type;
            lexeme = lex;
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
    }
}
