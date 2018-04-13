query
	: select_stat from_stat (where_stat)?

select_stat
	:	SELECT expression (COMMA expression)*
	;

from_stat
	:	FROM ID
	;

where_stat
	:	WHERE expression
	;

factor
	:	NUMBER
	|	id
	|	MINUS factor
	|	L_PARA expression R_PARA
	;

id
	:	ID
	|	func_call
	;

func_call
	:	ID L_PARA expression R_PARA
	;

mul_expr
	:	factor ((DIV|MOD|MUL) factor)*
	;

add_expr
	:	mul_expr ((PLUS|MINUS) mul_expr)*
	;

comp_expr
	:	add_expr ((COMP_OP) add_expr)*
	;

logic_expr
	:	comp_expr ((AND|OR) comp_expr)*
	;

expression
	:	logic_expr
	;