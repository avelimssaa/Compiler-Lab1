grammar ForLang;

// ----------------------
// PARSER RULES
// ----------------------

start
    : FOR keywordFor
    ;

keywordFor
    : LPAREN openBracketCycle
    ;

openBracketCycle
    : IDENTIFIER idCycle
    ;
idCycle
    : (IDENTIFIER | DIGIT | UNDERSCORE)* ARROW operatorExpression
    ;

operatorExpression
    : DIGIT beginNumber
    ;

beginNumber
    : DIGIT beginNumber
    | TO operatorTo
    ;

operatorTo
    : DIGIT endNumber
    ;

endNumber
    : DIGIT endNumber
    | RPAREN openCurly
    ;
    openCurly
    : LBRACE NEWLINE operatorNewline
    ;

operatorNewline
    : PRINTLN cycleBody
    | RBRACE closeCurly
    ;

cycleBody
    : keywordPrintln
    ;

keywordPrintln
    : LPAREN openBracePrintln
    ;

openBracePrintln
    : IDENTIFIER idCycleBody
    ;

idCycleBody
    : (IDENTIFIER | DIGIT | UNDERSCORE)* RPAREN closeBracePrintln
    ;

closeBracePrintln
    : NEWLINE operatorNewline
    ;

closeCurly
    : SEMICOLON
    ;


// ----------------------
// LEXER RULES
// ----------------------

FOR         : 'for';
TO          : 'to';
PRINTLN     : 'println';

ARROW       : '<-';

LPAREN      : '(';
RPAREN      : ')';
LBRACE      : '{';
RBRACE      : '}';
SEMICOLON   : ';';
UNDERSCORE : '_';


IDENTIFIER  : [a-zA-Z_][a-zA-Z0-9_]*;
DIGIT       : [0-9];

NEWLINE     : '\r'? '\n';
WS          : [ \t]+ -> skip;

