using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Accelerated_Delivery_Win
{
    public static class Resources
    {
        private static ContentManager content;

        public static Texture2D AchievementToastTexture;
        public static Texture2D BarTexture;
        public static BaseModel beachOuterModel;
        public static BaseModel beachSkyboxModel;
        public static Texture2D BeachTexture;
        public static SpriteFont BiggerFont;
        public static Model boxModel;
        public static Model blueBoxModel;
        public static Model blackBoxModel;
        public static Sprite DestroyedBoxesBase;
        public static Sprite DestroyedBoxesText;
        public static Texture2D EmptyTex;
        public static SpriteFont Font;
        public static BaseModel genericOuterModel;
        //public static BaseModel genericSkyboxModel;
        public static Texture2D halfBlack;
        public static BaseModel iceOuterModel;
        public static BaseModel iceSkyboxModel;
        public static Texture2D IceTexture;
        public static Texture2D LaserTexture;
        public static Effect LaserShader;
        public static BaseModel lavaOuterModel;
        public static BaseModel lavaSkyboxModel;
        public static SpriteFont LCDFont;
        public static Texture2D LCDNumbers;
        public static Sprite LevelOverlay;
        public static Texture2D MetalTex;
        public static Sprite[] OverlayWords;
        public static Texture2D Plus1;
        public static Sprite RemainingBoxesBase;
        public static Sprite RemainingBoxesText;
        public static Sprite ScoreboardBase;
        public static Sprite ScoreboardText;
        public static BaseModel skyOuterModel;
        public static BaseModel skySkyboxModel;
        public static Texture2D starTex;
        public static BaseModel spaceOuterModel;
        public static BaseModel spaceSkyboxModel;
        public static Sprite SurvivingBoxesBase;
        public static Sprite SurvivingBoxesText;
        public static Sprite TimeElapsedBase;
        public static Sprite TimeElapsedText;
        public static Model tubeModel;
        public static Dictionary<int, Rectangle> UINumbers;
        public static Texture2D WaterBumpMap;

        public static void Initialize(ContentManager cont)
        {
            content = cont;
        }

        /// <summary>
        /// Gets a resource from the content manager. It's recommended to pre-load your content to accelerate this process.
        /// </summary>
        /// <typeparam name="T">The type of the returned resource.</typeparam>
        /// <param name="name">The name (exactly as you would call content.Load()) of the resource.</param>
        /// <returns>The requested resource.</returns>
        public static T GetResourceFromManager<T>(string name)
        {
            return content.Load<T>(name);
        }
    }
}