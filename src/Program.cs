using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            mask = Construct9x9Matrix();

            depth = 0;
        }

        internal void Run()
        {
            var t = Read();
            board = Convert(t);
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            Solve();
            //sw.Stop();
            //Console.WriteLine($"Time = {sw.Elapsed.ToString()}");
            Output(board);
        }

        #region members
        int[][] board;

        int[][] regionMatrix;

        int[][] mask;

        int depth;
        #endregion

        static int[][] Construct9x9Matrix()
        {
            int[][] mat = new int[9][];
            for (int i = 0; i < 9; ++i)
                mat[i] = new int[9];
            return mat;
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
            int[][] u = Construct9x9Matrix();

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
            int[] row = new int[10];
            int[] col = new int[10];
            int[] sq = new int[10];

            Determine(row, col, sq);

            while (FillKnown(row, col, sq)) { }

            //Output(board);

            //NeedGuess();

            //Output(board);
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

        private static int[][][] Construct9x9xNMatrix()
        {
            int[][][] u = new int[9][][];
            for (int i = 0; i < 9; ++i)
            {
                u[i] = new int[9][];
                for (int j = 0; j < 9; ++j)
                    u[i][j] = new int[10];
            }
            return u;
        }

        #region helper data calculations
        // Mask = what numbers are already used for that particular cell
        private void CalculateMask(int[] row, int[] col, int[] sq)
        {
            // Basic: from row, col, sq
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] != 0)
                        continue;

                    mask[i][j] = (row[i]) | (col[j]) | (sq[regionMatrix[i][j]]);
                }
            }

            // Enhance mask: apply row, col to sq

            // sqRowCount [ region number ] [ row number ] [ trial number ] : possible to put this number in this row of this region
            // sqColCount [ region number ] [ col number ] [ trial number ] : possible to put this number in this col of this region
            int[][][] sqRowCount = Construct9x9xNMatrix();
            int[][][] sqColCount = Construct9x9xNMatrix();

            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                {
                    int regionNumber = regionMatrix[i][j];

                    if (board[i][j] == 0)
                    {
                        for (int trial = 1; trial <= 9; ++trial)
                        {
                            if (0 == (mask[i][j] & (1 << trial)))
                            {
                                ++sqRowCount[regionNumber][i][trial];
                                ++sqColCount[regionNumber][j][trial];
                            }
                        }
                    }
                }

            int[][] sqToRow = new int[][] { new int[] { 0, 1, 2 },
                new int[] { 0, 1, 2 },
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 3, 4, 5 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 6, 7, 8 },
                new int[] { 6, 7, 8 }
            };


            int[][] sqToCol = new int[][] { new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 }
            };

            for (int r = 0; r < 9; ++r) // loop each region
            {
                var rows = sqToRow[r];
                var cols = sqToCol[r];
                int[] rowTrial = new int[10]; // only this row of this region can put in this number
                int[] colTrial = new int[10]; // only this col of this region can put in this number
                rowTrial[0] = -1;
                colTrial[0] = -1;
                for (int trial = 1; trial <= 9; ++trial)
                {                    
                    rowTrial[trial] = IsOnlyOneStraightFitsInThisRegion(sqRowCount[r], rows, trial);
                    colTrial[trial] = IsOnlyOneStraightFitsInThisRegion(sqColCount[r], cols, trial);
                    //HandleOneRowFitsInThisRegion(r, sqRowCount[r], rows, trial);
                    //HandleOneColFitsInThisRegion(r, sqColCount[r], cols, trial);
                }

                // loop row/col index
                for(int index = 0; index < 3; ++index)
                {
                    CheckTrials(r, rowTrial, index, rows, sqColCount[r], cols, ((i, j) => i), ((i, j) => j));
                    CheckTrials(r, colTrial, index, cols, sqRowCount[r], rows, ((i, j) => j), ((i, j) => i));

                }
            }

        }

        private void CheckTrials(int regionNumber, int[] rowTrial, int index, int[] rows, int[][] colCount, int[] cols, Func<int, int, int> getRow, Func<int, int, int> getCol)
        {
            var trials = CollectTrials(rowTrial, index);
            // if only three numbers can put in this row
            if (trials.Count == 3)
            {
                // for rows[index], mask other than these numbers
                // for others rows, mask these 3 numbers
                MaskRowInRegionForTheseNumbers(regionNumber, ((i,j)=>getRow(i,j)==rows[index]), trials);
            }
            else if (trials.Count() == 2)
            {
                // test the other col/row if only two can fit in
                int indexCol = IsTwoStraightsFitsInThisRegion(colCount, cols, trials);

                // if yes, mask all other cells than those two in this region for all 2 numbers
                if (indexCol != -1)
                {
                    MaskRowInRegionForTheseNumbers(regionNumber, ((i,j)=> getRow(i,j) == rows[index] && getCol(i,j)!=indexCol), trials);
                }
            }
        }

        private static List<int> CollectTrials(int[] arr, int index)
        {
            List<int> trials = new List<int>();
            for (int trial = 1; trial <= 9; ++trial)
            {
                if (arr[trial] == index)
                    trials.Add(trial);
            }
            return trials;
        }

        private int ConstructMask(List<int> trials)
        {
            int n = 0;
            foreach (var trial in trials)
                n |= (1 << trial);
            return n;
        }

        private void MaskRowInRegionForTheseNumbers(int regionNumber, Func<int, int, bool> func, List<int> trails)
        {
            Console.WriteLine($"MaskRowInRegionForTheseNumber: {regionNumber} {string.Join(",", trails.Select(x=>x.ToString()))}");

            int masks = ConstructMask(trails);
            int negateMasks = 1022 ^ masks;

            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (regionMatrix[i][j] != regionNumber)
                        continue;

                    if (board[i][j] != 0)
                        continue;

                    if (func(i, j)) // in same row or col
                    {
                        //if (0 != (mask[i][j] & masks))
                        //    throw new NotImplementedException();
                        //if (negateMasks != (mask[i][j] | negateMasks))
                        //    throw new NotImplementedException();
                        mask[i][j] |= negateMasks;
                    }
                    else
                    {
                        mask[i][j] |= masks;
                    }

                }
            }
        }

        private int IsOnlyOneStraightFitsInThisRegion(int[][] rowTrialCount, int[] rowsInRegion, int trial)
        {
            int n1 = rowTrialCount[rowsInRegion[0]][trial];
            int n2 = rowTrialCount[rowsInRegion[1]][trial];
            int n3 = rowTrialCount[rowsInRegion[2]][trial];
            if (n1 != 0 && n2 == 0 && n3 == 0)
                return 0;
            if (n1 == 0 && n2 != 0 && n3 == 0)
                return 1;
            if (n1 == 0 && n2 == 0 && n3 != 0)
                return 2;

            return -1;
        }

        private int IsTwoStraightsFitsInThisRegion(int[][] colTrialCount, int[] colsInRegion, List<int> trials)
        {
            for(int index = 0; index < 3; ++index)
            {
                if( trials.Sum(x=> colTrialCount[colsInRegion[index]][x]) == 0 )
                    return index;
            }

            return -1;
        }
        #endregion

        #region basic filling
        private bool FillKnown(int[] row, int[] col, int[] sq)
        {
            //Output(board);

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
        #endregion

        #region guessing
        private bool IsSolved()
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] == 0)
                        return false;
                }
            }

            return true;
        }

        private int[][] Guess(int pos_i, int pos_j, int depth, ref int count, ref int maxdepth)
        {
            for(int trial = 1; trial <= 9; ++trial)
            {
                if (0 == (board[pos_i][pos_j] & (1 << trial)))
                    continue;

                ++count;
                if (depth > maxdepth)
                    maxdepth = depth;

                var p = new Program();
                for (int i = 0; i < 9; ++i)
                    for (int j = 0; j < 9; ++j)
                        p.board[i][j] = board[i][j];

                p.board[pos_i][pos_j] = trial;
                p.depth = depth + 1;
                p.Solve();
                if (p.IsSolved())
                    return p.board;
            }

            return null;
        }

        private void NeedGuess()
        {
            int count = 0;
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] == 0)
                        ++count;
                }
            }

            if (count == 0)
                return;

            // find count/2
            int target = (count / 2);

            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (board[i][j] == 0)
                    {
                        if (--target <= 0)
                        {
                            int tries = 0, maxdepth = 0;
                            var board = Guess(i, j, depth, ref tries, ref maxdepth);
                            if (depth == 0)
                                Console.WriteLine($"tries = {tries}, maxdepth = {maxdepth}");
                            return;
                        }
                    }
                }
            }

        }
        #endregion


    }
}
