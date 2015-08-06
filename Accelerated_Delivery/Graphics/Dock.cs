using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Accelerated_Delivery_Win
{
    public class Dock
    {
        private float maxWidth { get { return texture.Width; } }
        private float xPos { get { return texture.UpperLeft.X + texture.Width * 0.14f; } }
        private float yPos { get { return texture.UpperLeft.Y + texture.Height * 0.01f; } }
        private float offset { get { return 50 * RenderingDevice.TextureScaleFactor.Y; } }

        private Sprite texture;
        private SpriteFont font { get { return fontDelegate(); } }
        private FontDelegate fontDelegate;

        public Dock(TextureDelegate dockTex, FontDelegate font)
        {
            texture = new Sprite(dockTex, new Vector2(-dockTex().Width, 120), null, Sprite.RenderPoint.UpLeft);
            fontDelegate = font;
        }

        public void Draw()
        {
            RenderingDevice.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
            texture.Draw(new Color(255, 255, 255, 180));
            if(Input.ControlScheme == ControlScheme.Keyboard)
            {
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine1Key, new Vector2(xPos, yPos + offset), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine2Key, new Vector2(xPos, yPos + offset * 2), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine3Key, new Vector2(xPos, yPos + offset * 3), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine4Key, new Vector2(xPos, yPos + offset * 4), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine5Key, new Vector2(xPos, yPos + offset * 5), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine6Key, new Vector2(xPos, yPos + offset * 6), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine7Key, new Vector2(xPos, yPos + offset * 7), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine8Key, new Vector2(xPos, yPos + offset * 8), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine9Key, new Vector2(xPos, yPos + offset * 9), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.Machine0Key, new Vector2(xPos, yPos + offset * 10), true);
                SymbolWriter.WriteKeyboardIcon(Input.WindowsOptions.QuickBoxKey, new Vector2(xPos, yPos + offset * 11), true);
            }
            else
            {
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine1Key, new Vector2(xPos, yPos + offset), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine2Key, new Vector2(xPos, yPos + offset * 2), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine3Key, new Vector2(xPos, yPos + offset * 3), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine4Key, new Vector2(xPos, yPos + offset * 4), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine5Key, new Vector2(xPos, yPos + offset * 5), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine6Key, new Vector2(xPos, yPos + offset * 6), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine7Key, new Vector2(xPos, yPos + offset * 7), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine8Key, new Vector2(xPos, yPos + offset * 8), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine9Key, new Vector2(xPos, yPos + offset * 9), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.Machine0Key, new Vector2(xPos, yPos + offset * 10), true);
                SymbolWriter.WriteXboxIcon(Input.XboxOptions.QuickBoxKey, new Vector2(xPos, yPos + offset * 11), true);
            }
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 1", new Vector2(xPos + offset - 20, yPos - 15 + offset * 1), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 2", new Vector2(xPos + offset - 20, yPos - 15 + offset * 2), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 3", new Vector2(xPos + offset - 20, yPos - 15 + offset * 3), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 4", new Vector2(xPos + offset - 20, yPos - 15 + offset * 4), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 5", new Vector2(xPos + offset - 20, yPos - 15 + offset * 5), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 6", new Vector2(xPos + offset - 20, yPos - 15 + offset * 6), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 7", new Vector2(xPos + offset - 20, yPos - 15 + offset * 7), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 8", new Vector2(xPos + offset - 20, yPos - 15 + offset * 8), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 9", new Vector2(xPos + offset - 20, yPos - 15 + offset * 9), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Machine 10", new Vector2(xPos + offset - 20, yPos - 15 + offset * 10), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.DrawString(font, "Send Another Box", new Vector2(xPos + offset - 20, yPos - 15 + offset * 11), Color.White, 0, Vector2.Zero, RenderingDevice.TextureScaleFactor, SpriteEffects.None, 0);
            RenderingDevice.SpriteBatch.End();
        }

        public void Open()
        {
            texture.MoveTo(new Vector2(0, texture.UpperLeft.Y), 1.25f);
        }
        public void Close()
        {
            texture.MoveTo(new Vector2(-texture.Width, texture.UpperLeft.Y), 1.25f);
        }

        public void Reset()
        {
            texture.TeleportTo(new Vector2(-texture.Width / 2f, texture.Center.Y));
        }
    }
}
