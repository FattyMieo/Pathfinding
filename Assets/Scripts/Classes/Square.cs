[System.Serializable]
public class Square
{
	private static Square[] directions =
	{
		new Square( 0, 1),
		new Square( 1, 0),
		new Square( 0,-1),
		new Square(-1, 0)
	};

	public static int totalDirections { get { return directions.Length; } }

	public static int RoundToDir(int d)
	{
		if (d < 0) return totalDirections - (d % totalDirections);
		return d % totalDirections;
	}

	public static Square GetDirection(int dir)
	{
		return directions [RoundToDir(dir)];
	}

	public static Square[] GetDirections()
	{
		return directions;
	}

	public Square GetNeighbour(int dir)
	{
		Square neighDir = GetDirection(dir);
		return this + neighDir; //new AxialHex(x + neighDir.x, y + neighDir.y);
	}

	public Square[] GetNeighbours()
	{
		Square[] neighDirs = GetDirections();
		for(int i = 0; i < totalDirections; i++)
		{
			neighDirs[i] += this; //new AxialHex(x + neighDirs[i].x, y + neighDirs[i].y);
		}
		return neighDirs;
	}

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

	public int x;
	public int y;

	public Square(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

public static class SquareExtension
{
	public static Square ToArrayPos(this Square self, int radius)
	{
		return new Square(self.x + radius - 1, self.y + radius - 1);
	}
}