using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace HexagonMatch
{
    class Grid
    {
        List<Hexagon> selectedHex;
        Hexagon[,] hexMap;
        short hexagonSize = 50;
        int mapRadius;
        byte maxValue;
        Hexagon lastHexagon, prevHexagon;
        Vector2 normalScale;
        Random rand;
        static float Sqrt3 = (float)Math.Sqrt(3);
        Texture2D hexagonTexture;
        public static Color[] colors = { Color.LightSkyBlue, Color.SeaGreen, Color.White, Color.MediumTurquoise, Color.Aqua, Color.LightGreen };
        public static Texture2D Elements;

        public delegate void normalizeStartDelegat(Grid grid);
        public event normalizeStartDelegat NormalizeStart;
        public delegate void normalizeEndDelegat(Grid grid);
        public event normalizeEndDelegat NormalizeEnd;

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
        public Texture2D HexagonTexture
        {
            get
            {
                return hexagonTexture;
            }

            set
            {
                hexagonTexture = value;
                NormalScale = new Vector2((2.0f * HexagonSize) / hexagonTexture.Width, (Sqrt3 * HexagonSize) / hexagonTexture.Height);
            }
        }
        internal List<Hexagon> SelectedHex
        {
            get
            {
                return selectedHex;
            }
        }

        public static Rectangle ElementSource(int index)
        {
            int size = Elements.Height;
            return new Rectangle(size * index, 0, size, size);
        }
        public static Rectangle ElementSource(HexagonElement index)
        {
            int size = Elements.Height;
            return new Rectangle(size * (int)index, 0, size, size);
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
            if (h != null)
                hexMap[h.Q + mapRadius, h.R + mapRadius] = h;
        }
        public void RemoveHexagon(Hexagon h)
        {
            if (h != null)
                hexMap[h.Q + mapRadius, h.R + mapRadius] = null;
        }

        public Grid(short hexagonSize, Vector2 center, byte maxValue, Vector2 scale, int mapRadius = 3)
        {
            this.hexagonSize = hexagonSize;
            Center = center;
            hexMap = new Hexagon[mapRadius * 2 + 1, mapRadius * 2 + 1];
            selectedHex = new List<Hexagon>((mapRadius * 2 + 1) * (mapRadius * 2 + 1));
            this.maxValue = maxValue;
            this.mapRadius = mapRadius;
            normalScale = scale;
            rand = new Random();
        }

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
                SetHexagonByHex(new Hexagon(currHex, new HexagonContent(HexagonType.Element, (HexagonElement)rand.Next(maxValue))), currHex);
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

        private bool IsValidNextHexagon(Hexagon h)
        {
            return (h != lastHexagon) &&
                Hexagon.IsNeighbor(lastHexagon, h) &&
                h.Content.Type == HexagonType.Element &&
                h.Content.Element == lastHexagon.Content.Element &&
                !selectedHex.Contains(h);
        }

        public void Update(GameTime gameTime, TouchCollection touch)
        {
            if (touch.Count != 0)
            {
                Hexagon h = PixelToHexagon(touch[0].Position);
                if (h != null)
                {
                    if (lastHexagon == null)
                    {
                        h.CurrentColor = Color.Maroon;
                        selectedHex.Add(h);
                        prevHexagon = null;
                        lastHexagon = h;
                    }
                    else if (h == prevHexagon)
                    {
                        lastHexagon.CurrentColor = Color.White;
                        selectedHex.Remove(selectedHex.Last());
                        lastHexagon = prevHexagon;
                        prevHexagon = selectedHex.Count > 1 ? selectedHex[selectedHex.Count - 2] : null;
                    }
                    else if (IsValidNextHexagon(h))
                    {
                        h.CurrentColor = Color.Maroon;
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
                    foreach (Hexagon h in selectedHex)
                    {
                        SetHexagonByHex(null, h.Hex);
                    }
                    NormalizeStart?.Invoke(this);
                    Normalize();
                    NormalizeEnd?.Invoke(this);
                }
                else
                {
                    foreach (Hexagon h in selectedHex)
                        h.CurrentColor = Color.White;
                }
                selectedHex.Clear();
                lastHexagon = null;
                prevHexagon = null;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();
            foreach (Hexagon h in hexMap)
            {
                if (h != null)
                {
                    spriteBatch.Draw(hexagonTexture, position: h.Position, scale: h.Scale, color: h.CurrentColor);
                    if(h.Content.Type == HexagonType.Element)
                    {
                        spriteBatch.Draw(Elements, 
                            position: h.Center - new Vector2(Elements.Height / 2.0f) * h.Scale, 
                            sourceRectangle: ElementSource(h.Content.Element), 
                            scale: h.Scale,// * new Vector2(hexagonTexture.Height / Elements.Height) , 
                            color: Color.White);
                    }
                    //var size = font.MeasureString(h.Value.ToString()) * normalScale.X / 2;
                    //spriteBatch.DrawString(font, h.Value.ToString(), h.Center - size, Color.Black, 0, Vector2.Zero, normalScale.X, SpriteEffects.None, 0);
                }
            }
            spriteBatch.End();
        }

    }
}