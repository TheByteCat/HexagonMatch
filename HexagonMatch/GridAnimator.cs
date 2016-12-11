using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonMatch
{
    class FallAnimation
    {
        Vector2 startPosition, endPosition, currentPosition;
        HexagonElement element;

        public FallAnimation(Vector2 startPosition, Vector2 endPosition, HexagonElement element)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.element = element;
            currentPosition = startPosition;
        }

        public bool AnimationDone
        {
            get { return currentPosition.Y >= endPosition.Y; }
        }

        public void Update(GameTime gameTime, Vector2 speed)
        {
            if (!AnimationDone)
            {
                currentPosition += speed;
                currentPosition = !AnimationDone ? currentPosition : endPosition;
            }
        }
    }

    class GridAnimator
    {
    }
}