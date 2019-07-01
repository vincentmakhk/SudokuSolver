using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudokusolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();
            prog.Run();
        }

        public Program()
        {
            regionMatrix = new int[][] {
                new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 3, 3, 3, 4, 4, 4, 5, 5, 5 },
                new int[] { 3, 3, 3, 4, 4, 4, 5, 5, 5 },
                new int[] { 3, 3, 3, 4, 4, 4, 5, 5, 5 },
                new int[] { 6, 6, 6, 7, 7, 7, 8, 8, 8 },
                new int[] { 6, 6, 6, 7, 7, 7, 8, 8, 8 },
                new int[] { 6, 6, 6, 7, 7, 7, 8, 8, 8 }
            };

            mask = new int[9][];
            for (int i = 0; i < 9; ++i)
                mask[i] = new int[9];
        }

        internal void Run()
        {
            var t = Read();
            board = Convert(t);
            Solve();
            Output(board);
        }

        int[][] board;

        int[][] regionMatrix;

        int[][] mask;

        static List<string> Read()
        {
            List<string> t = new List<string>();
            string s;
            int i = 0;
            while ((s = Console.ReadLine()) != null)
            {
                t.Add(s);
                ++i;
                if (i == 9)
                    break;
            }

            return t;
        }

        static int[][] Convert(List<string> t)
        {
            int[][] u = new int[9][];
            for (int j = 0; j < 9; ++j)
                u[j] = new int[9];

            for(int i=0; i<9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                    if (t[i][j] >= '1' && t[i][j] <= '9')
                        u[i][j] = t[i][j] - '0';
            }

            return u;
        }

        static void Output(int[][] u)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                    Console.Write(u[i][j]);
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        void Solve()
        {
            int[] x = new int[10];
            int[] y = new int[10];
            int[] z = new int[10];

            Determine(x, y, z);

            while (FillKnown(x, y, z)) { }

            Output(board);

            //NeedGuess(u, x, y, z);

            //return u;
        }

        private void Determine(int[] row, int[] col, int[] sq)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] > 0)
                    {
                        Mark(row, i, board[i][j]); // row
                        Mark(col, j, board[i][j]); // column
                        Mark(sq, regionMatrix[i][j], board[i][j]); // region
                    }
                }
            }
        }

        private static void NeedGuess(int[][] u, int[] x, int[] y, int[] z)
        {
            throw new NotImplementedException();
        }

        private static int MissingOne(int r)
        {
            if (r >= 1022)
                return 0;

            for(int i=1; i<=9; ++i)
            {
                if ((r | (1 << i)) >= 1022)
                    return i;
            }

            return 0;
        }

        private void Assign(int[] row, int[] col, int[] sq, int i, int j, int _z, int k)
        {
            if (board[i][j] != 0)
                throw new NotImplementedException();

            board[i][j] = k;
            Mark(row, i, k);
            Mark(col, j, k);
            Mark(sq, _z, k);
        }

        private static void Mark(int[] arr, int i, int k)
        {
            if ((arr[i] & (1 << k)) != 0)
                throw new NotImplementedException();
            arr[i] |= 1 << k;
        }

        private void CalculateMask(int[] row, int[] col, int[] sq)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] != 0)
                        continue;

                    mask[i][j] = (row[i]) | (col[j]) | (sq[regionMatrix[i][j]]);
                }
            }
        }

        private bool FillKnown(int[] row, int[] col, int[] sq)
        {
            CalculateMask(row, col, sq);

            if (FindMissingOne(row, col, sq))
                return true;

            if (FindOnlyOneFit(row, col, sq))
                return true;

            return false;
        }

        private bool FindMissingOne(int[] row, int[] col, int[] sq)
        {
            //bool filled = false;

            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] != 0)
                        continue;

                    int k = MissingOne(mask[i][j]);
                    if (k > 0)
                    {
                        Assign(row, col, sq, i, j, regionMatrix[i][j], k);
                        Console.WriteLine($"FindMissingOne = {i} {j} {k} {board[i][j]}");
                        //Output(u);
                        return true;
                        //filled = true;
                    }
                }
            }
            //return filled;
            return false;
        }

        private bool FindOnlyOneFit(int[] x, int[] y, int[] z)
        {
            //bool filled = false;
            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] != 0)
                        continue;

                    // try each value
                    for (int trial = 1; trial <= 9; ++trial)
                    {
                        if (0 != (mask[i][j] & (1 << trial))) // value used
                            continue;

                        int d1 = 0, d2 = 0, d3 = 0;
                        for (int r = 0; r < 9; ++r)
                        {
                            if (board[i][r] == 0 && 0 == (mask[i][r] & (1 << trial)))
                                ++d1;
                            if (board[r][j] == 0 && 0 == (mask[r][j] & (1 << trial)))
                                ++d2;
                            for (int s = 0; s < 9; ++s)
                            {
                                if (regionMatrix[r][s] == regionMatrix[i][j] && board[r][s] == 0 && 0 == (mask[r][s] & (1 << trial)))
                                    ++d3;
                            }
                        }

                        if (d1 == 1 || d2 == 1 || d3 == 1) // only itself
                        {
                            Console.WriteLine($"FindOnlyOneFit = {i} {j} {trial} {board[i][j]}");
                            Assign(x, y, z, i, j, regionMatrix[i][j], trial);
                            //filled = true;
                            //break;
                            return true;
                        }
                    }
                }
            //return filled;
            return false;
        }

    }
}
