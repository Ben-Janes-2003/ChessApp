using ChessApp.Enums;

namespace ChessApp.Models;

public class Board
{
    public Piece[,] Spaces { get; set; } = new Piece[8, 8];
    public State State { get; set; } = new();

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
}
