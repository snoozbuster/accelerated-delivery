using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;

namespace Accelerated_Delivery_Win
{
    static class SimpleMessageBox
    {
#if XBOX
        private static int? dialogResult = null;
        public static bool Showing { get; set; }
#endif

        public static int? ShowMessageBox(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon)
        {
#if XBOX
            // don't do anything if the guide is visible - one issue this handles is showing dialogs in quick      
            // succession, we have to wait for the guide to go away before the next dialog can display      
            if(Guide.IsVisible) return null;
            // if we have a result then we're all done and we want to return it      
            if(dialogResult != null)
            {
                // preserve the result        
                int? saveResult = dialogResult;
                // reset everything for the next message box        
                dialogResult = null;
                Showing = false;
                // return the result        
                return saveResult;
            }
            // return nothing if the message box is still being displayed      
            if(Showing) return null;
            // otherwise show it      
            Showing = true;
            if(Program.Game.MessagePad != null)
                Guide.BeginShowMessageBox(title, text, buttons, focusButton, icon, MessageBoxEnd, null);
            else 
                throw new NullReferenceException("The Guide was pulled without a target!");

            return null;
#endif
            return -1;
        }
#if XBOX
        private static void MessageBoxEnd(IAsyncResult result)
        {
            dialogResult = Guide.EndShowMessageBox(result);
            // if no button was pressed then we want the result to be -1      
            if(dialogResult == null)
                dialogResult = -1;
        }
#endif
    }
}
