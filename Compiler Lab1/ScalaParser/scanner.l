%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "parser.tab.h"

extern int yylineno;
extern char *yytext;
extern int current_column;

void add_token(int code, char* lexeme, int line, int start_col, int end_col, int is_error, const char* error_msg);

void update_column();
%}

%option noyywrap
%option yylineno
%option nounput
%option noinput
%option outfile="lex.yy.c"

white       [ \t]+
newline     \n
digit       [0-9]
letter      [a-zA-Z]
identifier  {letter}({letter}|{digit}|_)*
comment     "//".*

%%

^[\xEF][\xBB][\xBF] {
}

{white} {
    update_column();
    add_token(269, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_SPACE;
}

{newline} {
    add_token(271, "\n", yylineno, current_column, current_column, 0, "");
    current_column = 1;
    yylval.strval = strdup("\n");
    return TOKEN_NEWLINE;
}

"for" {
    update_column();
    add_token(260, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_FOR;
}

"to" {
    update_column();
    add_token(261, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_TO;
}

"println" {
    update_column();
    add_token(258, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_PRINTLN;
}

"<-" {
    update_column();
    add_token(267, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_ARROW;
}

"=" {
    update_column();
    add_token(272, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_ASSIGN;
}

"(" {
    update_column();
    add_token(262, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_LPAREN;
}

")" {
    update_column();
    add_token(263, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_RPAREN;
}

"{" {
    update_column();
    add_token(264, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_LBRACE;
}

"}" {
    update_column();
    add_token(265, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_RBRACE;
}

";" {
    update_column();
    add_token(266, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_SEMICOLON;
}

{digit}+ {
    update_column();
    add_token(268, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.intval = atoi(yytext);
    return TOKEN_NUMBER;
}

{identifier} {
    update_column();
    add_token(259, yytext, yylineno, current_column - yyleng, current_column - 1, 0, "");
    yylval.strval = strdup(yytext);
    return TOKEN_IDENTIFIER;
}

{comment} {
    update_column();
}

. {
    update_column();
    add_token(273, yytext, yylineno, current_column - yyleng, current_column - 1, 1, "Invalid character");
    yylval.strval = strdup(yytext);
    return TOKEN_ERROR;
}

%%

void update_column() {
    current_column += yyleng;
}