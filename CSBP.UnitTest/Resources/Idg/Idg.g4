grammar Idg;

/* Parser Rules, first lowercase letter */
/*
 Lib herunterladen: https://www.antlr.org/download/antlr-4.13.2-complete.jar
 Erzeugen mit: java -jar ~/Downloads/antlr-4.13.2-complete.jar -Dlanguage=CSharp -o ~/cs/csbp/CSBP.UnitTest/Resources/Idg/ ~/cs/csbp/CSBP.UnitTest/Resources/Idg/Idg.g4
*/

model: ('Modell' mainmodel)? (WS | ML_COMMENT)* abstractelement* EOF;
mainmodel: ID;
abstractelement: sqlstatement;
sqlstatement: droptable;
droptable:
	'if' 'exists' '(' 'select' '*' 'from' 'sysobjects' 'where' 'id' '=' 'object_id' '(' STRING ')'
		'and' 'sysstat' '&' '0xf' '=' '3' ')' 'drop' 'table' ID 'GO';

/* Lexer Rules, uppercase name */
ID:
	'^'? ('a' ..'z' | 'A' ..'Z' | '_') (
		'a' ..'z'
		| 'A' ..'Z'
		| '_'
		| '0' ..'9'
	)*;
STRING: '"' ('""' | ~'"')* '"';
WS: [ \r\n\t]+ -> skip;
NL: '\r'? '\n' -> skip;
ML_COMMENT: '/*' .*? '*/' -> skip;
SL_COMMENT: '//' ~('\n' | '\r')* ('\r'? '\n')? -> skip;
fragment ANY: .;
