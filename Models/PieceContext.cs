using ChessApp.Enums;

namespace ChessApp.Models;

public record PieceContext(Coordinate CurrentPosition, Colour PieceColour);