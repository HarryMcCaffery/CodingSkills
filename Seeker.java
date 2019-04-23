package searching;

//Harry McCaffery Binary Search 10/22/2018
//Recursive implementation for extra credit
//array search is about 3x the speed of list search.  list search is around the same as list.contains().
//searches performed at 60-180 million ops/sec on my computer.
import java.util.List;

public class Seeker {
	
	
	public static int binarySearch(List<Double> l, double val)
	{
		return binarySearchRecursive(l,val,l.size()/2,1);
	}
	static int binarySearchRecursive(List<Double> l, double val, int pos, int iteration)
	{
		// this way we don't have to do the calculation 4 times
		int modifier = l.size()/(iteration*2);
	
		double high = l.get(pos+((modifier)));
		double low = l.get(pos-((modifier)));
		double mid = l.get(pos);
	
		
		if (high < val || low > val) return -1;
		if (mid == val ) return pos;
		if (high == val) return pos+(modifier);
		if (low == val ) return pos-(modifier);
		if (high == low) return -1;
		
		if (val > mid) return binarySearchRecursive(l,val,pos+(modifier/2),iteration*2);
		return  binarySearchRecursive(l,val,pos-(modifier/2),iteration*2);
	}
	
	public static int binarySearch(double[] l, double val)
	{
		return binarySearchRecursive(l,val,l.length/2,1);
	}
	static int binarySearchRecursive(double l [], double val, int pos, int iteration)
	{
		// this way we don't have to do the calculation 4 times
		int modifier = l.length/(iteration*2);
	
		double high = l[pos+((modifier))];
		double low = l[pos-((modifier))];
		double mid = l[pos];
	
		
		if (high < val || low > val) return -1;
		if (mid == val ) return pos;
		if (high == val) return pos+(modifier);
		if (low == val ) return pos-(modifier);
		if (high == low) return -1;
		
		if (val > mid) return binarySearchRecursive(l,val,pos+(modifier/2),iteration*2);
		return  binarySearchRecursive(l,val,pos-(modifier/2),iteration*2);
	}

}
