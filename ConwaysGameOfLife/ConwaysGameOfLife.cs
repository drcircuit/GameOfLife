using System;
using System.Threading;

namespace ConsoleMadness
{
    internal class ConwaysGameOfLife
    {
        private static bool _running;
        private static Board _board = new Board(Width, Height);

        private const int Height = 40;
        private const int Width = 80;

        private static void AddCell(MouseHandler.Coord position)
        {
            _board.State[position.X, position.Y] = true;
        }

        private static void Draw(ushort x, ushort y)
        {
            Write(x, y, "o");
        }

        private static void DrawBoard()
        {
            for (ushort x = 0; x < Width; ++x)
                for (ushort y = 0; y < Height - 1; ++y)
                    if (_board.IsAlive(x, y))
                        Draw(x, y);
                    else
                        Erase(x, y);
        }

        private static void Erase(ushort x, ushort y)
        {
            Write(x, y, " ");
        }

        private static void HandleMouse()
        {
            var mouseHandler = new MouseHandler();
            var mouse = new MouseHandler.Listener();
            uint recordLen = 0;
            while (true)
            {
                if (!MouseHandler.ReadConsoleInput(mouseHandler.Handle, ref mouse, 1, ref recordLen)) throw new Exception();
                switch (mouse.Event.Button)
                {
                    case Mouse.Button.None:
                        continue;
                    case Mouse.Button.Left:
                        AddCell(mouse.Event.Position);
                        Draw(mouse.Event.Position.X, mouse.Event.Position.Y);
                        continue;
                    case Mouse.Button.Right:
                        RemoveCell(mouse.Event.Position);
                        Erase(mouse.Event.Position.X, mouse.Event.Position.Y);
                        continue;
                }
            }
        }

        private static void KeyboardListener()
        {
            while (true)
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Spacebar:
                        _running = true;
                        continue;
                    case ConsoleKey.Escape:
                        _running = false;
                        continue;
                }
        }


        private static void Main()
        {
            SetupScreen();

            var keyboard = new Thread(KeyboardListener);
            var mouse = new Thread(HandleMouse);
            var render = new Thread(Render);

            keyboard.Start();
            render.Start();
            mouse.Start();
        }

        private static void PlayConway()
        {
            var nextBoard = new bool[Width, Height];

            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    var n = _board.LivingNeighbours(x, y);
                    var c = _board.Cell(x, y);
                    nextBoard[x, y] = c && (n == 2 || n == 3) || !c && n == 3;
                }

            _board.State = nextBoard;
        }

        private static void RemoveCell(MouseHandler.Coord position)
        {
            _board.Kill(position.X, position.Y);
        }

        private static void Render()
        {
            while (true)
                if (_running)
                {
                    PlayConway();
                    DrawBoard();
                }
        }

        private static void SetupScreen()
        {
            Console.CursorVisible = false;
            Console.BufferHeight = Height;
            Console.BufferWidth = Width;
            Console.WindowHeight = Height;
            Console.WindowWidth = Width;
        }


        private static void Write(ushort x, ushort y, string symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }
    }
}