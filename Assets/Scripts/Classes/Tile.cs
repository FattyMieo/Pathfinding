public abstract class Tile
{
	public int x;
	public int y;

	public Tile(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public abstract int totalDirections { get; }

	public abstract int RoundToDir(int dir);
}