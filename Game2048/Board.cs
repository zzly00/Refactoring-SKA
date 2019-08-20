using System;
using System.Collections.Generic;

namespace Game2048
{
    public class Board
    {
        private ulong[,] _board;
        public int NRows => _board.GetLength(0);
        public int NCols => _board.GetLength(1);
        private readonly Random _random = new Random();

        public Board(int nRows, int nCols)
        {
            _board = new ulong[nRows, nCols];
        }

        public Board(ulong[,] board)
        {
            _board = board;
        }

        public ulong GetBoardNumber(int nRows, int nCols)
        {
            return _board[nRows, nCols];
        }

        public void SetBoardNumber(int nRows, int nCols, ulong value)
        {
            _board[nRows, nCols] = value;
        }

        public Board Clone()
        {
            return new Board((ulong[,])_board.Clone());
        }

        public void PutNewValue()
        {
            var emptySlots = new List<Tuple<int, int>>();
            for (var iRow = 0; iRow < NRows; iRow++)
            {
                for (var iCol = 0; iCol < NCols; iCol++)
                {
                    if (GetBoardNumber(iRow, iCol) == 0)
                    {
                        emptySlots.Add(new Tuple<int, int>(iRow, iCol));
                    }
                }
            }

            // We should have at least 1 empty slot. Since we know the user is not dead
            // randomly pick an empty slot
            // randomly pick 2 (with 95% chance) or 4 (rest of the chance)
            var iSlot = _random.Next(0, emptySlots.Count);
            var value = _random.Next(0, 100) < 95 ? (ulong)2 : (ulong)4;
            SetBoardNumber(emptySlots[iSlot].Item1, emptySlots[iSlot].Item2, value);
        }

        public bool MoveHandler(Board board, Direction direction, out ulong score)
        {
            score = 0;
            var hasUpdated = false;

            var isAlongRow = direction == Direction.Left || direction == Direction.Right;

            // Should we process inner dimension in increasing index order?
            var isIncreasing = direction == Direction.Left || direction == Direction.Up;

            var outterCount = isAlongRow ? NRows : NCols;
            var innerCount = isAlongRow ? NCols : NRows;
            var innerStart = isIncreasing ? 0 : innerCount - 1;
            var innerEnd = isIncreasing ? innerCount - 1 : 0;

            var getValue = isAlongRow
                ? new Func<Board, int, int, ulong>((x, i, j) => x.GetBoardNumber(i, j))
                : new Func<Board, int, int, ulong>((x, i, j) => x.GetBoardNumber(j, i));

            var setValue = isAlongRow
                ? new Action<Board, int, int, ulong>((x, i, j, v) => x.SetBoardNumber(i,j,v))
                : new Action<Board, int, int, ulong>((x, i, j, v) => x.SetBoardNumber(j,i,v));

            for (var i = 0; i < outterCount; i++)
            {
                for (var j = innerStart; InnerCondition(j, innerStart, innerEnd); j = DropHandler(false, isIncreasing, j))
                {
                    if (getValue(board, i, j) == 0)
                    {
                        continue;
                    }

                    var newJ = j;
                    do
                    {
                        newJ = DropHandler(true, isIncreasing, newJ);
                    }
                    // Continue probing along as long as we haven't hit the boundary and the new position isn't occupied
                    while (InnerCondition(newJ, innerStart, innerEnd) && getValue(board, i, newJ) == 0);

                    if (InnerCondition(newJ, innerStart, innerEnd) && getValue(board, i, newJ) == getValue(board, i, j))
                    {
                        // We did not hit the canvas boundary (we hit a node) AND no previous merge occurred AND the nodes' values are the same
                        // Let's merge
                        var newValue = getValue(board, i, newJ) * 2;
                        setValue(board, i, newJ, newValue);
                        setValue(board, i, j, 0);

                        hasUpdated = true;
                        score += newValue;
                    }
                    else
                    {
                        // Reached the boundary OR...
                        // we hit a node with different value OR...
                        // we hit a node with same value BUT a prevous merge had occurred
                        //
                        // Simply stack along
                        newJ = DropHandler(false, isIncreasing, newJ);
                        if (newJ != j)
                        {
                            hasUpdated = true;
                        }

                        var value = getValue(board, i, j);
                        setValue(board, i, j, 0);
                        setValue(board, i, newJ, value);
                    }
                }
            }

            return hasUpdated;
        }

        private bool InnerCondition(int index, int innerStart, int innerEnd)
        {
            return Math.Min(innerStart, innerEnd) <= index && index <= Math.Max(innerStart, innerEnd);
        } 

        private static int DropHandler(bool isDrop, bool isIncreasing, int innerIndex)
        {
            if ((isDrop && isIncreasing) || (!isDrop && !isIncreasing))
            {
                return innerIndex - 1;
            }

            return innerIndex + 1;
        }
    }
}