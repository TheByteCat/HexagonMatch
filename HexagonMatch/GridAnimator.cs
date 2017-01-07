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
        HexagonContent content;
        Hex end;

        public FallAnimation(Vector2 startPosition, Vector2 endPosition, HexagonContent content, Hex endHex)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.content = content;
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
        internal Hex End
        {
            get
            {
                return end;
            }
        }
        internal HexagonContent Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
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
        public HexagonContent Content;
        public int Turn;

        public AnimationInfo(Hex start, Hex end, HexagonContent content)
        {
            Start = start;
            End = end;
            Content = content;
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
                            grid.GetHexagonByHex(a.End).Content = a.Content;
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
                        var curr = info.First;
                        int t = curr.Value.Turn;
                        bool[] freeColl = new bool[2 * grid.MapRadius + 1];
                        freeColl[curr.Value.Start.q + grid.MapRadius] = true;
                        do
                        {
                            if (curr.Value.Turn == t || !freeColl[curr.Value.Start.q + grid.MapRadius])
                            {
                                freeColl[curr.Value.Start.q + grid.MapRadius] = true;
                                if (grid.GetHexagonByHex(curr.Value.Start) != null)
                                    grid.GetHexagonByHex(curr.Value.Start).Content = HexagonContent.Empty;
                                Vector2 delta = new Vector2(grid.HexagonTexture.Width / 2.0f * grid.NormalScale.X, grid.HexagonTexture.Height / 2.0f * grid.NormalScale.Y);
                                tasks.AddFirst(new FallAnimation(
                                    grid.HexToPixel(curr.Value.Start) + delta,
                                    grid.HexToPixel(curr.Value.End) + delta,
                                    curr.Value.Content,
                                    curr.Value.End
                                    ));
                                var next = curr.Next;
                                info.Remove(curr);
                                if (next != null)
                                {
                                    curr = next;
                                    if (curr.Value.Turn != t && !freeColl[curr.Value.Start.q + grid.MapRadius])
                                    {
                                        t = curr.Value.Turn;                                        
                                    }
                                }
                                else
                                    break;
                            }
                            else
                            {
                                curr = curr.Next;
                            }
                        } while (curr != null);
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
                            sourceRectangle: Grid.ElementSource(a.Content.Element),
                            scale: scale,
                            color: Color.White);
            }
            spriteBatch.End();
        }

    }
}