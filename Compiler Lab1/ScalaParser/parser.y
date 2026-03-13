%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

extern int yylineno;
extern char *yytext;
extern FILE *yyin;
extern int yyleng;

int current_column = 1;

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
%}

%union {
    int intval;
    char *strval;
}

%token TOKEN_PRINTLN
%token TOKEN_IDENTIFIER
%token TOKEN_FOR
%token TOKEN_TO
%token TOKEN_LPAREN
%token TOKEN_RPAREN
%token TOKEN_LBRACE
%token TOKEN_RBRACE
%token TOKEN_SEMICOLON
%token TOKEN_ARROW
%token TOKEN_NUMBER
%token TOKEN_SPACE
%token TOKEN_TAB
%token TOKEN_NEWLINE
%token TOKEN_ASSIGN
%token TOKEN_ERROR

%type <intval> TOKEN_NUMBER
%type <strval> TOKEN_IDENTIFIER TOKEN_SPACE TOKEN_ERROR

%%

program:
    /* empty */
    | program token
    ;

token:
    TOKEN_FOR
    | TOKEN_PRINTLN
    | TOKEN_TO
    | TOKEN_IDENTIFIER
    | TOKEN_NUMBER
    | TOKEN_LPAREN
    | TOKEN_RPAREN
    | TOKEN_LBRACE
    | TOKEN_RBRACE
    | TOKEN_SEMICOLON
    | TOKEN_ARROW
    | TOKEN_ASSIGN
    | TOKEN_SPACE
    | TOKEN_NEWLINE
    | TOKEN_TAB
    | TOKEN_ERROR
    ;

%%

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
        case 258: return strdup("println");
        case 259: return strdup("identifier");
        case 260: return strdup("for");
        case 261: return strdup("to");
        case 262: return strdup("(");
        case 263: return strdup(")");
        case 264: return strdup("{");
        case 265: return strdup("}");
        case 266: return strdup(";");
        case 267: return strdup("<-");
        case 268: return strdup("number");
        case 269: return strdup("space");
        case 270: return strdup("tab");
        case 271: return strdup("newline");
        case 272: return strdup("=");
        case 273: return strdup("error");
        default:  return strdup("unknown");
    }
}

void yyerror(const char *s) {
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
    
    yyparse();
    print_tokens();
    
    return 0;
}