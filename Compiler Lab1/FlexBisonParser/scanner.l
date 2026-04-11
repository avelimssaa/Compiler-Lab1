%{
#include "parser.tab.h"
#include <stdio.h>
#include <string.h>

extern int yylineno;
extern int current_column;
extern char *yytext;
extern int yyleng;

void update_column(const char *text, int length);
%}

%option noyywrap

%%

"for"               { update_column(yytext, yyleng); return TOKEN_FOR; }
"println"           { update_column(yytext, yyleng); return TOKEN_PRINTLN; }
"to"                { update_column(yytext, yyleng); return TOKEN_TO; }
"<-"                { update_column(yytext, yyleng); return TOKEN_ARROW; }
"="                 { update_column(yytext, yyleng); return TOKEN_ASSIGN; }
"("                 { update_column(yytext, yyleng); return TOKEN_LPAREN; }
")"                 { update_column(yytext, yyleng); return TOKEN_RPAREN; }
"{"                 { update_column(yytext, yyleng); return TOKEN_LBRACE; }
"}"                 { update_column(yytext, yyleng); return TOKEN_RBRACE; }
";"                 { update_column(yytext, yyleng); return TOKEN_SEMICOLON; }
"\n"                { update_column(yytext, yyleng); yylineno++; return TOKEN_NEWLINE; }
[ \t]+              { update_column(yytext, yyleng); /* игнорируем пробелы и табы */ }
[a-zA-Z_][a-zA-Z0-9_]* {
    update_column(yytext, yyleng);
    yylval.strval = strdup(yytext);
    return TOKEN_IDENTIFIER;
}
[0-9]+              {
    update_column(yytext, yyleng);
    yylval.strval = strdup(yytext);
    return TOKEN_NUMBER;
}
[a-zA-Z]            {
    update_column(yytext, yyleng);
    yylval.strval = strdup(yytext);
    return TOKEN_LETTER;
}
[0-9]               {
    update_column(yytext, yyleng);
    yylval.strval = strdup(yytext);
    return TOKEN_DIGIT;
}
.                   {
    update_column(yytext, yyleng);
    return TOKEN_ERROR;
}

%%

void update_column(const char *text, int length) {
    for (int i = 0; i < length; i++) {
        if (text[i] == '\n') {
            current_column = 1;
        } else if (text[i] == '\t') {
            current_column += 4;
        } else {
            current_column++;
        }
    }
}