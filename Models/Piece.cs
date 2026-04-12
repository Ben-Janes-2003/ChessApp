using ChessApp.Enums;

namespace ChessApp.Models;

public class Piece
{
    public PieceType Type { get; set; }
    public Colour Colour { get; set; }
    public char Symbol => GetSymbol();
    public bool HasMoved = false;

    public Piece(Colour colour, PieceType type)
    {
        Colour = colour;
        Type = type;
    }

    private char GetSymbol()
    {
        return Type switch
        {
            PieceType.Pawn => Colour == Colour.white ? '♙' : '♟',
            PieceType.Rook => Colour == Colour.white ? '♖' : '♜',
            PieceType.Bishop => Colour == Colour.white ? '♗' : '♝',
            PieceType.Knight => Colour == Colour.white ? '♘' : '♞',
            PieceType.Queen => Colour == Colour.white ? '♕' : '♛',
            PieceType.King => Colour == Colour.white ? '♔' : '♚',
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
