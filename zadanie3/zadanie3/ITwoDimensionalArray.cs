namespace zadanie3
{
    interface ITwoDimensionalArray
    {
        //private int[,] twoDimensionalArray;
        // declaring Multidimensional Indexer 
        public int this[int row, int column]
        {
            // get accessor 
            // it returns the values which 
            // read the indexes 
            //return data[index1, index2];
            get;

            // set accessor 
            // write the values in 'data' 
            // using value keyword 
            //data[index1, index2] = value;
            //set;
        }
    }
}
