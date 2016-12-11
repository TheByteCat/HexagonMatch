using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonMatch
{
    class Hex
    {
        public Hex(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
        }
        public readonly int q;
        public readonly int r;
        public readonly int s;

        static public Hex Add(Hex a, Hex b)
        {
            return new Hex(a.q + b.q, a.r + b.r, a.s + b.s);
        }

        static public Hex Subtract(Hex a, Hex b)
        {
            return new Hex(a.q - b.q, a.r - b.r, a.s - b.s);
        }

        static public Hex Scale(Hex a, int k)
        {
            return new Hex(a.q * k, a.r * k, a.s * k);
        }

        static public List<Hex> directions = new List<Hex> { new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1) };

        static public Hex Direction(int direction)
        {
            return Hex.directions[direction];
        }

        static public Hex Neighbor(Hex hex, int direction)
        {
            return Hex.Add(hex, Hex.Direction(direction));
        }

        static public List<Hex> diagonals = new List<Hex> { new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1), new Hex(1, 1, -2) };

        static public Hex DiagonalNeighbor(Hex hex, int direction)
        {
            return Hex.Add(hex, Hex.diagonals[direction]);
        }

        static public int Length(Hex hex)
        {
            return (int)((Math.Abs(hex.q) + Math.Abs(hex.r) + Math.Abs(hex.s)) / 2);
        }

        static public int Distance(Hex a, Hex b)
        {
            return Hex.Length(Hex.Subtract(a, b));
        }
        public static bool operator ==(Hex h1, Hex h2)
        {
            return (h1.q == h2.q) && (h1.r == h2.r) && (h1.s == h2.s);
        }
        public static bool operator !=(Hex h1, Hex h2)
        {
            return (h1.q != h2.q) || (h1.r != h2.r) || (h1.s != h2.s);
        }
        public static Hex operator +(Hex h1, Hex h2)
        {
            return Hex.Add(h1, h2);
        }
        public static Hex operator -(Hex h1, Hex h2)
        {
            return Hex.Subtract(h1, h2);
        }
        public static bool IsNeighbor(Hex a, Hex b)
        {
            return directions.Contains(a - b);
        }
        public override bool Equals(object obj)
        {
            return this == (Hex)obj;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    enum HexagonType { Empty = 0, Element, Block }
    enum HexagonElement { None = -1, Blue = 0, Brown, Green, Grey, Leaf, Water, Violet, LightGreen, Orange };

    struct HexagonContent
    {
        HexagonType type;
        HexagonElement element;

        internal HexagonType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
                if (value == HexagonType.Block || value == HexagonType.Empty)
                    element = HexagonElement.None;
            }
        }
        internal HexagonElement Element
        {
            get
            {
                if (type == HexagonType.Block || type == HexagonType.Empty)
                    return HexagonElement.None;
                else
                    return element;
            }

            set
            {
                element = value;
                if (value == HexagonElement.None)
                    type = HexagonType.Empty;
            }
        }

        public HexagonContent(HexagonType type = HexagonType.Empty, HexagonElement element = HexagonElement.None)
        {
            this.type = type;
            this.element = element;
        }

        public static HexagonContent Empty
        {
            get
            {
                return new HexagonContent();
            }
        }
        public static HexagonContent Block
        {
            get
            {
                return new HexagonContent(HexagonType.Block);
            }
        }

    }

    class Hexagon
    {
        Hex hex;
        Color currentColor;
        public HexagonContent Content;

        static float sqrt3 = (float)Math.Sqrt(3);
        private static Grid ownerGrid;
        internal static Grid OwnerGrid
        {
            get
            {
                return ownerGrid;
            }

            set
            {
                ownerGrid = value;
            }
        }
        public static Hex HexRound(float Q, float R, float S)
        {
            int q = (int)(Math.Round(Q));
            int r = (int)(Math.Round(R));
            int s = (int)(Math.Round(S));
            double q_diff = Math.Abs(q - Q);
            double r_diff = Math.Abs(r - R);
            double s_diff = Math.Abs(s - S);
            if (q_diff > r_diff && q_diff > s_diff)
            {
                q = -r - s;
            }
            else
                if (r_diff > s_diff)
            {
                r = -q - s;
            }
            else
            {
                s = -q - r;
            }
            return new Hex(q, r, s);
        }
        public static bool IsNeighbor(Hexagon a, Hexagon b)
        {
            if (a != null && b != null)
                return Hex.IsNeighbor(a.Hex, b.Hex);
            else
                return false;
        }

        public Vector2 Position
        {
            get
            {
                float x = ownerGrid.HexagonSize * 3.0f / 2.0f * hex.q - ownerGrid.HexagonSize;
                float y = ownerGrid.HexagonSize * sqrt3 * (hex.r + hex.q / 2.0f) - sqrt3 / 2.0f * ownerGrid.HexagonSize;
                return new Vector2(x, y) + ownerGrid.Center;
            }
        }
        public Vector2 Center
        {
            get
            {
                return Position + new Vector2(Texture.Width / 2.0f * ownerGrid.NormalScale.X, Texture.Height / 2.0f * ownerGrid.NormalScale.Y);
            }
        }
        public Vector2 Scale
        {
            get { return new Vector2((2.0f * ownerGrid.HexagonSize) / Texture.Width, (sqrt3 * ownerGrid.HexagonSize) / Texture.Height); }
        }
        public int Q { get { return hex.q; } }
        public int R { get { return hex.r; } }
        public int S { get { return hex.s; } }
        internal Hex Hex
        {
            get
            {
                return hex;
            }

            set
            {
                hex = value;
            }
        }
        public HexagonType Type
        {
            get
            {
                return Content.Type;
            }

            set
            {
                Content.Type = value;
                if (value == HexagonType.Block || value == HexagonType.Empty)
                    Content.Element = HexagonElement.None;
            }
        }
        public Texture2D Texture
        {
            get { return ownerGrid.HexagonTexture; }
        }
        public Color CurrentColor
        {
            get
            {
                if (Type == HexagonType.Block)
                    return Color.DarkGray;
                else
                    return currentColor;
            }

            set
            {
                currentColor = value;
            }
        }

        public Hexagon(Hex hex, HexagonContent type)
        {
            this.hex = hex;
            currentColor = Color.White;
            this.Content = type;
        }
        public Hexagon(Point hex, HexagonContent type)
        {
            this.hex = new Hex(hex.X, hex.Y, (-1) * hex.X - hex.Y);
            currentColor = Color.White;
            Content = type;
        }
    }
}