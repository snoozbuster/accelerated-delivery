using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Clipboard = System.Windows.Forms.Clipboard;
using System.Net.Mail;
using System.Net;
using System.Security;

namespace Accelerated_Delivery_Win
{
    public class CrashDebugGame : Game
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private readonly Exception exception;
        private GraphicsDeviceManager g;

        KeyboardState lastFrame;

        bool sentMail = false;
        string add = "";

        private BoxCutter Cutter;

        public CrashDebugGame(Exception ex, BoxCutter cutter)
        {
            exception = ex;
            Cutter = cutter;
            try
            {
                Cutter.WriteExceptionToLog(ex, true);
            }
            catch { }
            g = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            g.PreferredBackBufferWidth = 800;
            g.PreferredBackBufferHeight = 600;
            font = Content.Load<SpriteFont>("Font/CrashFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            g.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState thisFrame = Keyboard.GetState();
            if((thisFrame.IsKeyDown(Keys.LeftControl) || thisFrame.IsKeyDown(Keys.RightControl)) &&
                thisFrame.IsKeyDown(Keys.C) && lastFrame.IsKeyUp(Keys.C))
            {
                string text = "Type:" + exception.GetType() + "\nError:\n" + exception.Message + "\nStack:\n" + exception.StackTrace;
                Clipboard.Clear();
                System.Threading.Thread.Sleep(500);
                Clipboard.SetDataObject(text, true, 5, 200);
            }
            if(((thisFrame.IsKeyDown(Keys.LeftControl) || thisFrame.IsKeyDown(Keys.RightControl)) &&
                thisFrame.IsKeyDown(Keys.M) && lastFrame.IsKeyUp(Keys.M)) && !sentMail)
            {
                SecureString p = new SecureString();
                p.AppendChar('d'); p.AppendChar('a'); p.AppendChar('r'); p.AppendChar('k');
                p.AppendChar('4'); p.AppendChar('T'); p.AppendChar('e'); p.AppendChar('O');
                p.MakeReadOnly();
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("snoozbuster@gmail.com", p)
                };
                using(MailMessage m = new MailMessage("snoozbuster@gmail.com", "alex@twobuttoncrew.com", "Crash Report", "Error:\n" + exception.Message + "\nStack:\n" + exception.StackTrace))
                    client.Send(m);
                sentMail = true;
                add = "\n\n                                   Mail sent!";
            }
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                thisFrame.IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
            lastFrame = thisFrame;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
            string text = "We sincerely apologize, but Accelerated Delivery has\nencountered a catastrophic malfunction.\nPress CTRL+C to copy the error message to the clipboard,\nand then mail it to alex@twobuttoncrew.com.\nPlease include what you were doing, what\nlevel/menu you were on, and such.\nPress Back or Escape to exit, and press CTRL+M\nto send an email containing the error (you\nmust be connected to the internet). Thanks, and\n                               have a nice day!" + add;
            spriteBatch.DrawString(font, text, new Vector2(g.GraphicsDevice.Viewport.Width * 0.5f, g.GraphicsDevice.Viewport.Height * 0.5f), Color.White,
                0, font.MeasureString(text) * 0.5f, 1, SpriteEffects.None, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
