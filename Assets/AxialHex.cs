using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxialHex
{
	private static AxialHex[] directions =
	{
		AxialHex.up, AxialHex.up + AxialHex.left, AxialHex.right,
		AxialHex.up, AxialHex.up + AxialHex.left, AxialHex.left
	};

	public static int totalDirections { get { return directions.Length; } }

	public static AxialHex up 		{ get { return new AxialHex ( 0,  1); } }
	public static AxialHex down 	{ get { return new AxialHex ( 0, -1); } }
	public static AxialHex left 	{ get { return new AxialHex (-1,  0); } }
	public static AxialHex right 	{ get { return new AxialHex ( 1,  0); } }

	public static int RoundToDir(int d)
	{
		if (d < 0) return totalDirections - (d % totalDirections);
		return d % totalDirections;
	}

	public static AxialHex GetDirection(int dir)
	{
		return directions [RoundToDir(dir)];
	}

	public static AxialHex[] GetDirections()
	{
		return directions;
	}

	public AxialHex GetNeighbour(int dir)
	{
		AxialHex neighDir = GetDirection(dir);
		return this + neighDir; //new AxialHex(x + neighDir.x, y + neighDir.y);
	}

	public AxialHex[] GetNeighbours()
	{
		AxialHex[] neighDirs = GetDirections();
		for(int i = 0; i < totalDirections; i++)
		{
			neighDirs[i] += this; //new AxialHex(x + neighDirs[i].x, y + neighDirs[i].y);
		}
		return neighDirs;
	}

	public static AxialHex operator+(AxialHex self, AxialHex other)
	{
		return new AxialHex(self.x + other.x, self.y + other.y);
	}

	public static AxialHex operator-(AxialHex self, AxialHex other)
	{
		return new AxialHex(self.x - other.x, self.y - other.y);
	}

	public static AxialHex operator*(AxialHex self, AxialHex other)
	{
		return new AxialHex(self.x * other.x, self.y * other.y);
	}

	public static AxialHex operator/(AxialHex self, AxialHex other)
	{
		return new AxialHex(self.x / other.x, self.y / other.y);
	}

	public int x;
	public int y;

	public AxialHex(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}
