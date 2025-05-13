using System.Diagnostics;
namespace SatSolver;

using static ExpFactory;

public class RandomExprGen
{
	private readonly long depth;

	private readonly Random rand = new Random();

	private readonly HashSet<VarExp> vars = new HashSet<VarExp>();

	private long nextVarId = 1;

	public RandomExprGen(long depth)
	{
		this.depth = depth;
	}

	public virtual Exp gen()
	{
		return genExpr(depth);
	}

	private VarExp Var
	{
		get
		{
			if (vars.Count > 0 && rand.NextDouble() >= 0.5)
			{
				VarExp[] varsAsArray = vars.ToArray();
				return varsAsArray[rand.Next(varsAsArray.Length)];
			}
			else
			{
				VarExp varExpr = VAR(nextVarId++);
				vars.Add(varExpr);
				return varExpr;
			}
		}
	}

	private Exp genExpr(long depth)
	{
		if (depth == 0)
		{
			return Var;
		}
		else
		{
			Exp? newExpr = null;
			switch (rand.Next(5))
			{
				case 0:
				{
					Exp left = genExpr(depth - 1);
					Exp right = rand.NextDouble() >= 0.5 ? genExpr(depth - 1) : NEG(left);

					newExpr = AND(left, right);
				}
					break;
				case 1:
				{
					Exp left = genExpr(depth - 1);
					Exp right = rand.NextDouble() >= 0.5 ? genExpr(depth - 1) : NEG(left);

					newExpr = EQUIV(left, right);
				}
					break;
				case 2:
				{
					Exp antecedent = genExpr(depth - 1);
					Exp consequent = genExpr(depth - 1);

					newExpr = IMPL(antecedent, consequent);
				}
					break;
				case 3:
				{
					Exp e = genExpr(depth - 1);

					newExpr = NEG(e);
				}
					break;
				case 4:
				{
					Exp left = genExpr(depth - 1);
					Exp right = genExpr(depth - 1);

					newExpr = OR(left, right);
				}
					break;
				default:
					Debug.Assert(false);
				break;
			}

			Debug.Assert(newExpr != null);

			return newExpr;
		}
	}

}