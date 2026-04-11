using ChessApp.Enums;

namespace ChessApp.Models;

public class PieceContext
{
    public Coordinate CurrentPosition { get; init; } = default!;
    public Colour PieceColour { get; init; }

    public PieceContext(Coordinate currentPosition, Colour pieceColour)
    {
        CurrentPosition = currentPosition;
        PieceColour = pieceColour;
    }
}
