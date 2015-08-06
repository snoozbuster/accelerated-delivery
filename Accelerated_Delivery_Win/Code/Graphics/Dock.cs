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
        private float offset { get { return 50 * Program.Game.TextureScaleFactor.Y; } }

        private Sprite texture;

        public Dock()
        {
            texture = new Sprite(ref Program.Game.Loader.Dock, new Vector2(-Program.Game.Loader.Dock.Width, 120), null, Sprite.RenderPoint.UpLeft);
        }

        public void Draw()
        {
            Program.Game.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
            texture.Draw(new Color(255, 255, 255, 180));
            if(Input.ControlScheme == ControlScheme.Keyboard)
            {
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key, new Vector2(xPos, yPos + offset), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine2Key, new Vector2(xPos, yPos + offset * 2), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine3Key, new Vector2(xPos, yPos + offset * 3), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine4Key, new Vector2(xPos, yPos + offset * 4), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine5Key, new Vector2(xPos, yPos + offset * 5), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine6Key, new Vector2(xPos, yPos + offset * 6), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine7Key, new Vector2(xPos, yPos + offset * 7), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine8Key, new Vector2(xPos, yPos + offset * 8), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine9Key, new Vector2(xPos, yPos + offset * 9), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine0Key, new Vector2(xPos, yPos + offset * 10), true);
                SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.QuickBoxKey, new Vector2(xPos, yPos + offset * 11), true);
            }
            else
            {
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key, new Vector2(xPos, yPos + offset), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine2Key, new Vector2(xPos, yPos + offset * 2), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine3Key, new Vector2(xPos, yPos + offset * 3), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine4Key, new Vector2(xPos, yPos + offset * 4), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine5Key, new Vector2(xPos, yPos + offset * 5), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine6Key, new Vector2(xPos, yPos + offset * 6), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine7Key, new Vector2(xPos, yPos + offset * 7), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine8Key, new Vector2(xPos, yPos + offset * 8), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine9Key, new Vector2(xPos, yPos + offset * 9), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine0Key, new Vector2(xPos, yPos + offset * 10), true);
                SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.QuickBoxKey, new Vector2(xPos, yPos + offset * 11), true);
            }
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 1", new Vector2(xPos + offset - 20, yPos - 15 + offset * 1), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 2", new Vector2(xPos + offset - 20, yPos - 15 + offset * 2), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 3", new Vector2(xPos + offset - 20, yPos - 15 + offset * 3), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 4", new Vector2(xPos + offset - 20, yPos - 15 + offset * 4), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 5", new Vector2(xPos + offset - 20, yPos - 15 + offset * 5), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 6", new Vector2(xPos + offset - 20, yPos - 15 + offset * 6), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 7", new Vector2(xPos + offset - 20, yPos - 15 + offset * 7), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 8", new Vector2(xPos + offset - 20, yPos - 15 + offset * 8), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 9", new Vector2(xPos + offset - 20, yPos - 15 + offset * 9), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Machine 10", new Vector2(xPos + offset - 20, yPos - 15 + offset * 10), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.DrawString(Program.Game.Loader.Font, "Send Another Box", new Vector2(xPos + offset - 20, yPos - 15 + offset * 11), Color.White, 0, Vector2.Zero, Program.Game.TextureScaleFactor, SpriteEffects.None, 0);
            Program.Game.SpriteBatch.End();
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
