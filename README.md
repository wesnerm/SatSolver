# CDCL-based SAT Solver.

The SAT reads from STDIN and takes a formula expressed in S-Expressions.

The formula consists of the following:
- A variable, which is the letter x followed by positive integer (e.g., x1, x2, x3, ...)
- A list consisting of an operator and 1 or 2 arguments
  - Negation operator: (not x1)
  - Conjunction operator: (and x1 x2)
  - Disjunction operator: (or x1 x2)
  - Implication operator: (impl x1 x2)
  - Equivalence operator: (equiv x1 x2)
	
The SAT includes both a DPLL and a CDCL (Conflict-Driven Clause Learning) implementation.

The DPLL implementation is a recursive backtracking algorithm that searches for a satisfying assignment to the variables in the formula. 
It uses unit propagation to simplify the formula before searching for a solution.

The CDCL implementation is an extension of the DPLL algorithm that uses conflict-driven clause learning to improve the search process.
It learns from conflicts encountered during the search and adds new clauses to the formula to prevent the same conflicts from occurring again.
The CDCL implementation also includes a restarts mechanism that periodically restarts the search process to improve performance.

Several optimizations are demonstrated in the code, including:
- Unit propagation
- Watched literals
- Conflict analysis
	- Non-chronological backjumping
	- Clause learning
- Heuristic variable selection with VSIDS (Variable State Independent Decaying Sum)
- Restarts
