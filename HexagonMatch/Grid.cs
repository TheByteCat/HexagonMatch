using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HexagonMatch
{
    class Grid
    {
        List<Hexagon> selectedHex;
        Hexagon[,] hexMap;
        short hexagonSize = 50;
        int mapRadius, score = 0;
        byte maxValue;
        Hexagon lastHexagon, prevHexagon;
        Vector2 normalScale;
        Random rand;
        static float Sqrt3 = (float)Math.Sqrt(3);

        public Vector2 Center
        {
            get;
            set;
        }
        public Vector2 NormalScale
        {
            get
            {
                return normalScale;
            }

            set
            {
                normalScale = value;
            }
        }
        public short HexagonSize
        {
            get
            {
                return hexagonSize;
            }

            set
            {
                hexagonSize = value;
            }
        }
        internal Hexagon[,] HexMap
        {
            get
            {
                return hexMap;
            }

            set
            {
                hexMap = value;
            }
        }
        public int Score
        {
            get
            {
                return score;
            }

            set
            {
                score = value;
            }
        }

        private Hex PixelToHex(Vector2 position)
        {
            Vector2 fixPosition = new Vector2((position.X - Center.X), (position.Y - Center.Y));
            float q = fixPosition.X * 2 / 3 / hexagonSize;
            float r = (-fixPosition.X / 3 + Sqrt3 / 3 * fixPosition.Y) / hexagonSize;
            return Hexagon.HexRound(q, r, -q - r);
        }
        private Hexagon HexToHexagon(Hex hex)
        {
            if (Math.Abs(hex.q) > mapRadius || Math.Abs(hex.r) > mapRadius)
                return null;
            else
                return hexMap[hex.q + mapRadius, hex.r + mapRadius];
        }
        public Hexagon PixelToHexagon(Vector2 position)
        {
            return HexToHexagon(PixelToHex(position));
        }
        public Hexagon GetHexagonByIndex(int q, int r)
        {
            if (Math.Abs(q) > mapRadius || Math.Abs(r) > mapRadius)
                return null;
            else
                return hexMap[q + mapRadius, r + mapRadius];
        }
        public void SetHexagonByIndex(Hexagon h, int q, int r)
        {
            if (Math.Abs(q) <= mapRadius && Math.Abs(r) <= mapRadius)
                hexMap[q + mapRadius, r + mapRadius] = h;
        }
        public Hexagon GetHexagonByHex(Hex hex)
        {
            if (Math.Abs(hex.q) > mapRadius || Math.Abs(hex.r) > mapRadius)
                return null;
            else
                return hexMap[hex.q + mapRadius, hex.r + mapRadius];
        }
        public void SetHexagonByHex(Hexagon h, Hex hex)
        {
            if (Math.Abs(hex.q) <= mapRadius && Math.Abs(hex.r) <= mapRadius)
            {
                if (h != null)
                    h.Hex = hex;
                hexMap[hex.q + mapRadius, hex.r + mapRadius] = h;
            }
        }
        public void AddHexagon(Hexagon h)
        {
            hexMap[h.Q + mapRadius, h.R + mapRadius] = h;
        }
        public void RemoveHexagon(Hexagon h)
        {
            hexMap[h.Q + mapRadius, h.R + mapRadius] = null;
        }

        public Grid(short hexagonSize, Vector2 center, byte maxValue, Vector2 scale, int mapRadius = 3)
        {
            this.hexagonSize = hexagonSize;
            Center = center;
            hexMap = new Hexagon[mapRadius * 2 + 1, mapRadius * 2 + 1];
            selectedHex = new List<Hexagon>((mapRadius * 2 + 1) *(mapRadius * 2 + 1));
            this.maxValue = maxValue;
            this.mapRadius = mapRadius;
            normalScale = scale;
            rand = new Random();
        }

        //public Hexagon Neighbor(Hexagon a, int direction)
        //{
        //    return HexToHexagon(Hex.Neighbor(a.Hex, direction));
        //}

        private void NormalizeCell(Hex h)
        {
            if (GetHexagonByHex(h) == null)
            {
                Hex nextHex = h, currHex = h;
                while (currHex.r > -mapRadius && currHex.s < mapRadius)
                {
                    nextHex = Hex.Neighbor(currHex, 2);
                    if (GetHexagonByHex(nextHex) == null)
                        NormalizeCell(nextHex);
                    SetHexagonByHex(GetHexagonByHex(nextHex), currHex);
                    currHex = nextHex;
                }
                SetHexagonByHex(new Hexagon(currHex, (byte)(rand.Next(maxValue) + 1), hexagonSize, Game1.colors[rand.Next(Game1.colors.Length)]), currHex);
            }
        }

        /// <summary>
        /// Filling the empty cells of the upper cell's values and randomly generating new if it necessary
        /// </summary>
        public void Normalize()
        {
            foreach (Hexagon h in selectedHex)
            {
                NormalizeCell(h.Hex);
            }
        }

        public void Update(GameTime gameTime, TouchCollection touch)
        {
            if (touch.Count == 1)
            {
                Hexagon h = PixelToHexagon(touch[0].Position);
                if (h != null)
                {
                    if (lastHexagon == null)
                    {
                        h.CurrentColor = Color.Violet;
                        selectedHex.Add(h);
                        lastHexagon = h;
                    }
                    else if (h == prevHexagon)
                    {
                        lastHexagon.CurrentColor = lastHexagon.BaseColor;
                        selectedHex.Remove(selectedHex.Last());
                        lastHexagon = prevHexagon;
                        prevHexagon = selectedHex.Count > 1 ? selectedHex[selectedHex.Count - 2] : null;
                    }
                    else if ((h != lastHexagon) && Hexagon.IsNeighbor(lastHexagon, h) && (Math.Abs(h.Value - lastHexagon.Value) == 1 || Math.Abs(h.Value - lastHexagon.Value) == maxValue - 1))
                    {
                        h.CurrentColor = Color.Violet;
                        selectedHex.Add(h);
                        prevHexagon = lastHexagon;
                        lastHexagon = h;
                    }
                }
            }
            else
            {
                if (selectedHex.Count > 2)
                {
                    score += selectedHex.Count * 5;
                    foreach (Hexagon h in selectedHex)
                        SetHexagonByHex(null, h.Hex);
                    Normalize();
                }
                else
                {
                    foreach (Hexagon h in selectedHex)
                        h.CurrentColor = h.BaseColor;
                }
                selectedHex.Clear();
                lastHexagon = prevHexagon = null;
                //prevHexagon = null;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();            
            foreach (Hexagon h in hexMap)
            {
                if (h != null)
                {
                    spriteBatch.Draw(Hexagon.Texture, position: h.Position, scale: h.Scale, color: h.CurrentColor);
                    var size = font.MeasureString(h.Value.ToString()) * normalScale.X / 2;
                    spriteBatch.DrawString(font, h.Value.ToString(), h.Center - size, Color.Black, 0, Vector2.Zero, normalScale.X, SpriteEffects.None, 0);
                }
            }
            spriteBatch.End();
        }

    }
}