using ChessApp.Enums;

namespace ChessApp.Models;

public class Board
{
    public Piece[,] Spaces { get; private set; } = new Piece[8, 8];
    public Colour CurrentPlayer { get; private set; } = Colour.white;
    private Piece? _selectedPiece;
    public Piece? SelectedPiece 
    { 
        get => _selectedPiece; 
        set 
        { 
            _selectedPiece = GetPieceToSelect(value); 
            _eligibleSpaces = GetEligibleSpaces(_selectedPiece);
        } 
    }

    private HashSet<Coordinate> _eligibleSpaces = new();
    public HashSet<Coordinate> EligibleSpacesForSelectedPiece => _eligibleSpaces;

    public void Initialise()
    {
        Spaces[0, 0] = new Piece(Colour.black, PieceType.Rook);
        Spaces[0, 1] = new Piece(Colour.black, PieceType.Knight);
        Spaces[0, 2] = new Piece(Colour.black, PieceType.Bishop);
        Spaces[0, 3] = new Piece(Colour.black, PieceType.Queen);
        Spaces[0, 4] = new Piece(Colour.black, PieceType.King);
        Spaces[0, 5] = new Piece(Colour.black, PieceType.Bishop);
        Spaces[0, 6] = new Piece(Colour.black, PieceType.Knight);
        Spaces[0, 7] = new Piece(Colour.black, PieceType.Rook);
        Spaces[1, 0] = new Piece(Colour.black, PieceType.Pawn);
        Spaces[1, 1] = new Piece(Colour.black, PieceType.Pawn);
        Spaces[1, 2] = new Piece(Colour.black, PieceType.Pawn);
        Spaces[1, 3] = new Piece(Colour.black, PieceType.Pawn);
        Spaces[1, 4] = new Piece(Colour.black, PieceType.Pawn);
        Spaces[1, 5] = new Piece(Colour.black, PieceType.Pawn);
        Spaces[1, 6] = new Piece(Colour.black, PieceType.Pawn);
        Spaces[1, 7] = new Piece(Colour.black, PieceType.Pawn);

        Spaces[6, 0] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[6, 1] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[6, 2] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[6, 3] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[6, 4] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[6, 5] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[6, 6] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[6, 7] = new Piece(Colour.white, PieceType.Pawn);
        Spaces[7, 0] = new Piece(Colour.white, PieceType.Rook);
        Spaces[7, 1] = new Piece(Colour.white, PieceType.Knight);
        Spaces[7, 2] = new Piece(Colour.white, PieceType.Bishop);
        Spaces[7, 3] = new Piece(Colour.white, PieceType.Queen);
        Spaces[7, 4] = new Piece(Colour.white, PieceType.King);
        Spaces[7, 5] = new Piece(Colour.white, PieceType.Bishop);
        Spaces[7, 6] = new Piece(Colour.white, PieceType.Knight);
        Spaces[7, 7] = new Piece(Colour.white, PieceType.Rook);
    }

    public HashSet<Coordinate> GetEligibleSpaces(Piece? piece)
    {
        if (piece is null) return new();
        Coordinate? currentPosition = GetPieceLocation(piece);
        Colour pieceColour = piece.Colour;
        if (currentPosition is null) return new();
        return piece.Type switch
        {
            PieceType.Pawn => GetEligibleSpacesPawn(currentPosition, pieceColour),
            PieceType.Rook => GetEligibleSpacesRook(currentPosition, pieceColour),
            PieceType.Bishop => GetEligibleSpacesBishop(currentPosition, pieceColour),
            PieceType.Knight => GetEligibleSpacesKnight(currentPosition, pieceColour),
            PieceType.Queen => GetEligibleSpacesQueen(currentPosition, pieceColour),
            PieceType.King => GetEligibleSpacesKing(currentPosition, pieceColour),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private HashSet<Coordinate> GetEligibleSpacesPawn(Coordinate currentPosition, Colour pieceColour)
    {
        return new();
    }

    private HashSet<Coordinate> GetEligibleSpacesRook(Coordinate currentPosition, Colour pieceColour)
    {
        return new();
    }

    private HashSet<Coordinate> GetEligibleSpacesBishop(Coordinate currentPosition, Colour pieceColour)
    {
        return new();
    }

    private HashSet<Coordinate> GetEligibleSpacesKnight(Coordinate currentPosition, Colour pieceColour)
    {
        HashSet<Coordinate> validMoves = new();
        int[] offsets = { -2, -1, 1, 2 };
        foreach (int deltaRowCoordinate in offsets)
        {
            foreach (int deltaColumnCoordinate in offsets)
            {
                if (Math.Abs(deltaRowCoordinate) == Math.Abs(deltaColumnCoordinate)) continue;

                Coordinate newPosition = new(
                    currentPosition.Row + deltaRowCoordinate,
                    currentPosition.Column + deltaColumnCoordinate);

                if (isValidMove(newPosition, pieceColour)) validMoves.Add(newPosition);
            }
        }
        return validMoves;
    }

    private HashSet<Coordinate> GetEligibleSpacesQueen(Coordinate currentPosition, Colour pieceColour)
    {
        HashSet<Coordinate> validRookMoves = GetEligibleSpacesRook(currentPosition, pieceColour);
        HashSet<Coordinate> validBishopMoves = GetEligibleSpacesBishop(currentPosition, pieceColour);

        HashSet<Coordinate> validMoves = new(validRookMoves);
        validMoves.UnionWith(validBishopMoves);
        return validMoves;
    }

    private HashSet<Coordinate> GetEligibleSpacesKing(Coordinate currentPosition, Colour pieceColour)
    {
        return new();
    }

    private bool isValidMove(Coordinate position, Colour friendlyColour)
    {
        return !(IsOutOfBounds(position) ||
            IsOccupliedByFriendly(position, friendlyColour) ||
            WouldTakeKing(position, friendlyColour));
    }

    private bool IsOutOfBounds(Coordinate position)
    {
        return position.Row < 0 || position.Row > 7 ||
            position.Column < 0 || position.Column > 7;
    }

    private bool WouldTakeKing(Coordinate position, Colour friendlyColour)
    {
        return !IsOccupliedByFriendly(position, friendlyColour) && Spaces[position.Row, position.Column]?.Type == PieceType.King;
    }

    private bool IsOccupliedByFriendly(Coordinate position, Colour friendlyColour)
    {
        return Spaces[position.Row, position.Column]?.Colour == friendlyColour;
    }

    private Coordinate? GetPieceLocation(Piece piece)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (Spaces[row, col] == piece) return new(row, col);
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
