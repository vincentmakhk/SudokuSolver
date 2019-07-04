# SudokuSolver

Console C# program to solve Sudoku

## Usage

Input: a 9x9 text matrix:

```
  5  2   
 2 734   
 6 8    9
 9 1287  
  7  6  2
 84  31  
 7 35  8 
   4  2  
5     61 
```

Output: a filled 9x9 text matrix:

```
415692378
928734561
763815429
396128745
157946832
284573196
672351984
831469257
549287613
```

## Built With

A Visual Studio 2015 project file with .NET framework 4.5.2

## Algorithm

Two logical deduction are used:
- only possible number in that cell, due to other numbers have been used in the same row, column, or 3x3 square
- only possible number in that cell within its 3x3 square, by considering what the unfilled cells can possibly be

And try recursive guessing once the logics exhausted

## Authors

Vincent Mak

## License

This project is licensed under the GNU General Public License v3.0 License - see the [LICENSE.txt](LICENSE.txt) file for details

## Acknowledgments

Thanks Nikoli for the invention of Sudoku

Thanks the following websites for providing puzzles:
- https://sudoku.com/ - from beginners to evil
- https://puzzling.stackexchange.com/questions/12/are-there-published-sudoku-puzzles-that-require-guessing - for samples that needs guessing
- https://www.telegraph.co.uk/news/science/science-news/9359579/Worlds-hardest-sudoku-can-you-crack-it.html - benchmark to prove a good solution
- http://www.sudokuwiki.org/Weekly_Sudoku.asp - for more difficult samples
- and other various sites
