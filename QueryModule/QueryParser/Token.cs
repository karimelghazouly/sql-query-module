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
        TokenType tokenType;
        string lexeme;
    }
}
