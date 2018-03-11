public static class DiagonalSquare
{
	private static Square[] directions =
	{
		new Square( 0, 1), new Square( 1, 1),
		new Square( 1, 0), new Square( 1,-1),
		new Square( 0,-1), new Square(-1,-1),
		new Square(-1, 0), new Square(-1, 1)
	};
	
	private static float[] cost =
	{
		1.0f, MathExtension.sqrt2,
		1.0f, MathExtension.sqrt2,
		1.0f, MathExtension.sqrt2,
		1.0f, MathExtension.sqrt2
	};

	public static int totalDirections { get { return directions.Length; } }

	public static int RoundToDir(int dir)
	{
		if (dir < 0) return totalDirections - (dir % totalDirections);
		return dir % totalDirections;
	}

	public static Square GetDirection(int dir)
	{
		Square s = new Square();
		s.x = directions [RoundToDir(dir)].x;
		s.y = directions [RoundToDir(dir)].y;
		return s;
	}

	public static Square[] GetDirections()
	{
		Square[] s = new Square[totalDirections];
		for(int i = 0; i < totalDirections; i++)
		{
			s[i] = new Square();
			s[i].x = directions[i].x;
			s[i].y = directions[i].y;
		}
		return s;
	}

	public static float GetCost(int dir)
	{
		float c = cost[RoundToDir(dir)];
		return c;
	}
}

public static class NDiagonalSquare
{
	private static Square[] directions =
	{
		new Square( 0, 1),
		new Square( 1, 0),
		new Square( 0,-1),
		new Square(-1, 0)
	};
	
	private static float[] cost =
	{
		1.0f,
		1.0f,
		1.0f,
		1.0f
	};
	
	public static int totalDirections { get { return directions.Length; } }

	public static int RoundToDir(int dir)
	{
		if (dir < 0) return totalDirections - (dir % totalDirections);
		return dir % totalDirections;
	}

	public static Square GetDirection(int dir)
	{
		Square s = new Square();
		s.x = directions [RoundToDir(dir)].x;
		s.y = directions [RoundToDir(dir)].y;
		return s;
	}

	public static Square[] GetDirections()
	{
		Square[] s = new Square[totalDirections];
		for(int i = 0; i < totalDirections; i++)
		{
			s[i] = new Square();
			s[i].x = directions[i].x;
			s[i].y = directions[i].y;
		}
		return s;
	}

	public static float GetCost(int dir)
	{
		float c = cost[RoundToDir(dir)];
		return c;
	}
}

[System.Serializable]
public class Square : Tile
{
	//Getters
	public bool isDiagonal
	{
		get { return PathfindingManager.instance.checkDiagonals; }
	}

	//Constructors
	public Square(int x, int y) : base(x, y) { }
	public Square() : base(0, 0) { }

	//Functions
	public override int totalDirections
	{
		get { return (isDiagonal ? DiagonalSquare.totalDirections : NDiagonalSquare.totalDirections); }
	}

	public override int RoundToDir(int dir)
	{
		return (isDiagonal ? DiagonalSquare.RoundToDir(dir) : NDiagonalSquare.RoundToDir(dir));
	}

	public Square GetDirection(int dir)
	{
		return (isDiagonal ? DiagonalSquare.GetDirection(dir) : NDiagonalSquare.GetDirection(dir));
	}

	public Square[] GetDirections()
	{
		return (isDiagonal ? DiagonalSquare.GetDirections() : NDiagonalSquare.GetDirections());
	}

	public float GetCost(int dir)
	{
		return (isDiagonal ? DiagonalSquare.GetCost(dir) : NDiagonalSquare.GetCost(dir));
	}

	public Square GetNeighbour(int dir)
	{
		Square neighDir = GetDirection(dir);
		return this + neighDir; //new AxialHex(x + neighDir.x, y + neighDir.y);
	}

	public Square[] GetNeighbours()
	{
		Square[] neighDirs = GetDirections();
		for(int i = 0; i < neighDirs.Length; i++)
		{
			neighDirs[i] += this; //new AxialHex(x + neighDirs[i].x, y + neighDirs[i].y);
		}
		return neighDirs;
	}

	//Operators
	public static Square operator+(Square self, Square other)
	{
		return new Square(self.x + other.x, self.y + other.y);
	}

	public static Square operator-(Square self, Square other)
	{
		return new Square(self.x - other.x, self.y - other.y);
	}

	public static Square operator*(Square self, Square other)
	{
		return new Square(self.x * other.x, self.y * other.y);
	}

	public static Square operator/(Square self, Square other)
	{
		return new Square(self.x / other.x, self.y / other.y);
	}

	public static Square operator*(Square self, int other)
	{
		return new Square(self.x * other, self.y * other);
	}

	public static Square operator/(Square self, int other)
	{
		return new Square(self.x / other, self.y / other);
	}

	public static Square operator*(int other, Square self)
	{
		return self * other;
	}

	public static Square operator/(int other, Square self)
	{
		return self / other;
	}
}