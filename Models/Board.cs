using ChessApp.Enums;

namespace ChessApp.Models;

public class Board
{
    public Piece?[,] Squares { get; private set; } = new Piece?[8, 8];
    public Colour CurrentPlayer { get; private set; } = Colour.White;
    private Piece? _selectedPiece;
    public Piece? SelectedPiece 
    { 
        get => _selectedPiece; 
        set 
        { 
            _selectedPiece = GetPieceToSelect(value); 
            _eligibleSquares = GetEligibleSquares(_selectedPiece);
        } 
    }

    int DirectionOfPlay => CurrentPlayer == Colour.White ? -1 : 1;

    private HashSet<Coordinate> _eligibleSquares = new();
    public HashSet<Coordinate> EligibleSquaresForSelectedPiece => _eligibleSquares;

    public void Initialise()
    {
        PieceType[] backRow = { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };

        for (int column = 0; column < 8; column++)
        {
            Squares[1, column] = new Piece(Colour.Black, PieceType.Pawn);
            Squares[6, column] = new Piece(Colour.White, PieceType.Pawn);

            Squares[0, column] = new Piece(Colour.Black, backRow[column]);
            Squares[7, column] = new Piece(Colour.White, backRow[column]);
        }
    }

    public void MoveSelectedPieceTo(Coordinate position, bool castling = false)
    {
        if (SelectedPiece is not null && (EligibleSquaresForSelectedPiece.Contains(position) || castling))
        {
            Coordinate? previousPosition = GetPieceLocation(SelectedPiece);
            if (previousPosition is null) return;
            SetSquareByCoordinate(position, SelectedPiece);
            SetSquareByCoordinate(previousPosition, null);
            CheckEnPassantTarget(position);
            SelectedPiece.HasMoved = true;
            CheckCastling(position, previousPosition);
            SelectedPiece = null;
            Colour newPlayer = CurrentPlayer == Colour.White ? Colour.Black : Colour.White;
            HandleEnPassant(newPlayer);
            CurrentPlayer = newPlayer;
        }
    }

    private void Castle(int rookRow, int columnDelta)
    {
        (int currentRookColumn, int newRookColumn) = columnDelta == -2 ? (0, 3) : (7, 5);
        Coordinate rookCoordinate = new(rookRow, currentRookColumn);
        SelectedPiece = GetSquareByCoordinate(rookCoordinate);
        Coordinate newRookCoordinate = new(rookRow, newRookColumn);
        MoveSelectedPieceTo(newRookCoordinate, castling: true);
    }

    private HashSet<Coordinate> GetEligibleSquares(Piece? piece)
    {
        if (piece is null) return new();
        Coordinate? currentPosition = GetPieceLocation(piece);
        Colour pieceColour = piece.Colour;
        if (currentPosition is null) return new();
        PieceContext context = new(currentPosition, pieceColour);
        return piece.Type switch
        {
            PieceType.Pawn => GetEligibleSquaresPawn(context, piece.HasMoved),
            PieceType.Rook => GetEligibleSquaresRook(context),
            PieceType.Bishop => GetEligibleSquaresBishop(context),
            PieceType.Knight => GetEligibleSquaresKnight(context),
            PieceType.Queen => GetEligibleSquaresQueen(context),
            PieceType.King => GetEligibleSquaresKing(context, piece.HasMoved),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private HashSet<Coordinate> GetEligibleSquaresPawn(PieceContext context, bool hasMoved)
    {
        HashSet<Coordinate> validMoves = new();
        Coordinate currentPosition = context.Position;

        Coordinate oneSpace = new(currentPosition.Row + DirectionOfPlay, currentPosition.Column);
        if (GetSquareByCoordinate(oneSpace) is null) validMoves.Add(oneSpace);

        foreach (int columnDelta in new int[] { -1, 1 })
        {
            Coordinate horizontal = new(currentPosition.Row, currentPosition.Column + columnDelta);
            PieceContext horizontalContext = new(horizontal, context.FriendlyColour);
            if (!IsValidMove(horizontalContext)) continue;
            Piece? horizontalPiece = GetSquareByCoordinate(horizontal);
            if (horizontalPiece is not null && horizontalPiece.EnPassantTarget)
            {
                Coordinate enPassantMove = new(oneSpace.Row, horizontal.Column);
                validMoves.Add(enPassantMove);
            }

            Coordinate diagonal = new(oneSpace.Row, oneSpace.Column + columnDelta);
            PieceContext diagonalContext = new(diagonal, context.FriendlyColour);
            if (!IsValidMove(diagonalContext)) continue;
            if (GetSquareByCoordinate(diagonal) is not null) validMoves.Add(diagonal);
        }

        if (!validMoves.Contains(oneSpace)) return validMoves;

        if (!hasMoved)
        {
            Coordinate twoSpaces = new(oneSpace.Row + DirectionOfPlay, currentPosition.Column);
            if (GetSquareByCoordinate(twoSpaces) is null) validMoves.Add(twoSpaces);
        }
        return validMoves;
    }

    private HashSet<Coordinate> GetEligibleSquaresRook(PieceContext context)
    {
        Vector[] vectors = new Vector[] { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
        return GetValidMovesByVectors(vectors, context);
    }

    private HashSet<Coordinate> GetEligibleSquaresBishop(PieceContext context)
    {
        Vector[] vectors = new Vector[] { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        return GetValidMovesByVectors(vectors, context);
    }

    private HashSet<Coordinate> GetEligibleSquaresKnight(PieceContext context)
    {
        int[] offsets = { -2, -1, 1, 2 };

        return GetValidMovesByOffsets(offsets,
            context,
            skipDeltaWhere: (Coordinate coordinateDelta) =>
                IsDiagonalOffset(coordinateDelta));
    }

    private HashSet<Coordinate> GetEligibleSquaresQueen(PieceContext context)
    {
        HashSet<Coordinate> validRookMoves = GetEligibleSquaresRook(context);
        HashSet<Coordinate> validBishopMoves = GetEligibleSquaresBishop(context);

        HashSet<Coordinate> validMoves = new(validRookMoves);
        validMoves.UnionWith(validBishopMoves);
        return validMoves;
    }

    // TODO: Add castling logic (can't move through check)
    private HashSet<Coordinate> GetEligibleSquaresKing(PieceContext context, bool hasMoved)
    {
        HashSet<Coordinate> validMoves = new();
        int[] offsets = { -1, 0, 1 };

        validMoves = GetValidMovesByOffsets(offsets,
            context,
            skipDeltaWhere: (Coordinate coordinateDelta) =>
                IsZeroOffset(coordinateDelta));

        if (hasMoved) return validMoves;
        int rookRow = context.FriendlyColour == Colour.White ? 7 : 0;
        foreach (int rookColumn in new int[] {0, 7})
        {
            Coordinate rookCoordinate = new(rookRow, rookColumn);
            Piece? rook = GetSquareByCoordinate(rookCoordinate);
            if (rook is not null &&
                rook.Type == PieceType.Rook &&
                !rook.HasMoved)
            {
                int currentKingColumn = context.Position.Column;
                int kingColumnDirection = rookColumn > currentKingColumn ?  1 : -1;
                int columnBeforeKing = currentKingColumn + kingColumnDirection;
                PieceContext rookContext = new(rookCoordinate, context.FriendlyColour);
                Coordinate newRookCoordinate = new(rookRow, columnBeforeKing);
                if (GetEligibleSquaresRook(rookContext).Contains(newRookCoordinate))
                {
                    Coordinate newKingCoordinate = new(rookRow, columnBeforeKing + kingColumnDirection);
                    validMoves.Add(newKingCoordinate);
                }
            }
        }
        return validMoves;
    }

    private bool IsZeroOffset(Coordinate coordinateDelta)
    {
        return coordinateDelta.Row == 0 && coordinateDelta.Column == 0;
    }

    private bool IsDiagonalOffset(Coordinate coordinateDelta)
    {
        return Math.Abs(coordinateDelta.Row) == Math.Abs(coordinateDelta.Column);
    }

    private bool IsValidMove(PieceContext context)
    {
        return !(IsOutOfBounds(context.Position) ||
            IsOccupliedByFriendly(context) ||
            WouldTakeKing(context));
    }

    private bool IsOutOfBounds(Coordinate position)
    {
        return position.Row < 0 || position.Row > 7 ||
            position.Column < 0 || position.Column > 7;
    }

    private bool WouldTakeKing(PieceContext context)
    {
        return !IsOccupliedByFriendly(context) && GetSquareByCoordinate(context.Position)?.Type == PieceType.King;
    }

    private bool IsOccupliedByFriendly(PieceContext context)
    {
        return GetSquareByCoordinate(context.Position)?.Colour == context.FriendlyColour;
    }

    private HashSet<Coordinate> GetValidMovesByOffsets(int[] offsets, PieceContext context, Func<Coordinate, bool>? skipDeltaWhere = null)
    {
        HashSet<Coordinate> validMoves = new();
        foreach (int rowDelta in offsets)
        {
            foreach (int columnDelta in offsets)
            {
                Coordinate coordinateDelta = new(rowDelta, columnDelta);
                if (skipDeltaWhere is not null && skipDeltaWhere(coordinateDelta)) continue;

                Coordinate newPosition = new(
                    context.Position.Row + rowDelta,
                    context.Position.Column + columnDelta);

                PieceContext newContext = new(newPosition, context.FriendlyColour);
                if (IsValidMove(newContext)) validMoves.Add(newPosition);
            }
        }
        return validMoves;
    }

    private HashSet<Coordinate> GetValidMovesByVectors(Vector[] vectors, PieceContext context)
    {
        HashSet<Coordinate> validMoves = new();
        Coordinate currentPosition = context.Position;
        foreach (Vector vector in vectors)
        {
            Coordinate lastPosition = currentPosition;
            while (true)
            {
                Coordinate newPosition = new(lastPosition.Row + vector.RowDirection, lastPosition.Column + vector.ColumnDirection);
                if (!IsValidMove(new PieceContext(newPosition, context.FriendlyColour))) break;
                validMoves.Add(newPosition);
                lastPosition = newPosition;
                if (GetSquareByCoordinate(newPosition) is not null) break;
            }
        }
        return validMoves;
    }

    private Coordinate? GetPieceLocation(Piece piece)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (Squares[row, col] == piece) return new(row, col);
            }
        }
        return null;
    }

    private Piece? GetPieceToSelect(Piece? piece)
    {
        bool deselectPiece =
            piece is null ||
            piece == SelectedPiece;
        if (deselectPiece) return null;
        if (piece!.Colour != CurrentPlayer) return SelectedPiece;
        return piece;
    }

    private Piece? GetSquareByCoordinate(Coordinate coordinate)
    {
        try
        {
            return Squares[coordinate.Row, coordinate.Column];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    private void SetSquareByCoordinate(Coordinate coordinate, Piece? piece)
    {
        Squares[coordinate.Row, coordinate.Column] = piece;
    }

    private void CheckEnPassantTarget(Coordinate position)
    {
        if (SelectedPiece?.Type == PieceType.Pawn && !SelectedPiece.HasMoved && position.Row == 3 || position.Row == 4)
        {
            SelectedPiece?.EnPassantTarget = true;
        }
    }

    private void CheckCastling(Coordinate position, Coordinate previousPosition)
    {
        if (SelectedPiece?.Type == PieceType.King)
        {
            int columnDelta = position.Column - previousPosition.Column;
            int rookRow = CurrentPlayer == Colour.White ? 7 : 0;
            if (Math.Abs(columnDelta) == 2)
            {
                Castle(rookRow, columnDelta);
                return;
            }
        }
    }

    private void HandleEnPassant(Colour newPlayer)
    {
        foreach (Piece? piece in Squares)
        {
            if (piece?.Colour == newPlayer && piece.EnPassantTarget)
            {
                Coordinate? pieceLocation = GetPieceLocation(piece);
                if (pieceLocation is null) continue;
                Coordinate potentialEnPassant = new(pieceLocation.Row + DirectionOfPlay, pieceLocation.Column);
                Piece? potentialEnPassantPiece = GetSquareByCoordinate(potentialEnPassant);
                if (potentialEnPassantPiece is not null && potentialEnPassantPiece.Colour != newPlayer)
                {
                    Squares[pieceLocation.Row, pieceLocation.Column] = null;
                }
                piece.EnPassantTarget = false;
            }
        }
    }
}
