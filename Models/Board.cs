using ChessApp.Enums;

namespace ChessApp.Models;

public class Board
{
    public Piece[,] Squares { get; private set; } = new Piece[8, 8];
    public Colour CurrentPlayer { get; private set; } = Colour.white;
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

    private HashSet<Coordinate> _eligibleSquares = new();
    public HashSet<Coordinate> EligibleSquaresForSelectedPiece => _eligibleSquares;

    public void Initialise()
    {
        Squares[0, 0] = new Piece(Colour.black, PieceType.Rook);
        Squares[0, 1] = new Piece(Colour.black, PieceType.Knight);
        Squares[0, 2] = new Piece(Colour.black, PieceType.Bishop);
        Squares[0, 3] = new Piece(Colour.black, PieceType.Queen);
        Squares[0, 4] = new Piece(Colour.black, PieceType.King);
        Squares[0, 5] = new Piece(Colour.black, PieceType.Bishop);
        Squares[0, 6] = new Piece(Colour.black, PieceType.Knight);
        Squares[0, 7] = new Piece(Colour.black, PieceType.Rook);
        Squares[1, 0] = new Piece(Colour.black, PieceType.Pawn);
        Squares[1, 1] = new Piece(Colour.black, PieceType.Pawn);
        Squares[1, 2] = new Piece(Colour.black, PieceType.Pawn);
        Squares[1, 3] = new Piece(Colour.black, PieceType.Pawn);
        Squares[1, 4] = new Piece(Colour.black, PieceType.Pawn);
        Squares[1, 5] = new Piece(Colour.black, PieceType.Pawn);
        Squares[1, 6] = new Piece(Colour.black, PieceType.Pawn);
        Squares[1, 7] = new Piece(Colour.black, PieceType.Pawn);

        Squares[6, 0] = new Piece(Colour.white, PieceType.Pawn);
        Squares[6, 1] = new Piece(Colour.white, PieceType.Pawn);
        Squares[6, 2] = new Piece(Colour.white, PieceType.Pawn);
        Squares[6, 3] = new Piece(Colour.white, PieceType.Pawn);
        Squares[6, 4] = new Piece(Colour.white, PieceType.Pawn);
        Squares[6, 5] = new Piece(Colour.white, PieceType.Pawn);
        Squares[6, 6] = new Piece(Colour.white, PieceType.Pawn);
        Squares[6, 7] = new Piece(Colour.white, PieceType.Pawn);
        Squares[7, 0] = new Piece(Colour.white, PieceType.Rook);
        Squares[7, 1] = new Piece(Colour.white, PieceType.Knight);
        Squares[7, 2] = new Piece(Colour.white, PieceType.Bishop);
        Squares[7, 3] = new Piece(Colour.white, PieceType.Queen);
        Squares[7, 4] = new Piece(Colour.white, PieceType.King);
        Squares[7, 5] = new Piece(Colour.white, PieceType.Bishop);
        Squares[7, 6] = new Piece(Colour.white, PieceType.Knight);
        Squares[7, 7] = new Piece(Colour.white, PieceType.Rook);
    }

    public HashSet<Coordinate> GetEligibleSquares(Piece? piece)
    {
        if (piece is null) return new();
        Coordinate? currentPosition = GetPieceLocation(piece);
        Colour pieceColour = piece.Colour;
        if (currentPosition is null) return new();
        PieceContext context = new(currentPosition, pieceColour);
        return piece.Type switch
        {
            PieceType.Pawn => GetEligibleSquaresPawn(context),
            PieceType.Rook => GetEligibleSquaresRook(context),
            PieceType.Bishop => GetEligibleSquaresBishop(context),
            PieceType.Knight => GetEligibleSquaresKnight(context),
            PieceType.Queen => GetEligibleSquaresQueen(context),
            PieceType.King => GetEligibleSquaresKing(context),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private HashSet<Coordinate> GetEligibleSquaresPawn(PieceContext context)
    {
        return new();
    }

    private HashSet<Coordinate> GetEligibleSquaresRook(PieceContext context)
    {
        return new();
    }

    private HashSet<Coordinate> GetEligibleSquaresBishop(PieceContext context)
    {
        return new();
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

    private HashSet<Coordinate> GetEligibleSquaresKing(PieceContext context)
    {
        int[] offsets = { -1, 0, 1 };

        return GetValidMovesByOffsets(offsets,
            context,
            skipDeltaWhere: (Coordinate coordinateDelta) =>
                IsZeroOffset(coordinateDelta));
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
        return !(IsOutOfBounds(context.CurrentPosition) ||
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
        Coordinate position = context.CurrentPosition;
        return !IsOccupliedByFriendly(context) && Squares[position.Row, position.Column]?.Type == PieceType.King;
    }

    private bool IsOccupliedByFriendly(PieceContext context)
    {
        Coordinate position = context.CurrentPosition;
        return Squares[position.Row, position.Column]?.Colour == context.PieceColour;
    }

    private HashSet<Coordinate> GetValidMovesByOffsets(int[] offsets, PieceContext context, Func<Coordinate, bool>? skipDeltaWhere = null)
    {
        HashSet<Coordinate> validMoves = new();
        foreach (int deltaRowCoordinate in offsets)
        {
            foreach (int deltaColumnCoordinate in offsets)
            {
                Coordinate coordinateDelta = new(deltaRowCoordinate, deltaColumnCoordinate);
                if (skipDeltaWhere is not null && skipDeltaWhere(coordinateDelta)) continue;

                Coordinate newPosition = new(
                    context.CurrentPosition.Row + deltaRowCoordinate,
                    context.CurrentPosition.Column + deltaColumnCoordinate);

                PieceContext newContext = new(newPosition, context.PieceColour);
                if (IsValidMove(newContext)) validMoves.Add(newPosition);
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
}
