#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace HexagonMatch
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields
        
        public TouchCollection TouchState;
        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        #endregion


        
        public void Update()
        {
            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                var g = TouchPanel.ReadGesture();
                var p1 = Vector2.Transform(g.Position - Screen.InputTranslate, Screen.InputScale);
                var p2 = Vector2.Transform(g.Position2 - Screen.InputTranslate, Screen.InputScale);
                var p3 = Vector2.Transform(g.Delta - Screen.InputTranslate, Screen.InputScale);
                var p4 = Vector2.Transform(g.Delta2 - Screen.InputTranslate, Screen.InputScale);
                g = new GestureSample(g.GestureType, g.Timestamp, p1, p2, p3, p4);
                Gestures.Add(g);
            }
        }

        public Screen Screen
        {
            get;
            set;
        }
    }
}