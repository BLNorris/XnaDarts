﻿using Microsoft.Xna.Framework;
using XnaDarts.Gameplay.Modes;
using XnaDarts.Screens.GameModeScreens.Components;

namespace XnaDarts.Screens.GameModeScreens
{
    public class CricketModeScreen : BaseModeScreen
    {
        public CricketModeScreen(Cricket cricket)
            : base(cricket)
        {
            GuiComponents.Add(new CricketMarksComponent(cricket));
            GuiComponents.Add(new CricketRoundMarksComponent(cricket));

            var dartScoreComponent = ((DartScoreComponent) GuiComponents.Find(x => x is DartScoreComponent));
            dartScoreComponent.Vertical = true;
            dartScoreComponent.Position = new Vector2(0.8f, 0.5f);

            GuiComponents.Remove(GuiComponents.Find(x => x is BigScoreComponent));
        }
    }
}