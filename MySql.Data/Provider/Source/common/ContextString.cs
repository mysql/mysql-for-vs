// Copyright � 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.Text;

namespace MySql.Data.Common
{
	internal class ContextString
	{
        string contextMarkers;
        bool escapeBackslash;

		// Create a private ctor so the compiler doesn't give us a default one
		public ContextString(string contextMarkers, bool escapeBackslash)
		{
            this.contextMarkers = contextMarkers;
            this.escapeBackslash = escapeBackslash;
		}

        public string ContextMarkers
        {
            get { return contextMarkers; }
            set { contextMarkers = value; }
        }

        public int IndexOf(string src, string target)
        {
            return IndexOf(src, target, 0);
        }

        public int IndexOf(string src, string target, int startIndex)
        {
            int index = src.IndexOf(target, startIndex);
            while (index != -1)
            {
                if (!IndexInQuotes(src, index, startIndex)) break;
                index = src.IndexOf(target, index + 1);
            }
            return index;
        }

        private bool IndexInQuotes(string src, int index, int startIndex)
        {
            char contextMarker = Char.MinValue;
            bool escaped = false;

            for (int i = startIndex; i < index; i++)
            {
                char c = src[i];

                int contextIndex = contextMarkers.IndexOf(c);

                // if we have found the closing marker for our open marker, then close the context
                if (contextIndex > -1 && contextMarker == contextMarkers[contextIndex] && !escaped)
                    contextMarker = Char.MinValue;

                // if we have found a context marker and we are not in a context yet, then start one
                else if (contextMarker == Char.MinValue && contextIndex > -1 && !escaped)
                    contextMarker = c;

                else if (c == '\\' && escapeBackslash)
                    escaped = !escaped;
            }
            return contextMarker != Char.MinValue || escaped;
        }

        public int IndexOf(string src, char target)
        {
            char contextMarker = Char.MinValue;
            bool escaped = false;
            int pos = 0;

            foreach (char c in src)
            {
                int contextIndex = contextMarkers.IndexOf(c);

                // if we have found the closing marker for our open marker, then close the context
                if (contextIndex > -1 && contextMarker == contextMarkers[contextIndex] && !escaped)
                    contextMarker = Char.MinValue;

                // if we have found a context marker and we are not in a context yet, then start one
                else if (contextMarker == Char.MinValue && contextIndex > -1 && !escaped)
                    contextMarker = c;

                else if (contextMarker == Char.MinValue && c == target)
                    return pos;
                else if (c == '\\' && escapeBackslash)
                    escaped = !escaped;
                pos++;
            }
            return -1;
        }

		public string[] Split(string src, string delimiters)
		{
			ArrayList parts = new ArrayList();
			StringBuilder sb = new StringBuilder();
            bool escaped = false;

			char contextMarker = Char.MinValue;

			foreach (char c in src)
			{
				if (delimiters.IndexOf(c) != -1 && !escaped)
				{
					if (contextMarker != Char.MinValue) 
						sb.Append(c);
					else
					{
						if (sb.Length > 0) 
						{
							parts.Add( sb.ToString() );
							sb.Remove( 0, sb.Length );
						}
					}
				}
                else if (c == '\\' && escapeBackslash)
                    escaped = !escaped;
				else 
				{
					int contextIndex = contextMarkers.IndexOf(c);
                    if (!escaped && contextIndex != -1)
                    {
                        // if we have found the closing marker for our open 
                        // marker, then close the context
                        if ((contextIndex % 2) == 1)
                        {
                            if (contextMarker == contextMarkers[contextIndex - 1])
                                contextMarker = Char.MinValue;
                        }
                        else
                        {
                            // if the opening and closing context markers are 
                            // the same then we will always find the opening
                            // marker.
                            if (contextMarker == contextMarkers[contextIndex + 1])
                                contextMarker = Char.MinValue;
                            else if (contextMarker == Char.MinValue)
                                contextMarker = c;
                        }
                    }

					sb.Append( c );
				}
			}
			if (sb.Length > 0)
				parts.Add( sb.ToString() );
			return (string[])parts.ToArray( typeof(string) );
		}
	}
}
