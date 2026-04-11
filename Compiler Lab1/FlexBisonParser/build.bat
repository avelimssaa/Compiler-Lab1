@echo off
echo === Building Parser for Scala for Loop ===
echo.

set FLEX_PATH=D:\flex_bison\win_flex.exe
set BISON_PATH=D:\flex_bison\win_bison.exe

if not exist "%FLEX_PATH%" (
    echo ERROR: win_flex.exe not found at %FLEX_PATH%
    echo Please specify the correct path in FLEX_PATH variable
    goto error
)

if not exist "%BISON_PATH%" (
    echo ERROR: win_bison.exe not found at %BISON_PATH%
    echo Please specify the correct path in BISON_PATH variable
    goto error
)

echo [1/3] Generating parser from parser.y...
%BISON_PATH% -d parser.y
if errorlevel 1 (
    echo Error generating parser
    goto error
)
echo Done

echo [2/3] Generating lexer from scanner.l...
%FLEX_PATH% scanner.l
if errorlevel 1 (
    echo Error generating lexer
    goto error
)
echo Done

echo [3/3] Compiling to parser.exe...
gcc -o parser.exe parser.tab.c lex.yy.c
if errorlevel 1 (
    echo Compilation error
    goto error
)
echo Done

echo.
echo === Build completed successfully! ===
echo Created file: parser.exe
dir parser.exe
goto end

:error
echo.
echo === ERROR! Build aborted ===
exit /b 1

:end
pause