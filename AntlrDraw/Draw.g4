grammar Draw;
prog:		stm*;
stm:		Init NUMBER NUMBER			# init
			| Up						# up
			| Down						# down
			| Left						# left
			| Right						# right
			| Draw 						# draw
			| Display 					# display
			;









fragment U:		'u' | 'U';
fragment P:		'p' | 'P';
fragment D:		'd' | 'D';
fragment O:		'o' | 'O';
fragment W:		'w' | 'W';
fragment N:		'n' | 'N';
fragment L:		'l' | 'L';
fragment E:		'e' | 'E';
fragment F:		'f' | 'F';
fragment T:		't' | 'T';
fragment R:		'r' | 'R';
fragment I:		'i' | 'I';
fragment G:		'g' | 'G';
fragment H:		'h' | 'H';
fragment A:		'a' | 'A';
fragment S:		's' | 'S';
fragment Y:		'y' | 'Y';

Up:			U P;
Down:		D O W N;
Left:		L E F T;
Right:		R I G H T;
Init:		I N I T;
Draw:		D R A W;
Display:	D I S P L A Y;

NUMBER:		[0-9]+;

WS:			[ \t\n\r]+ -> skip;






