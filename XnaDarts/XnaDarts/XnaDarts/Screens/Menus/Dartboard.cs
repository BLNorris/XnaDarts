﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaDarts.ScreenManagement;

namespace XnaDarts.Screens.Menus
{
    public class Dartboard
    {
        public static int[] SegmentRotation = {1, 8, 10, 3, 19, 5, 12, 14, 17, 6, 15, 18, 4, 16, 7, 13, 9, 2, 11, 0};
        public static int[] SegmentOrder = {20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5};
        private Texture2D _doubleBullTexture;
        private Texture2D _doubleTexture;
        private IntPair _highlight;
        private Texture2D _mapTexture;
        private Texture2D _segmentTexture;
        private Texture2D _singleBullTexture;
        private Texture2D[] _textures;
        private Texture2D _tripleTexture;
        public Vector2 Position = Vector2.Zero;
        public float Scale = 1.0f;
        public Dictionary<IntPair, Color> SegmentColor = new Dictionary<IntPair, Color>();
        private const float DoubleBullseyeRadius = 40.0f;
        private const float DoubleRadius = 320.0f;
        private const float SegmentDegrees = 18;
        private const float SingleBullseyeRadius = 15.0f;
        private const float TripleRadius = 190.0f;

        public Vector2 TextureCenter
        {
            get { return new Vector2(_mapTexture.Width, _mapTexture.Height)*0.5f; }
        }

        public event SerialManager.DartRegisteredDelegate OnSegmentClicked;

        public void LoadContent(ContentManager content)
        {
            _mapTexture = content.Load<Texture2D>(@"Images\SegmentMap");
            _segmentTexture = content.Load<Texture2D>(@"Images\Segment");
            _tripleTexture = content.Load<Texture2D>(@"Images\Triple");
            _doubleTexture = content.Load<Texture2D>(@"Images\Double");
            _singleBullTexture = content.Load<Texture2D>(@"Images\SingleBull");
            _doubleBullTexture = content.Load<Texture2D>(@"Images\DoubleBull");

            _textures = new[] {_segmentTexture, _doubleTexture, _tripleTexture, _singleBullTexture, _doubleBullTexture};
        }

        public void HandleInput(InputState input)
        {
            var segment = GetSegment(new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y));
            if (segment != null)
            {
                if (input.MouseClick)
                {
                    if (OnSegmentClicked != null)
                    {
                        OnSegmentClicked(segment.X, segment.Y);
                    }
                }
            }

            _highlight = segment;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Position = new Vector2(XnaDartsGame.Viewport.Width, XnaDartsGame.Viewport.Height)*0.5f;

            var transform = Matrix.CreateScale(Scale)*Matrix.CreateTranslation(new Vector3(Position, 0));

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transform);

            spriteBatch.Draw(_mapTexture, Vector2.Zero, null, Color.White, 0, TextureCenter, 1.0f, SpriteEffects.None, 0);

            float rotation;
            Texture2D texture;

            foreach (var p in SegmentColor)
            {
                getTextureAndRotation(p.Key, out texture, out rotation);

                spriteBatch.Draw(texture, Vector2.Zero, null, p.Value, rotation, TextureCenter, 1.0f, SpriteEffects.None,
                    0);
            }

            if (_highlight != null)
            {
                getTextureAndRotation(_highlight, out texture, out rotation);
                spriteBatch.Draw(texture, Vector2.Zero, null, Color.Yellow, rotation, TextureCenter, 1.0f,
                    SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        private void getTextureAndRotation(IntPair segment, out Texture2D texture, out float rotation)
        {
            if (segment.X == 25)
            {
                texture = _textures[2 + segment.Y];
                rotation = 0;
            }
            else
            {
                texture = _textures[segment.Y - 1];
                rotation = MathHelper.ToRadians(SegmentRotation[segment.X - 1]*SegmentDegrees);
            }
        }

        public IntPair GetSegment(Vector2 position)
        {
            var halfSize = TextureCenter*Scale;
            var distanceToCenter = Vector2.Distance(position, Position);

            if (distanceToCenter < (DoubleRadius + 30.0f)*Scale)
            {
                if (distanceToCenter < SingleBullseyeRadius*Scale)
                {
                    return new IntPair(25, 2); // Double BullsEye
                }
                if (distanceToCenter < DoubleBullseyeRadius*Scale)
                {
                    return new IntPair(25, 1); // Single BullsEye
                }
                var segmentVector = Vector2.UnitY;

                var rotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(SegmentDegrees));

                var dx = (int) position.X - (int) Position.X;
                var dy = (int) position.Y - (int) Position.Y;

                var rotation = (float) Math.Atan2(dy, dx);

                var tempVector = Position - position;
                tempVector.Normalize();

                for (var i = 0; i < 20; i++)
                {
                    var angle = (float) Math.Acos(Vector2.Dot(segmentVector, tempVector));

                    if (Math.Abs(angle) < MathHelper.ToRadians(SegmentDegrees*0.5f))
                    {
                        var temp = new IntPair(SegmentOrder[i], 0);

                        if (distanceToCenter > TripleRadius*Scale && distanceToCenter < (TripleRadius + 30.0f)*Scale)
                            // Triple
                        {
                            temp.Y = 3;
                        }
                        else if (distanceToCenter > DoubleRadius*Scale) //Double
                        {
                            temp.Y = 2;
                        }
                        else
                        {
                            temp.Y = 1;
                        }

                        return temp;
                    }

                    segmentVector = Vector2.Transform(segmentVector, rotationMatrix);
                }
            }

            return null;
        }

        public void ColorSegment(int p, Color c)
        {
            for (var i = 0; i < 3; i++)
            {
                SegmentColor.Add(new IntPair(p, i + 1), c);
            }
        }
    }
}