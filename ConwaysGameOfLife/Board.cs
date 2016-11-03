namespace ConsoleMadness
{
    public struct Board
    {
        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            State = new bool[width,height];
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public bool[,] State { get; set; }

        public bool IsAlive(int x, int y) => State[x, y];
        public int LivingNeighbours(int x, int y)
        {
            var neighbours = 0;

            for (var row = -1; row <= 1; row++)
            {
                if (y + row < 0 || y + row >= Height)
                    continue;

                var k = (y + row + Height) % Height;

                for (var col = -1; col <= 1; col++)
                {
                    if (x + col < 0 || x + col >= Width)
                        continue;

                    var h = (x + col + Width) % Width;

                    neighbours += State[h, k] ? 1 : 0;
                }
            }

            return neighbours - (State[x, y] ? 1 : 0);
        }

        public void Kill(ushort x, ushort y)
        {
            State[x, y] = false;
        }

        public bool Cell(int x, int y)
        {
            return State[x, y];
        }
    }
}
