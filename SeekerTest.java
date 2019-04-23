package searching;

import static org.junit.jupiter.api.Assertions.*;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

class SeekerTest {

	double[] sortedData = {-16.0, 1.2, 1.3, 8.75, 13.6, 17.4, 87.0, 98.6, 100.0, 115.3, 115.8};
	
	@BeforeEach
	void setUp() throws Exception {
	}

	@AfterEach
	void tearDown() throws Exception {
	}

	@Test
	void testBinarySearchArray() {
		for (int i = 0; i < 100000000; i++)
		{//interestingly, performs about 3x faster than list search, due to list structure.
		
		assertTrue(Seeker.binarySearch(sortedData, 100.1) == -1);
		assertTrue(Seeker.binarySearch(sortedData, -16.0) == 0);
		assertTrue(Seeker.binarySearch(sortedData, 115.8) == 10);
		assertTrue(Seeker.binarySearch(sortedData, 17.4) == 5);
		assertTrue(Seeker.binarySearch(sortedData, -50.0) == -1);
		assertTrue(Seeker.binarySearch(sortedData, 250.0) == -1);
		}
		
			
	}

	@Test
	void testBinarySearchList() {
		List<Double> sortedList = new ArrayList<Double>();
		for(double eachDouble : sortedData) {
			sortedList.add(eachDouble);
		}
	//	for (int i = 0; i < 100000000; i++)
	//	{performs at about 60 million ops/sec
		assertTrue(Seeker.binarySearch(sortedList, 100.1) == -1);
		assertTrue(Seeker.binarySearch(sortedList, -16.0) == 0);
		assertTrue(Seeker.binarySearch(sortedList, 115.8) == 10);
		assertTrue(Seeker.binarySearch(sortedList, 17.4) == 5);
		assertTrue(Seeker.binarySearch(sortedList, -50.0) == -1);
		assertTrue(Seeker.binarySearch(sortedList, 250.0) == -1);
	
			
		//}
	}

}