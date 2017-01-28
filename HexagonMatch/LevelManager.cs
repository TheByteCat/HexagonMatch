using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace HexagonMatch
{
    class LevelCondition
    {
        LevelConditionInfo info;
        int progress;

        public LevelCondition(LevelConditionInfo info)
        {
            this.info = info;
            Progress = 0;
        }

        public bool Complete
        {
            get { return progress >= info.RequiredAmount; }
        }
        public int Progress
        {
            get
            {
                return progress;
            }

            set
            {
                progress = Math.Min(value, info.RequiredAmount);
            }
        }
        internal LevelConditionInfo Info
        {
            get
            {
                return info;
            }

            set
            {
                info = value;
            }
        }
    }

    struct LevelConditionInfo
    {
        public HexagonElement Element;
        public int RequiredAmount;

        public LevelConditionInfo(HexagonElement element, int requiredAmount)
        {
            Element = element;
            RequiredAmount = requiredAmount;
        }
    }

    struct LevelInfo
    {
        public int MaxSteps;
        public List<LevelConditionInfo> Conditions;

        public LevelInfo(int maxSteps, params LevelConditionInfo[] conditions)
        {
            MaxSteps = maxSteps;
            Conditions = conditions.ToList();
        }
    }

    class LevelManager : GameComponent
    {
        List<LevelCondition> conditions;
        int steps;
        Grid grid;

        public LevelManager(Game game, Grid grid, LevelInfo info) : base(game)
        {
            steps = info.MaxSteps;
            conditions = new List<LevelCondition>(info.Conditions.Count);
            foreach (LevelConditionInfo i in info.Conditions)
                conditions.Add(new LevelCondition(i));
            this.grid = grid;
            grid.NormalizeStart += CheckProgress;
        }

        public int Steps
        {
            get
            {
                return steps;
            }

            set
            {
                steps = Math.Max(0, value);
            }
        }
        internal List<LevelCondition> Conditions
        {
            get
            {
                return conditions;
            }

            set
            {
                conditions = value;
            }
        }

        private void CheckProgress(Grid grid)
        {
            int[] el = new int[5];
            bool complete = true;
            foreach (Hexagon h in grid.SelectedHex)
            {
                if (h.Content.Element != HexagonElement.None)
                    el[(int)h.Content.Element]++;
            }
            foreach (LevelCondition c in conditions)
            {
                c.Progress += el[(int)c.Info.Element];
                if (!c.Complete)
                    complete = false;
            }
            if (complete)
                CompleteLevel();
            Steps -= 1;

        }

        private void CompleteLevel()
        {
            grid.NormalizeStart -= CheckProgress;
        }
    }
}