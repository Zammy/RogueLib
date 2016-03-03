﻿using System.Collections.Generic;
using System;

namespace RogueLib
{
    public class LevelMng 
    {
        static LevelMng _instance = null;
        public static LevelMng Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LevelMng();
                }
                return _instance;
            }
        }

        public Tile[,] grid;

        public void LoadLevel(Tile[,] tiles)
        {
            this.grid = tiles;
        }







        private class Step : IEquatable<Step>
        {
            public Point Pos;
            public int Heuristic;
            public int FromStart;

            public Step Parent = null;

            public Step()
            {
            }

            public Step (Point pos, int Heuristic, int FromStart)
            {
                this.Pos = pos;
                this.Heuristic = Heuristic;
                this.FromStart = FromStart;
            }

            public int Score { get { return Heuristic + FromStart; } }

            #region IEquatable implementation

            public bool Equals(Step other)
            {
                if (other == null) 
                    return false;

                return other.Pos == this.Pos;
            }

            #endregion
        }

        List<Step> closedList = new List<Step>();
        List<Step> openList = new List<Step>();

        public Point[] PathFromAtoB(Point start, Point goal)
        {
            closedList.Clear();
            openList.Clear();

            openList.Add( new Step(start, (start-goal).Length, 0) );

            Step stepGoal;
            while(true)
            {
                var lowestScoreStep = GetLowestScoreFromList(openList, goal);
                if (lowestScoreStep.Pos == goal) 
                {
                    stepGoal = lowestScoreStep;
                    break;
                }

                openList.Remove(lowestScoreStep);
                closedList.Add(lowestScoreStep);

                Step[] stepsAround = GetStepsAroundStep(lowestScoreStep, goal);
                foreach(var step in stepsAround)
                {
                    if (step == null)
                    {
                        continue;
                    }

                    if (closedList.Contains(step))
                    {
                        continue;
                    }

                    var sameStep = FindStepInPos(openList, step.Pos);

                    if (sameStep != null)
                    {
                        if (sameStep.Score <= step.Score)
                        {
                            continue;
                        }

                        openList.Remove( sameStep );
                    }

                    openList.Add(step);
                }
            }

            return ExtractPath(stepGoal);
        }

        bool IsPassable(Point p)
        {
            try 
            {
                if (grid[p.X, p.Y] == null)
                    return false;
            }
            catch
            {
                return false;
            }

            return grid[p.X, p.Y].IsPassable;
        }

        Step GetLowestScoreFromList(List<Step> steps, Point goal)
        {
            int minScore = int.MaxValue;
            Step bestStep = null;
            foreach (var step in steps)
            {
                if (step.Pos == goal)
                    return step;

                // >= so that if we have steps with equal score we will add the most recent addition
                if (minScore >= step.Score)
                {
                    minScore = step.Score;
                    bestStep = step;
                }
            }
            return bestStep;
        }

        Step[] GetStepsAroundStep(Step around, Point goal)
        {
            Step[] steps = new Step[4];

            System.Func<Point, Step> addStep = (Point pos) =>
            {
                if (!this.IsPassable(pos))
                {
                    return null;
                }

                return new Step()
                {
                    Pos = pos,
                    Heuristic = (pos-goal).Length,
                    FromStart = around.FromStart + 1,
                    Parent = around
                };
            };

            steps[0] = addStep( new Point(around.Pos.X + 1, around.Pos.Y) );
            steps[1] = addStep( new Point(around.Pos.X - 1, around.Pos.Y) );
            steps[2] = addStep( new Point(around.Pos.X, around.Pos.Y + 1) );
            steps[3] = addStep( new Point(around.Pos.X, around.Pos.Y - 1) );

            return steps;
        }

        Step FindStepInPos(List<Step> steps, Point pos)
        {
            foreach (var step in steps)
            {
                if (step.Pos == pos)
                {
                    return step;
                }
            }

            return null;
        }

        Point[] ExtractPath(Step step)
        {
            List<Point> path = new List<Point>();
            step = step.Parent;
            do
            {
                path.Add(step.Pos);
                step = step.Parent;
            } 
            while(step.Parent != null);

            path.Reverse();

            return path.ToArray();
        }
    }

}