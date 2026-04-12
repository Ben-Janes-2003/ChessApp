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
            PieceType.Pawn => Colour == Colour.White ? '♙' : '♟',
            PieceType.Rook => Colour == Colour.White ? '♖' : '♜',
            PieceType.Bishop => Colour == Colour.White ? '♗' : '♝',
            PieceType.Knight => Colour == Colour.White ? '♘' : '♞',
            PieceType.Queen => Colour == Colour.White ? '♕' : '♛',
            PieceType.King => Colour == Colour.White ? '♔' : '♚',
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
