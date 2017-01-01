using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonMatch
{
    class FallAnimation
    {
        Vector2 startPosition, endPosition, currentPosition, direction;
        HexagonElement element;
        Hex end;

        public FallAnimation(Vector2 startPosition, Vector2 endPosition, HexagonElement element, Hex endHex)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.element = element;
            end = endHex;
            currentPosition = startPosition;
            direction = endPosition - startPosition;
            direction.Normalize();
        }

        public bool AnimationDone
        {
            get { return currentPosition.Y >= endPosition.Y; }
        }

        public Vector2 StartPosition
        {
            get
            {
                return startPosition;
            }
        }
        public Vector2 EndPosition
        {
            get
            {
                return endPosition;
            }
        }
        public Vector2 CurrentPosition
        {
            get
            {
                return currentPosition;
            }
        }
        internal HexagonElement Element
        {
            get
            {
                return element;
            }
        }
        internal Hex End
        {
            get
            {
                return end;
            }
        }

        public void Update(GameTime gameTime, float speed)
        {
            if (!AnimationDone)
            {
                currentPosition += direction * speed * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                currentPosition = !AnimationDone ? currentPosition : endPosition;
            }
        }
    }

    struct AnimationInfo
    {
        public Hex Start, End;
        public HexagonElement Element;
        public int Turn;

        public AnimationInfo(Hex start, Hex end, HexagonElement element)
        {
            Start = start;
            End = end;
            Element = element;
            Turn = 0;
        }
    }

    class GridAnimator
    {
        LinkedList<FallAnimation> tasks;//Current animation
        LinkedList<AnimationInfo> info;//Store all animation
        Grid grid;
        int turn = 0;
        float speed;
        bool isEnable = false;

        public bool IsEnable
        {
            get
            {
                return isEnable;
            }

            set
            {
                isEnable = value;
            }
        }
        
        public GridAnimator(Grid g, float speed)
        {
            grid = g;
            this.speed = speed;
            tasks = new LinkedList<FallAnimation>();
            info = new LinkedList<AnimationInfo>();
        }

        public void StartNormalize()
        {
            isEnable = false;
            turn = 0;
            grid.CopyMapToBuffer();
        }

        public void NextTurn()
        {
            turn++;
        }

        public void AddAnimation(AnimationInfo animInfo)
        {
            animInfo.Turn = turn;
            info.AddLast(animInfo);
        }

        public void AnimationStart()
        {
            isEnable = true;
            turn = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (isEnable)
            {
                if (tasks.Count != 0)
                {
                    var curr = tasks.First;
                    while (curr != null)
                    {
                        var a = curr.Value;
                        a.Update(gameTime, speed);
                        if (a.AnimationDone)
                        {
                            grid.GetHexagonByHex(a.End).Content = new HexagonContent(a.Element);
                            var next = curr.Next;
                            tasks.Remove(curr);
                            if (next == null)
                                break;
                            curr = next;
                        }
                        curr = curr.Next;
                    }
                }
                else
                {
                    if (info.Count != 0)
                    {
                        int t = info.First.Value.Turn;
                        while (info.Count != 0 && info.First.Value.Turn == t)
                        {
                            if (grid.GetHexagonByHex(info.First.Value.Start) != null)
                                grid.GetHexagonByHex(info.First.Value.Start).Content = HexagonContent.Empty;
                            Vector2 delta = new Vector2(grid.HexagonTexture.Width / 2.0f * grid.NormalScale.X, grid.HexagonTexture.Height / 2.0f * grid.NormalScale.Y);
                            tasks.AddFirst(new FallAnimation(
                                grid.HexToPixel(info.First.Value.Start) + delta,
                                grid.HexToPixel(info.First.Value.End) + delta,
                                info.First.Value.Element,
                                info.First.Value.End
                                ));
                            info.RemoveFirst();
                        }
                    }
                    else
                        isEnable = false;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            float sqrt3 = (float)Math.Sqrt(3);
            foreach (FallAnimation a in tasks)
            {
                Vector2 scale = new Vector2((2.0f * grid.HexagonSize) / grid.HexagonTexture.Width, (sqrt3 * grid.HexagonSize) / grid.HexagonTexture.Height);
                spriteBatch.Draw(Grid.Elements,
                            position: a.CurrentPosition - new Vector2(Grid.Elements.Height / 2.0f) * scale,
                            sourceRectangle: Grid.ElementSource(a.Element),
                            scale: scale,
                            color: Color.White);
            }
            spriteBatch.End();
        }

    }
}