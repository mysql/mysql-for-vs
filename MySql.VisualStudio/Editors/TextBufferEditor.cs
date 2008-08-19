using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using System.Runtime.InteropServices;

namespace MySql.Data.VisualStudio.Editors
{
    internal class TextBufferEditor 
    {
        private bool noContent;

        public TextBufferEditor()
        {
            noContent = true;
            CreateCodeEditor();
        }

        public IVsCodeWindow CodeWindow { get; private set; }
        public IVsTextBuffer TextBuffer { get; private set; }

        private void CreateCodeEditor()
        {
            Guid clsidTextBuffer = typeof(VsTextBufferClass).GUID;
            Guid iidTextBuffer = VSConstants.IID_IUnknown;
            TextBuffer = (IVsTextBuffer)MySqlDataProviderPackage.Instance.CreateInstance(
                                 ref clsidTextBuffer,
                                 ref iidTextBuffer,
                                 typeof(object));

            // first we need to site our buffer
            IObjectWithSite ows = (IObjectWithSite)TextBuffer;
            ows.SetSite(MySqlDataProviderPackage.Instance);

            // then we need to tell our buffer not to attempt to autodetect the
            // language settings
            IVsUserData userData = TextBuffer as IVsUserData;
            Guid g = EditorFactory.GuidVSBufferDetectLangSid;
            int result = userData.SetData(ref g, false);

            Guid clsidCodeWindow = typeof(VsCodeWindowClass).GUID;
            Guid iidCodeWindow = typeof(IVsCodeWindow).GUID;
            IVsCodeWindow pCodeWindow = (IVsCodeWindow)MySqlDataProviderPackage.Instance.CreateInstance(
                   ref clsidCodeWindow,
                   ref iidCodeWindow,
                   typeof(IVsCodeWindow));
            if (pCodeWindow == null)
                throw new Exception("Failed to create core editor");

            // Give the text buffer to the code window.                    
            // We are giving up ownership of the text buffer!                    
            pCodeWindow.SetBuffer((IVsTextLines)TextBuffer);

            CodeWindow = pCodeWindow;
        }

        public bool Dirty
        {
            get
            {
                uint flags;
                TextBuffer.GetStateFlags(out flags);
                return (flags & (uint)BUFFERSTATEFLAGS.BSF_MODIFIED) != 0;
            }
            set
            {
                uint flags;
                TextBuffer.GetStateFlags(out flags);
                if (value)
                    flags |= (uint)BUFFERSTATEFLAGS.BSF_MODIFIED;
                else
                    flags &= ~(uint)BUFFERSTATEFLAGS.BSF_MODIFIED;
                TextBuffer.SetStateFlags(flags);
            }
        }

        public string Text
        {
            get
            {
                IVsTextLines lines = (IVsTextLines)TextBuffer;
                int lineCount, lineLength;
                string text;
                lines.GetLineCount(out lineCount);
                lines.GetLengthOfLine(lineCount - 1, out lineLength);
                lines.GetLineText(0, 0, lineCount - 1, lineLength, out text);
                return text;
            }
            set 
            {
                if (noContent)
                    Initialize(value);
                else
                    Replace(value);
                noContent = false;
            }
        }

        private void Initialize(string value)
        {
            IVsTextLines lines = (IVsTextLines)TextBuffer;
            lines.InitializeContent(value, value.Length);
        }

        private void Replace(string value)
        {
            int endLine, endCol;
            IVsTextLines lines = (IVsTextLines)TextBuffer;
            lines.GetLastLineIndex(out endLine, out endCol);

            IntPtr pText = Marshal.StringToCoTaskMemAuto(value);

            try
            {
                lines.ReplaceLines(0, 0, endLine, endCol, pText, value.Length, null);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pText);
            }
        }
    }
}
