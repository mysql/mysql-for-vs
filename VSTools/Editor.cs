using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace MySql.VSTools
{
    /// <summary>
    /// Extension of a RichTextBox
    /// </summary>
    public class MyEditor : RichTextBox
    {
        public MyEditor()
        {
        }

        private void InitializeComponent()
        {
            // 
            // MyEditor
            // 
            this.WordWrap = false;
        }

        /// <summary>
        /// Return the index of the first character on the specified line
        /// </summary>
        /// <param name="line">Line for which you want to get the char index</param>
        /// <returns>Char index where that line can be found in the Text</returns>
        public int GetCharIndexOfLine(int line)
        {

            int lowerPosition = 0;
            int upperPosition = Text.Length;
            int lowerLine = 0;
            int upperLine = GetLineFromCharIndex(upperPosition);

            // Make sure the line is inside the bound of the text, if not, return the bound
            if (line <= lowerPosition)
                return lowerPosition;
            else if (line > upperLine)
                return upperPosition;

            int currentPosition = lowerPosition;
            int currentLine = lowerLine;
            bool increasing = true;
            int deltaLine = line;
            int deltaPosition = 0;
            int linesLeft = upperLine;

            // Look for the line
            while(currentLine != line)
            {
                deltaLine = line - currentLine;

                if (increasing)
                {
                    lowerPosition = currentPosition;
                    lowerLine = currentLine;
                    deltaPosition = upperPosition - currentPosition;
                    linesLeft = upperLine - currentLine;
                }
                else
                {
                    upperPosition = currentPosition;
                    upperLine = currentLine;
                    deltaPosition = lowerPosition - currentPosition;
                    linesLeft = currentLine - lowerLine;
                }

                // As we get closer move to a more binary like search to compensate for the fact that one line could be long and the other short
                if (linesLeft < 2)
                    linesLeft = 2;

                // Calculate offset to next position
                int delta = deltaPosition * deltaLine / linesLeft;
                if (delta == 0)
                {
                    if (increasing)
                        delta = 1;
                    else
                        delta = -1;
                }

                // Set new position
                currentPosition += delta;
                currentLine = GetLineFromCharIndex(currentPosition);
                increasing = currentLine < line;
            }

            // Now that we found the line, find the first character on that line
            while(GetLineFromCharIndex(currentPosition) == line)
            {
                --currentPosition;
            }
            
            return currentPosition + 1;
        }

    }
}
