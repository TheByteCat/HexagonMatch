using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HexagonMatch
{
    struct Hex
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
    }


    class Hexagon
    {
        Hex hex;
        short size;
        byte value;
        Color color;

        static Texture2D texture;
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
        public static Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                ownerGrid.NormalScale = new Vector2((2.0f * ownerGrid.HexagonSize) / Hexagon.Texture.Width, (sqrt3 * ownerGrid.HexagonSize) / Hexagon.Texture.Height);
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
                float x = size * 3.0f / 2.0f * hex.q - size;
                float y = size * sqrt3 * (hex.r + hex.q / 2.0f) - sqrt3 / 2.0f * size;
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
            get { return new Vector2((2.0f * size) / Hexagon.Texture.Width, (sqrt3 * size) / Hexagon.Texture.Height); }
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
        public byte Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
        public short Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }
        public Color BaseColor
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
                CurrentColor = value;
            }
        }
        public Color CurrentColor
        {
            get;
            set;
        }

        public Hexagon(Hex hex, byte value, short size, Color color)
        {
            this.hex = hex;
            this.value = value;
            this.size = size;
            this.color = color;
            CurrentColor = color;
        }
        public Hexagon(Vector3 hex, byte value, short size, Color color)
        {
            this.hex = new Hex((int)hex.X, (int)hex.Y, (int)hex.Z);
            this.value = value;
            this.size = size;
            this.color = color;
            CurrentColor = color;
        }
    }
}