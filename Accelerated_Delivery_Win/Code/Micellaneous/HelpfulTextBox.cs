using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net;

namespace Accelerated_Delivery_Win
{
    public sealed class HelpfulTextBox
    {
        private Rectangle screenSpace;
        private Color color = Color.White;
        private readonly SpriteFont font;
        private readonly Vector2 stringPos;
        private readonly Vector2 replaceLR1, replaceLR2, replaceSorBV, replaceCam1, spacesVector;

        private const string spacesPerIcon = "      ";

        private const string printLR = "%lr%";
        private const string printS = "%s%";
        private const string printB = "%b%";
        private const string printCam = "%c%";
        private const string printNum1 = "%1%";
        private const string printTilde = "%~%";
        private const string printRes = "%r%";
        private const string printZoom = "%z%";
        private const string printHelp = "%h%";

        private const string replaceLR = spacesPerIcon + " " + spacesPerIcon;
        private const string replaceSorB = spacesPerIcon;
#if WINDOWS
        private const string replaceCam = spacesPerIcon;
#elif XBOX
        private const string replaceCam = spacesPerIcon;
#endif

        private string previousFrameText = "";
        private string[] previousFrameFormattedText;

        private float newlineHalfHeight { get { return font.MeasureString("\n").Y * Program.Game.TextureScaleFactor.Y * 0.5f; } }
        private Vector2 upperLeft { get { return new Vector2(screenSpace.X, screenSpace.Y); } }

        private Vector2 scale = Vector2.One;

        /// <summary>
        /// Creates a text box that does nice things.
        /// </summary>
        /// <param name="space">The screen space this text box has available.</param>
        public HelpfulTextBox(Rectangle space, SpriteFont font)
        {
            screenSpace = space;
            this.font = font;
            previousFrameFormattedText = new string[0];

            spacesVector = font.MeasureString(spacesPerIcon);
            replaceLR1 = new Vector2(spacesVector.X * 0.5f, spacesVector.Y * 0.5f);
            replaceLR2 = new Vector2(spacesVector.X * 1.5f + font.MeasureString(" ").X, spacesVector.Y * 0.5f);
            replaceSorBV = new Vector2(spacesVector.X * 0.5f, spacesVector.Y * 0.5f);
            replaceCam1 = new Vector2(spacesVector.X * 0.5f, spacesVector.Y * 0.5f);

            stringPos = new Vector2(screenSpace.X, screenSpace.Y);
        }

        public void SetSpace(Rectangle r)
        {
            screenSpace = r;
        }

        public void SetTextColor(Color c)
        {
            color = c;
        }

        /// <summary>
        /// Warning: This does not scale icons.
        /// </summary>
        /// <param name="s"></param>
        public void SetScaling(Vector2 s)
        {
            scale = s;
        }

        /// <summary>
        /// Draws text. 
        /// </summary>
        /// <param name="text">Expects the text to be in the same form as the HelpfulText property in MenuControl.</param>
        public void Draw(string text)
        {
            string[] formattedText = convertString(text);
            string temp = "";
            Vector2 previous;
            int numNewlines = 0;
            for(int i = 0; i < formattedText.Length; i++)
            {
                if(formattedText[i] == printS)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.Keyboard)
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.SelectionKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    else
                        SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.SelectionKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    formattedText[i] = spacesPerIcon;
                    temp += spacesPerIcon + " ";
                }
                else if(formattedText[i] == printB)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.Keyboard)
                        SymbolWriter.WriteKeyboardIcon(Keys.Escape,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    else
                        SymbolWriter.WriteXboxIcon(Buttons.Back,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    formattedText[i] = spacesPerIcon;
                    temp += spacesPerIcon + " ";
                }
                else if(formattedText[i] == printLR)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.Keyboard)
                    {
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.MenuLeftKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR1 * Program.Game.TextureScaleFactor, color.A);
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.MenuRightKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR2 * Program.Game.TextureScaleFactor, color.A);
                    }
                    else
                        SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.MenuLeftKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + (replaceLR1 + replaceLR2) * 0.5f * Program.Game.TextureScaleFactor, color.A);
                    formattedText[i] = replaceLR;
                    temp += replaceLR + " ";
                }
                else if(formattedText[i] == printCam)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.XboxController)
                    {
                        SymbolWriter.WriteXboxIcon(Buttons.LeftThumbstickLeft,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + replaceLR1 * Program.Game.TextureScaleFactor + upperLeft, color.A);
                        SymbolWriter.WriteXboxIcon(Buttons.RightThumbstickLeft,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + replaceLR2 * Program.Game.TextureScaleFactor + upperLeft, color.A);
                        formattedText[i] = replaceCam;
                        temp += replaceCam + " ";
                    }
                    else // Windows
                    {
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.CameraUpKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR1 * Program.Game.TextureScaleFactor, color.A);
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.CameraLeftKey,
                            new Vector2(previous.X - replaceLR1.X * 0.5f * Program.Game.TextureScaleFactor.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR2 * Program.Game.TextureScaleFactor, color.A);
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.CameraDownKey,
                            new Vector2(previous.X + replaceLR2.X * 1.1f * Program.Game.TextureScaleFactor.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR1 * Program.Game.TextureScaleFactor, color.A);
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.CameraRightKey,
                            new Vector2(previous.X + replaceLR2.X * 0.95f * Program.Game.TextureScaleFactor.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR2 * Program.Game.TextureScaleFactor, color.A);
                        formattedText[i] = replaceLR + replaceLR;
                        temp += replaceLR + replaceLR + " ";
                    }
                }
                else if(formattedText[i] == printNum1)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.XboxController)
                        SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.Machine1Key,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    else
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.Machine1Key,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    formattedText[i] = spacesPerIcon;
                    temp += spacesPerIcon + " ";
                }
                else if(formattedText[i] == printTilde)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.XboxController)
                        SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.QuickBoxKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    else
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.QuickBoxKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    formattedText[i] = spacesPerIcon;
                    temp += spacesPerIcon + " ";
                }
                else if(formattedText[i] == printRes)
                {
                    formattedText[i] = Program.Game.Width + "x" + Program.Game.Height;
                    temp += formattedText[i] + " ";
                }
                else if(formattedText[i] == printZoom)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.Keyboard)
                    {
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.CameraZoomPlusKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR1 * Program.Game.TextureScaleFactor, color.A);
                        SymbolWriter.WriteKeyboardIcon(Program.Game.Manager.CurrentSaveWindowsOptions.CameraZoomMinusKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR2 * Program.Game.TextureScaleFactor, color.A);
                    }
                    else
                    {
                        SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.CameraZoomPlusKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR1 * Program.Game.TextureScaleFactor, color.A);
                        SymbolWriter.WriteXboxIcon(Program.Game.Manager.CurrentSaveXboxOptions.CameraZoomMinusKey,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + upperLeft + replaceLR2 * Program.Game.TextureScaleFactor, color.A);
                    }
                    formattedText[i] = replaceLR;
                    temp += replaceLR + " ";
                }
                else if(formattedText[i] == printHelp)
                {
                    previous = font.MeasureString(temp) * Program.Game.TextureScaleFactor * scale;
                    if(Input.ControlScheme == ControlScheme.XboxController)
                        SymbolWriter.WriteXboxIcon(Buttons.Back,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    else
                        SymbolWriter.WriteKeyboardIcon(Keys.Tab,
                            new Vector2(previous.X, numNewlines * newlineHalfHeight) + spacesVector * 0.5f * Program.Game.TextureScaleFactor + upperLeft, color.A);
                    formattedText[i] = spacesPerIcon;
                    temp += spacesPerIcon + " ";
                }
                else if(formattedText[i] != "\n")
                    temp += formattedText[i] + " ";
                else
                {
                    temp = "";
                    numNewlines++;
                }
            }

            string draw = "";

            foreach(string s in formattedText)
                draw += s + (s == "\n" ? "" : " ");

            Program.Game.SpriteBatch.DrawString(font, draw, upperLeft, color * (color.A / 255f), 0, Vector2.Zero, Program.Game.TextureScaleFactor * scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Expands a given string to add spaces where needed and add newlines.
        /// </summary>
        /// <param name="text">The text from Draw().</param>
        private string[] convertString(string text)
        {
            if(previousFrameText == text)
                return (string[])previousFrameFormattedText.Clone();

            previousFrameText = text;

            List<string> words = new List<string>();
            string temp = "";

            foreach(char c in text)
            {
                if(c != ' ')
                    temp += c;
                else
                {
                    words.Add(temp);
                    temp = "";
                }
            }
            words.Add(temp); // adds the last word.

            double cumulativeX = 0;
            for(int i = 0; i < words.Count; i++)
            {
                string measure;
                if(words[i] == printB || words[i] == printS)
                    measure = replaceSorB;
                else if(words[i] == printLR)
                    measure = replaceLR;
                else
                    measure = words[i];

                Vector2 tempLength = font.MeasureString(measure + " ") * Program.Game.TextureScaleFactor * scale;
                if(cumulativeX + tempLength.X > screenSpace.Width)
                {
                    words.Insert(i++, "\n"); // increase i because we already add the X value for this word.
                    cumulativeX = tempLength.X;
                }
                else
                    cumulativeX += tempLength.X;
            }

            previousFrameFormattedText = words.ToArray();
            return words.ToArray();
        }
    }
}
