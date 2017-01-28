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
        Hexagon[,] hexMap, bufferMap;
        short hexagonSize = 50;
        int mapRadius;
        byte maxValue;
        Hexagon lastHexagon, prevHexagon;
        //Vector2 normalScale;
        Random rand;
        GridAnimator animator;
        static float Sqrt3 = (float)Math.Sqrt(3);
        Texture2D hexagonTexture, wallTexture;
        //public static Color[] colors = { Color.LightSkyBlue, Color.SeaGreen, Color.White, Color.MediumTurquoise, Color.Aqua, Color.LightGreen };
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
                return new Vector2((2.0f * HexagonSize) / hexagonTexture.Width, (Sqrt3 * HexagonSize) / hexagonTexture.Height);
            }
        }
        public short HexagonSize { get { return hexagonSize; }}
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
            }
        }
        public Texture2D WallTexture
        {
            get
            {
                return wallTexture;
            }

            set
            {
                wallTexture = value;
            }
        }
        internal List<Hexagon> SelectedHex
        {
            get
            {
                return selectedHex;
            }
        }
        internal Hexagon[,] BufferMap
        {
            get
            {
                return bufferMap;
            }

            set
            {
                bufferMap = value;
            }
        }
        public int MapRadius
        {
            get
            {
                return mapRadius;
            }

            set
            {
                mapRadius = value;
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

        public Hex PixelToHex(Vector2 position)
        {
            Vector2 fixPosition = new Vector2((position.X - Center.X), (position.Y - Center.Y));
            float q = fixPosition.X * 2 / 3 / hexagonSize;
            float r = (-fixPosition.X / 3 + Sqrt3 / 3 * fixPosition.Y) / hexagonSize;
            return Hexagon.HexRound(q, r, -q - r);
        }
        public Vector2 HexToPixel(Hex hex)
        {
            float x = HexagonSize * 3.0f / 2.0f * hex.q - HexagonSize;
            float y = HexagonSize * Sqrt3 * (hex.r + hex.q / 2.0f) - Sqrt3 / 2.0f * HexagonSize;
            return new Vector2(x, y) + Center;
        }
        public Hexagon HexToHexagon(Hex hex)
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
        /// <summary>
        /// Return Hexagon from HexMap by Hex coordinates
        /// </summary>
        public Hexagon GetHex(Hex hex)
        {
            return GetHexagonByHex(hex);
        }
        public void SetHexInfo(Hex hex, HexagonContent info)
        {
            if (Math.Abs(hex.q) <= mapRadius && Math.Abs(hex.r) <= mapRadius)
            {
                hexMap[hex.q + mapRadius, hex.r + mapRadius].Content = info;
            }
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
        public void CopyMapToBuffer()
        {
            int len = mapRadius * 2 + 1;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    bufferMap[i, j] = Hexagon.Copy(hexMap[i, j]);
                }
            }
        }
        public void SwapMaps()
        {
            Hexagon[,] temp = hexMap;
            hexMap = bufferMap;
            bufferMap = temp;
        }

        public Grid(short hexagonSize, Vector2 center, byte maxValue, int mapRadius = 3)
        {
            this.hexagonSize = hexagonSize;
            Center = center;
            hexMap = new Hexagon[mapRadius * 2 + 1, mapRadius * 2 + 1];
            bufferMap = new Hexagon[mapRadius * 2 + 1, mapRadius * 2 + 1];
            selectedHex = new List<Hexagon>((mapRadius * 2 + 1) * (mapRadius * 2 + 1));
            this.maxValue = maxValue;
            this.mapRadius = mapRadius;
            rand = new Random();
            animator = new GridAnimator(this, 500f);
        }

        public void FieldCell(Hex hex)
        {
            Hexagon h = GetHex(hex);
            if (h.Type == HexagonType.Empty)
            {
                if (h.S == mapRadius || h.R == -mapRadius)
                {
                    SetHexInfo(hex, new HexagonContent((HexagonElement)rand.Next(maxValue)));
                    animator.AddAnimation(new AnimationInfo(Hex.Neighbor(hex, 2), hex, h.Content));
                }
                else
                {
                    HexagonContent hexContent = HexagonContent.Empty;
                    for (int i = 0; i < 3; i++)
                    {
                        Hexagon tempHex = GetHex(Hex.Neighbor(hex, ((i + 1) % 3) + 1));//get hex in order : up -> left-up -> right-up
                        if (tempHex != null && tempHex.Type != HexagonType.Block && !tempHex.Content.Frozen && !h.HaveWall(((i + 1) % 3) + 1))
                        {
                            hexContent = tempHex.Content;
                            //
                            animator.AddAnimation(new AnimationInfo(tempHex.Hex, hex, hexContent));
                            //
                            tempHex.Content = HexagonContent.Empty;
                            FieldCell(tempHex.Hex);
                            break;
                        }
                    }
                    h.Content = hexContent;
                }
            }
        }

        /// <summary>
        /// Filling the empty cells of the upper cell's values and randomly generating new if it necessary
        /// </summary>
        public void Normalize()
        {
            animator.StartNormalize();
            SwapMaps();
            for (int layer = mapRadius; layer >= -mapRadius; layer--)
            {
                Hex upperHex = new Hex(0, (-1) * layer, layer);
                FieldCell(upperHex);//Field upper cell in leyer
                animator.NextTurn();
                Hex leftHex = Hex.Neighbor(upperHex, 4);
                Hex rightHex = Hex.Neighbor(upperHex, 0);
                for (int i = 0; i < mapRadius + Math.Min(0, layer); i++)
                {
                    FieldCell(leftHex);
                    animator.NextTurn();
                    FieldCell(rightHex);
                    animator.NextTurn();
                    leftHex = Hex.Neighbor(leftHex, 4);
                    rightHex = Hex.Neighbor(rightHex, 0);
                }
            }
            SwapMaps();
            animator.AnimationStart();
        }

        private void Defreeze()
        {
            foreach(Hexagon h in selectedHex)
            {
                for (int i = 0; i < 5; i++)
                {
                    Hexagon temp = GetHexagonByHex(Hex.Neighbor(h.Hex, i));
                    if(temp != null)
                        temp.Content.Frozen = false;
                }
            }
        }

        private bool IsValidNextHexagon(Hexagon h)
        {            
            return (h != lastHexagon) &&
                Hexagon.IsNeighbor(lastHexagon, h) &&
                h.Content.Type == HexagonType.Element &&
                h.Content.Element == lastHexagon.Content.Element &&
                !selectedHex.Contains(h) &&
                !h.Content.Frozen;
        }

        private void InputHandling(TouchCollection touch)
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
                    else if (IsValidNextHexagon(h) && !lastHexagon.HaveWall(lastHexagon.Hex, h.Hex))
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
                    NormalizeStart?.Invoke(this);
                    foreach (Hexagon h in selectedHex)
                    {
                        SetHexInfo(h.Hex, HexagonContent.Empty);
                        h.CurrentColor = Color.White;
                    }                    
                    Defreeze();
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

        public void Update(GameTime gameTime, TouchCollection touch)
        {
            if (animator.IsEnable)
                animator.Update(gameTime);
            else
                InputHandling(touch);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            //spriteBatch.Begin();
            foreach (Hexagon h in hexMap)
            {
                if (h != null)
                {
                    spriteBatch.Draw(hexagonTexture, position: h.Position, scale: h.Scale, color: h.CurrentColor);
                    for (int i = 0; i < 6; i++)
                    {
                        if(h.HaveWall(i))
                        {
                            spriteBatch.Draw(wallTexture, position: h.Center, rotation: -i * (float)Math.PI / 3.0f, scale: h.Scale, color: Color.White);
                        }
                    }
                    if (h.Content.Type == HexagonType.Element)
                    {
                        spriteBatch.Draw(Elements,
                            position: h.Center - new Vector2(Elements.Height / 2.0f) * h.Scale,
                            sourceRectangle: ElementSource(h.Content.Element),
                            scale: h.Scale, 
                            color: Color.White);
                    }
                }
            }            
            //spriteBatch.End();
            animator.Draw(spriteBatch);
        }

    }
}