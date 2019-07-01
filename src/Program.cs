using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudokuai
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = Read();
            var u = Convert(t);
            var r = Solve(u);
            Output(r);
        }


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

        static int[][] Solve(int[][] u)
        {
            int[] x = new int[10];
            int[] y = new int[10];
            int[] z = new int[10];

            Determine(u, x, y, z);

            while (FillKnown(u, x, y, z)) { }

            Output(u);

            //NeedGuess(u, x, y, z);

            return u;
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

        private static bool FillKnown(int[][] u, int[] x, int[] y, int[] z)
        {
            int[][] n = new int[][] {
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

            int[][] P = new int[9][];
            for (int i = 0; i < 9; ++i)
                P[i] = new int[9];

            bool filled = false;
            //int k;
            for(int i=0; i<9; ++i)
            {
                for(int j=0; j<9; ++j)
                {
                    if (u[i][j] != 0)
                        continue;

                    P[i][j] = (x[i]) | (y[j]) | (z[n[i][j]]);
                    int k = MissingOne(P[i][j]);
                    if (k>0)
                    {
                        Assign(u, x, y, z, i, j, n[i][j], k);
                        Output(u);
                        filled = true;
                    }
                }
                //k = MissingOne(x[i]);
                //if (k > 0)
                //{
                //    RowFill(u, i, k);
                //    filled = true;
                //}
                //k = MissingOne(y[i]);
                //if (k > 0)
                //{
                //    ColumnFill(u, i, k);
                //    filled = true;
                //}
                //k = MissingOne(z[i]);
                //if (k > 0)
                //{
                //    RegionFill(u, i, k);
                //    filled = true;
                //}
            }

            if (!filled)
            {
                for (int i = 0; i <9 && !filled; ++i)
                    for(int j=0; j< 9 && !filled; ++j)
                    {
                        if (u[i][j] > 0)
                            continue;

                        // try each value
                        for (int trial = 1; trial <= 9; ++trial)
                        {
                            if (0 != (P[i][j] & (1 << trial))) // value used
                                continue;

                            int d1 = 0, d2 = 0, d3 = 0;
                            for(int r = 0; r<9; ++r)
                            {
                                if (u[i][r] == 0 && 0 == (P[i][r] & (1 << trial)))
                                    ++d1;
                                if (u[r][j] == 0 && 0 == (P[r][j] & (1 << trial)))
                                    ++d2;
                                for(int s = 0; s<9;++s)
                                {
                                    if (n[r][s] == n[i][j] && u[r][s] == 0 && 0 == (P[r][s] & (1 << trial)))
                                        ++d3;
                                }
                            }

                            if (d1 == 1 || d2 == 1 || d3 == 1) // only itself
                            {
                                Assign(u, x, y, z, i, j, n[i][j], trial);
                                filled = true;
                                break;
                            }
                        }
                    }
            }

            return filled;
        }

        private static void Assign(int[][] u, int[] x, int[] y, int[] z, int i, int j, int _z, int k)
        {
            if(u[i][j] != 0)
                throw new NotImplementedException();

            u[i][j] = k;
            Mark(x, i, k);
            Mark(y, j, k);
            Mark(z, _z, k);
        }

        private static void Mark(int[] x, int i, int k)
        {
            if ((x[i] & (1 << k)) != 0)
                throw new NotImplementedException();
            x[i] |= 1 << k;
        }
        /*
        private static void RegionFill(int[][] u, int p, int k)
        {
            int[][] n = new int[][] {
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

            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (u[i][j] == 0 && n[i][j] == p)
                    {
                        u[i][j] = k;
                    }
                }
            }

            int[] d = new int[9];
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (n[i][j] == p)
                    {
                        d[ u[i][j]-1 ]++;
                    }
                }
            }
            bool b = d.All(x => x == 1);
            if (!b)
                throw new NotImplementedException();
        }

        private static void ColumnFill(int[][] u, int i, int k)
        {
            for (int j = 0; j < 9; ++j)
            {
                if (u[j][i] == 0)
                    u[j][i] = k;
            }

            int[] d = new int[9];
            for (int j = 0; j < 9; ++j)
            {
                ++d[u[j][i]-1];
            }
            bool b = d.All(x => x == 1);
            if (!b)
                throw new NotImplementedException();
        }

        private static void RowFill(int[][] u, int i, int k)
        {
            for(int j=0; j<9; ++j)
            {
                if (u[i][j] == 0)
                    u[i][j] = k;
            }

            int[] d = new int[9];
            for (int j = 0; j < 9; ++j)
            {
                ++d[u[i][j]-1];
            }
            bool b = d.All(x => x == 1);
            if (!b)
                throw new NotImplementedException();
        }
        */
        private static void Determine(int[][] u, int[] x, int[] y, int[] z)
        {
            //int[] m = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[][] n = new int[][] {
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

            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (u[i][j] > 0)
                    {
                        Mark(x, i, u[i][j]); // row
                        Mark(y, j, u[i][j]); // column
                        Mark(z, n[i][j], u[i][j]); // region
                    }
                }
            }
        }
    }
}
