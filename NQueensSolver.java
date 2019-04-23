package gamestuff;
//Slightly cleaned up over last version.
//Harry McCaffery 11/5/2018 NQueensSolver r2. Solves 8x8 board in approx. 35 ms.
import java.awt.Point;
import java.util.ArrayList;
import java.util.List;

public class NQueensSolver extends PuzzleSolver{
	
	public NQueensSolver(GameBoard aBoard) {
		board = aBoard;
		moves = new ArrayList<Move>();
	}
	
	@Override
	public boolean hasWon() {
		return moves.size() == board.getSize();
	}
	
	@Override
	public List<Move> nextLegalMoves(){
		List<Move> possible = new ArrayList<Move>();
		int row = currentRow() ;
		Point curPoint;
		for (int i = 0; i < board.getSize(); i++)
			if (isSafe(curPoint = new Point(row,i))) possible.add(new Move(new Queen(),null,curPoint));
		return possible;
	}
	public boolean isSafe(Point p)
	{	
		//Shouldn't ever NEED to test the y value, as one queen should only ever be placed
		// per row.
		for (Move m : moves) 
		    if (isThreatened(p, m.getNewLocation())) return false;
		return true;

	}
	public boolean isThreatened(Point p1,Point p2)
	{		
		    return (p1.x == p2.x || p1.y == p2.y 
		    		|| (Math.abs(p1.x-p2.x)==Math.abs(p1.y-p2.y)));		
	}
	public int currentRow()
	{
		return moves.size();
	}
	public boolean hasVisitedLocation(Point aLocation) {
		for(Move eachMove : moves) {
			if(eachMove.getNewLocation().equals(aLocation)) {
				return true;
			}
		}
		return false;
	}
}






