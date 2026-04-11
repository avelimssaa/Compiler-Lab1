%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

extern int yylineno;
extern char *yytext;
extern FILE *yyin;
extern int yyleng;

int current_column = 1;
int has_error = 0;

void yyerror(const char *s);

typedef struct {
    int code;
    char* type;
    char* lexeme;
    int line;
    int start_column;
    int end_column;
    int is_error;
    char* error_message;
} TokenInfo;

TokenInfo tokens[10000];
int token_count = 0;

void add_token(int code, char* lexeme, int line, int start_col, int end_col, int is_error, const char* error_msg);
char* get_token_type(int code);
int yylex();
void print_tokens();

void update_column(const char *text, int length);
%}

%union {
    int intval;
    char *strval;
}

%token TOKEN_FOR
%token TOKEN_PRINTLN
%token TOKEN_TO
%token TOKEN_LPAREN
%token TOKEN_RPAREN
%token TOKEN_LBRACE
%token TOKEN_RBRACE
%token TOKEN_SEMICOLON
%token TOKEN_ARROW
%token TOKEN_ASSIGN
%token TOKEN_IDENTIFIER
%token TOKEN_NUMBER
%token TOKEN_SPACE
%token TOKEN_TAB
%token TOKEN_NEWLINE
%token TOKEN_ERROR
%token TOKEN_LETTER
%token TOKEN_DIGIT

%type <strval> TOKEN_IDENTIFIER TOKEN_NUMBER TOKEN_LETTER TOKEN_DIGIT

%start START

%%

START:
    TOKEN_FOR KEYWORD_FOR
    ;

KEYWORD_FOR:
    TOKEN_LPAREN OPERATOR_OPEN_BRACKET_CYCLE
    ;

OPERATOR_OPEN_BRACKET_CYCLE:
    TOKEN_IDENTIFIER ID_CYCLE
    ;

ID_CYCLE:
    TOKEN_IDENTIFIER ID_CYCLE
    | TOKEN_NUMBER ID_CYCLE
    | TOKEN_ASSIGN OPERATOR_EXPRESSION
    ;

OPERATOR_EXPRESSION:
    TOKEN_NUMBER BEGIN_NUMBER
    ;

BEGIN_NUMBER:
    TOKEN_NUMBER BEGIN_NUMBER
    | TOKEN_TO OPERATOR_TO
    ;

OPERATOR_TO:
    TOKEN_NUMBER END_NUMBER
    ;

END_NUMBER:
    TOKEN_NUMBER END_NUMBER
    | TOKEN_RPAREN OPERATOR_CLOSE_BRACKET_CYCLE
    ;

OPERATOR_CLOSE_BRACKET_CYCLE:
    TOKEN_LBRACE OPERATOR_OPEN_CURLY_BRACE
    ;

OPERATOR_OPEN_CURLY_BRACE:
    TOKEN_NEWLINE OPERATOR_NEWLINE
    ;

OPERATOR_NEWLINE:
    TOKEN_PRINTLN KEYWORD_PRINTLN
    | TOKEN_RBRACE OPERATOR_CLOSE_CURLY_BRACE
    ;

KEYWORD_PRINTLN:
    TOKEN_LPAREN OPERATOR_OPEN_BRACE_PRINTLN
    ;

OPERATOR_OPEN_BRACE_PRINTLN:
    TOKEN_IDENTIFIER ID_CYCLE_BODY
    ;

ID_CYCLE_BODY:
    TOKEN_IDENTIFIER ID_CYCLE_BODY
    | TOKEN_NUMBER ID_CYCLE_BODY
    | TOKEN_RPAREN OPERATOR_CLOSE_BRACE_PRINTLN
    ;

OPERATOR_CLOSE_BRACE_PRINTLN:
    TOKEN_NEWLINE OPERATOR_NEWLINE
    ;

OPERATOR_CLOSE_CURLY_BRACE:
    TOKEN_SEMICOLON
    ;

%%

void yyerror(const char *s) {
    has_error = 1;
    fprintf(stderr, "Syntax error at line %d, column %d: %s\n", 
            yylineno, current_column, s);
    fprintf(stderr, "  Unexpected token: '%s'\n", yytext);
    
    add_token(273, yytext, yylineno, current_column, 
              current_column + yyleng - 1, 1, s);
}

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

void add_token(int code, char* lexeme, int line, int start_col, int end_col, int is_error, const char* error_msg) {
    if (token_count < 10000) {
        tokens[token_count].code = code;
        tokens[token_count].type = strdup(get_token_type(code));
        tokens[token_count].lexeme = strdup(lexeme);
        tokens[token_count].line = line;
        tokens[token_count].start_column = start_col;
        tokens[token_count].end_column = end_col;
        tokens[token_count].is_error = is_error;
        tokens[token_count].error_message = error_msg ? strdup(error_msg) : strdup("");
        token_count++;
    }
}

char* get_token_type(int code) {
    switch (code) {
        case 258: return strdup("for");
        case 259: return strdup("println");
        case 260: return strdup("to");
        case 261: return strdup("(");
        case 262: return strdup(")");
        case 263: return strdup("{");
        case 264: return strdup("}");
        case 265: return strdup(";");
        case 266: return strdup("<-");
        case 267: return strdup("=");
        case 268: return strdup("identifier");
        case 269: return strdup("number");
        case 270: return strdup("letter");
        case 271: return strdup("digit");
        case 272: return strdup("newline");
        case 273: return strdup("error");
        default:  return strdup("unknown");
    }
}

void print_tokens() {
    printf("[\n");
    for (int i = 0; i < token_count; i++) {
        printf("  {\n");
        printf("    \"code\": %d,\n", tokens[i].code);
        printf("    \"type\": \"%s\",\n", tokens[i].type);
        
        printf("    \"lexeme\": \"");
        for (int j = 0; tokens[i].lexeme[j]; j++) {
            char c = tokens[i].lexeme[j];
            if (c == '"') printf("\\\"");
            else if (c == '\\') printf("\\\\");
            else if (c == '\n') printf("\\n");
            else if (c == '\t') printf("\\t");
            else if (c == '\r') printf("\\r");
            else printf("%c", c);
        }
        printf("\",\n");
        
        printf("    \"line\": %d,\n", tokens[i].line);
        printf("    \"start_column\": %d,\n", tokens[i].start_column);
        printf("    \"end_column\": %d,\n", tokens[i].end_column);
        printf("    \"is_error\": %s,\n", tokens[i].is_error ? "true" : "false");
        printf("    \"error_message\": \"%s\"\n", tokens[i].error_message);
        printf("  }%s\n", (i < token_count - 1) ? "," : "");
    }
    printf("]\n");
}

int main(int argc, char **argv) {
    if (argc > 1) {
        FILE *file = fopen(argv[1], "r");
        if (!file) {
            fprintf(stderr, "Cannot open file: %s\n", argv[1]);
            return 1;
        }
        yyin = file;
    }
    
    token_count = 0;
    current_column = 1;
    has_error = 0;
    
    yyparse();
    print_tokens();
    
    return has_error ? 1 : 0;
}