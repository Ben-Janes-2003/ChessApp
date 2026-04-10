using ChessApp.Models;

namespace ChessApp.Pages;

public partial class Home
{
    private Board Board { get; set; } = new Board();

    protected override void OnInitialized()
    {
        Board.Initialise();
    }
}
