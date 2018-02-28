//[System.Serializable]
//public class DiagonalSquare : Square
//{
//	private static Square[] directions =
//	{
//		new Square( 0, 1), new Square( 1, 1),
//		new Square( 1, 0), new Square( 1,-1),
//		new Square( 0,-1), new Square(-1,-1),
//		new Square(-1, 0), new Square(-1, 1)
//	};
//
//	public static int totalDirections { get { return directions.Length; } }
//
//	public static int RoundToDir(int d)
//	{
//		if (d < 0) return totalDirections - (d % totalDirections);
//		return d % totalDirections;
//	}
//
//	public static Square GetDirection(int dir)
//	{
//		return directions [RoundToDir(dir)];
//	}
//
//	public static Square[] GetDirections()
//	{
//		return directions;
//	}
//}
//
//public static class DiagonalSquareExtension
//{
//	public static DiagonalSquare ToArrayPos(this DiagonalSquare self, int radius)
//	{
//		return new DiagonalSquare(self.x + radius - 1, self.y + radius - 1);
//	}
//}