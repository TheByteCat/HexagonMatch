using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HexagonMatch
{
    class Enemy
    {
        Texture2D texture;
        int maxHp, hp;
        string name;

        public Texture2D Texture
        {
            get
            {
                return texture;
            }

            set
            {
                texture = value;
            }
        }

        public int MaxHp
        {
            get
            {
                return maxHp;
            }

            set
            {
                maxHp = value;
            }
        }

        public int Hp
        {
            get
            {
                return hp;
            }

            set
            {
                hp = MathHelper.Clamp(value, 0, maxHp);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public Enemy(string name, Texture2D texture, int maxHp)
        {
            this.Texture = texture;
            this.MaxHp = maxHp;
            Hp = maxHp;
            this.Name = name;
        }
    }
}