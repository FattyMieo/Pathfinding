public static class AxialHex
{
	private static Hex[] directions =
	{
		new Hex( 0,-1), new Hex( 1,-1), new Hex( 1, 0),
		new Hex( 0, 1), new Hex(-1, 1), new Hex(-1, 0)
	};

	private static float[] cost =
	{
		1.0f, 1.0f, 1.0f,
		1.0f, 1.0f, 1.0f
	};

	public static int totalDirections { get { return directions.Length; } }

	public static int RoundToDir(int dir)
	{
		if (dir < 0) return totalDirections - (dir % totalDirections);
		return dir % totalDirections;
	}

	public static Hex GetDirection(int dir)
	{
		Hex h = new Hex();
		h.x = directions [RoundToDir(dir)].x;
		h.y = directions [RoundToDir(dir)].y;
		return h;
	}

	public static Hex[] GetDirections()
	{
		Hex[] h = new Hex[totalDirections];
		for(int i = 0; i < totalDirections; i++)
		{
			h[i] = new Hex();
			h[i].x = directions[i].x;
			h[i].y = directions[i].y;
		}
		return h;
	}

	public static float GetCost(int dir)
	{
		float c = cost[RoundToDir(dir)];
		return c;
	}
}

[System.Serializable]
public class Hex : Tile
{
	//Constructor
	public Hex(int x, int y) : base(x, y) { }
	public Hex() : base(0, 0) { }

	//Functions
	public override int totalDirections
	{
		get { return AxialHex.totalDirections; }
	}

	public override int RoundToDir(int dir)
	{
		return AxialHex.RoundToDir(dir);
	}

	public Hex GetDirection(int dir)
	{
		return AxialHex.GetDirection(dir);
	}

	public Hex[] GetDirections()
	{
		return AxialHex.GetDirections();
	}

	public float GetCost(int dir)
	{
		return AxialHex.GetCost(dir);
	}

	public Hex GetNeighbour(int dir)
	{
		Hex neighDir = GetDirection(dir);
		return this + neighDir; //new AxialHex(x + neighDir.x, y + neighDir.y);
	}

	public Hex[] GetNeighbours()
	{
		Hex[] neighDirs = GetDirections();
		for(int i = 0; i < neighDirs.Length; i++)
		{
			neighDirs[i] += this; //new AxialHex(x + neighDirs[i].x, y + neighDirs[i].y);
		}
		return neighDirs;
	}

	//Operators
	public static Hex operator+(Hex self, Hex other)
	{
		return new Hex(self.x + other.x, self.y + other.y);
	}

	public static Hex operator-(Hex self, Hex other)
	{
		return new Hex(self.x - other.x, self.y - other.y);
	}

	public static Hex operator*(Hex self, Hex other)
	{
		return new Hex(self.x * other.x, self.y * other.y);
	}

	public static Hex operator/(Hex self, Hex other)
	{
		return new Hex(self.x / other.x, self.y / other.y);
	}

	public static Hex operator*(Hex self, int other)
	{
		return new Hex(self.x * other, self.y * other);
	}

	public static Hex operator/(Hex self, int other)
	{
		return new Hex(self.x / other, self.y / other);
	}

	public static Hex operator*(int other, Hex self)
	{
		return self * other;
	}

	public static Hex operator/(int other, Hex self)
	{
		return self / other;
	}
}

public static class AxialHexExtension
{
	public static Hex ToArrayPos(this Hex self, int radius)
	{
		if(self.x == 0 && self.y == 0) return self;
		return new Hex(self.x + radius - 1, self.y + radius - 1);
	}
}