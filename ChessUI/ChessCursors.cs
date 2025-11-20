using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ChessUI
{
    public static class ChessCursors
    {
        public static readonly Cursor WhitePawnCursor = LoadCursor("Assets/CursorW.cur");
        public static readonly Cursor BlackPawnCursor = LoadCursor("Assets/CursorB.cur");

        public static Cursor LoadCursor(string filePath)
        {
            Stream stream= Application.GetResourceStream(new Uri(filePath,UriKind.Relative)).Stream;
            return new Cursor(stream, true);
        }
    }
}
