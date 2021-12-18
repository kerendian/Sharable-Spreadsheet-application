using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
class SharableSpreadSheet
{
    private String[,] spreadaheet;
    private static SemaphoreSlim numOfSearching; // use to setConcurrentSearchLimit  
    private static ArrayList writerMutexArr; // to lock each row in the spreadsheet
    private static Semaphore lockReadCount;
    private static Semaphore spreadSheetLock;
    private static int read_count;
    private static int searchLimited;
    int maxReaders; // will use this for the function setConcurrentSearchLimit and in all the searching functions - the number of conccurent searching proccess curently
    private static int currnUsers;
    //private static int writerInRow;
    int rows;
    int cols;
    public SharableSpreadSheet(int nRows, int nCols)
    {
        String[,] spreadaheet = new String[nRows, nCols];
        for (int i = 0; i < nRows; i++)
        {
            for (int j = 0; j < nCols; j++)
            {
                spreadaheet[i, j] = "";
            }
        }
        rows = nRows;
        cols = nCols;
        lockReadCount = new Semaphore(1, Int32.MaxValue);
        maxReaders = 0;
        currnUsers = 0;
        writerMutexArr = new ArrayList();
        spreadSheetLock = new Semaphore(1, 1);
        for (int i = 0; i < nRows; i++)
        {
            writerMutexArr.Add(new Mutex(false));
        }
        read_count = 0;
        searchLimited = -1;
    }
    public String[,] getMat()
    {
        return this.spreadaheet;
    }
    public int getRows()
    {
        return rows;
    }
    public int getCols()
    {
        return cols;
    }
    public void setMat(String[,] matrix)
    {
        this.spreadaheet = matrix;

    }
    public String getCell(int row, int col)
    {
        // return the string at [row,col] 
        lockReadCount.WaitOne();
        String res;
        Interlocked.Increment(ref read_count);
        if (read_count == 1) /* first reader */ // must lock the access to read the var read count because can be changed in the middle of the reading
        {
            spreadSheetLock.WaitOne();
        }
        lockReadCount.Release();
        /* reading is performed */
        ((Mutex)writerMutexArr[row]).WaitOne();
        res = this.spreadaheet[row, col];
        ((Mutex)writerMutexArr[row]).ReleaseMutex();
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Decrement(ref read_count);
        if (read_count == 0) /* last reader */
        {
            spreadSheetLock.Release();
        }
        lockReadCount.Release();
        return res;
    }
    public bool setCell(int row, int col, String str)
    {
        // set the string at [row,col]
        if (row < 0 || row > this.rows || col < 0 || col > this.cols)
            return false;
        spreadSheetLock.WaitOne();
        ((Mutex)writerMutexArr[row]).WaitOne();
        /* writing is performed */
        spreadaheet[row, col] = str;
        ((Mutex)writerMutexArr[row]).ReleaseMutex();
        spreadSheetLock.Release();
        return true;
    }
    public bool searchString(String str, ref int row, ref int col)
    {
        // search the cell with string str, and return true/false accordingly.
        // stores the location in row,col.
        // return the first cell that contains the string (search from first row to the last row)
        bool flag = true;
        /* reading is performed */
        if (searchLimited != -1)
        {
            numOfSearching.Wait(); //to setConcurrentSearchLimit
        }
        Interlocked.Increment(ref maxReaders);
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Increment(ref read_count);
        if (read_count == 1) // first reader in this row 
        {
            spreadSheetLock.WaitOne();
        }
        lockReadCount.Release();
        for (int i = 0; i < this.rows; i++)
        {
            ((Mutex)writerMutexArr[i]).WaitOne();// writers are blocked from this row
            for (int j = 0; j < this.cols; j++)
            {
                if (str.Equals(spreadaheet[i, j]))
                {
                    row = i;
                    col = j;
                    flag = true;
                    break;
                }
            }
            ((Mutex)writerMutexArr[i]).ReleaseMutex();
            if (flag == true)
                break;

        }
        if (searchLimited != -1)
        {
            numOfSearching.Release();
        }
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Decrement(ref read_count);
        if (read_count == 0) // last reader in this row 
        {
            spreadSheetLock.Release();
        }
        lockReadCount.Release();
        Interlocked.Decrement(ref maxReaders);
        return flag;
    }
    public bool exchangeRows(int row1, int row2)
    {
        // exchange the content of row1 and row2
        if (row1 < 0 || row1 > this.rows || row2 < 0 || row2 > this.rows)
            return false;
        spreadSheetLock.WaitOne();
        /* writing is performed */
        String[] temp = new String[this.cols];
        for (int i = 0; i < this.cols; i++)
        {
            temp[i] = spreadaheet[row1, i];
            spreadaheet[row1, i] = spreadaheet[row2, i];
            spreadaheet[row2, i] = temp[i];
        }
        spreadSheetLock.Release();
        return true;
    }
    public bool exchangeCols(int col1, int col2)
    {
        // exchange the content of col1 and col2
        if (col1 < 0 || col1 > this.cols || col2 < 0 || col2 > this.cols)
            return false;
        /* writing is performed */
        spreadSheetLock.WaitOne();
        String[] temp = new String[this.rows];
        for (int i = 0; i < this.rows; i++)
        {
            temp[i] = spreadaheet[i, col1];
            spreadaheet[i, col1] = spreadaheet[i, col2];
            spreadaheet[i, col2] = temp[i];
        }
        spreadSheetLock.Release();
        return true;
    }
    public bool searchInRow(int row, String str, ref int col)
    {
        Interlocked.Increment(ref maxReaders);
        if (searchLimited != -1)
        {
            numOfSearching.Wait();//to setConcurrentSearchLimit
        }

        // perform search in specific row
        if (row < 0 || row > this.rows)
            return false;
        bool flag = false;
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Increment(ref read_count);
        if (read_count == 1) // first reader in this row 
        {
            spreadSheetLock.WaitOne();
        }
        lockReadCount.Release();
        /* reading is performed */
        ((Mutex)writerMutexArr[row]).WaitOne();
        for (int i = 0; i < this.cols; i++)
        {
            if (str.Equals(spreadaheet[row, i]))
            {
                col = i;
                flag = true;
            }
            if (flag == true) break;
        }
        ((Mutex)writerMutexArr[row]).ReleaseMutex();
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Decrement(ref read_count);
        if (read_count == 0) // last reader in this row 
        {
            spreadSheetLock.Release();
        }
        lockReadCount.Release();
        if (searchLimited != -1)
        {
            numOfSearching.Release();
        }
        Interlocked.Decrement(ref maxReaders);
        return flag;
    }
    public bool searchInCol(int col, String str, ref int row)
    {
        Interlocked.Increment(ref maxReaders);
        if (searchLimited != -1)
        {
            numOfSearching.Wait();//to setConcurrentSearchLimit
        }
        // perform search in specific col
        if (col < 0 || col > this.cols)
            return false;
        bool flag = false;
        /* reading is performed */
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Increment(ref read_count);
        if (read_count == 1) // first reader in this row 
        {
            spreadSheetLock.WaitOne();
        }
        lockReadCount.Release();
        for (int i = 0; i < this.rows; i++)
        {
            ((Mutex)writerMutexArr[i]).WaitOne();
            if (str.Equals(spreadaheet[i, col]))
            {
                row = i;
                flag = true;
            }
            ((Mutex)writerMutexArr[i]).ReleaseMutex();
            if (flag == true) break;
        }
        if (searchLimited != -1)
        {
            numOfSearching.Release();
        }
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Decrement(ref read_count);
        if (read_count == 0) // last reader in this row 
        {
            spreadSheetLock.Release();
        }
        lockReadCount.Release();
        Interlocked.Decrement(ref maxReaders);
        return flag;
    }
    public bool searchInRange(int col1, int col2, int row1, int row2, String str, ref int row, ref int col)
    {
        Interlocked.Increment(ref maxReaders);
        if (searchLimited != -1)
        {
            numOfSearching.Wait();//to setConcurrentSearchLimit
        }
        // perform search within spesific range: [row1:row2,col1:col2] 
        //includes col1,col2,row1,row2
        bool flag = false;
        /* reading is performed */
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Increment(ref read_count);
        if (read_count == 1) // first reader in this row 
        {
            spreadSheetLock.WaitOne();
        }
        lockReadCount.Release();
        for (int i = row1; i <= row2; i++)
        {
            ((Mutex)writerMutexArr[i]).WaitOne();
            for (int j = col1; j <= col2; j++)
            {
                if (str.Equals(spreadaheet[i, j]))
                {
                    row = i;
                    col = j;
                    flag = true;
                }
            }
            ((Mutex)writerMutexArr[i]).ReleaseMutex();
            if (flag == true) break;
        }
        if (searchLimited != -1)
        {
            numOfSearching.Release();
        }
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Decrement(ref read_count);
        if (read_count == 0) // last reader in this row 
        {
            spreadSheetLock.Release();
        }
        lockReadCount.Release();
        Interlocked.Decrement(ref maxReaders);
        return flag;
    }
    public bool addRow(int row1)
    {
        //add a row after row1
        if (row1 < 0 || row1 > this.rows)
            return false;
        spreadSheetLock.WaitOne();
        String[,] newSpreadsheet = new String[(rows + 1), cols];
        writerMutexArr.Add(new Mutex()); //add new mutex for the spesific row that added TODO: check if its a problem to add mutex to the list if another proses is reading from the list.
        int rowIndex = 0;
        for (int i = 0; i < this.rows + 1; i++)
        {
            if (i == row1 + 1)
            {
                rowIndex--;
            }
            for (int j = 0; j < this.cols; j++)
            {
                if (i == row1 + 1)
                {
                    newSpreadsheet[i, j] = "";
                    continue;
                }
                newSpreadsheet[i, j] = spreadaheet[(rowIndex), j];
            }
            rowIndex++;
        }
        this.spreadaheet = newSpreadsheet;
        Interlocked.Increment(ref this.rows);
        spreadSheetLock.Release();
        return true;
    }
    public bool addCol(int col1)
    {
        spreadSheetLock.WaitOne();

        //add a column after col1
        if (col1 < 0 || col1 > this.cols)
            return false;

        String[,] newSpreadsheet = new String[rows, cols + 1];
        int colIndex = 0;
        for (int i = 0; i < this.cols + 1; i++)
        {

            if (i == col1 + 1)
            {
                colIndex--;
            }
            for (int j = 0; j < this.rows; j++)
            {
                if (i == col1 + 1)
                {
                    newSpreadsheet[j, i] = "";
                    continue;
                }
                newSpreadsheet[j, i] = spreadaheet[j, (colIndex)];

            }
            colIndex++;
        }
        this.spreadaheet = newSpreadsheet;
        Interlocked.Increment(ref this.cols);
        spreadSheetLock.Release();

        return true;
    }
    public void getSize(ref int nRows, ref int nCols)// need a lock?
    {
        // return the size of the spreadsheet in nRows, nCols
        nRows = this.rows;
        nCols = this.cols;
    }
    public bool setConcurrentSearchLimit(int nUsers)
        {
            // this function aims to limit the number of users that can perform the search operations concurrently.
            // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
            // In this case additional search operations will wait for existing search to finish.  
            if (searchLimited == -1 && nUsers > maxReaders) // the first time we will limit the number of searchers
            {
                Interlocked.Increment(ref searchLimited);
                numOfSearching = new SemaphoreSlim(nUsers);
                currnUsers = nUsers;
            }
            else if (nUsers < maxReaders || nUsers == currnUsers) // if nUsers is LT the curr num of seaching proccess
            {
                return false;
            }
            else if (nUsers > maxReaders && nUsers > currnUsers)
            {
                numOfSearching.Release(nUsers - currnUsers); // this operation increments the number of searching proccess
                Interlocked.Increment(ref searchLimited);
                currnUsers = nUsers;
            }
            else if(nUsers > maxReaders && nUsers < currnUsers)
            {
                int diff = currnUsers - nUsers;
                for (int i = 0; i < diff; ++i)
                {
                    numOfSearching.Wait();
                }
                Interlocked.Increment(ref searchLimited);
                currnUsers = nUsers;
            }
            return true;
        }
    public bool save(String fileName)
    {
        // save the spreadsheet to a file fileName.
        // you can decide the format you save the data. There are several options.
        //*********************************************************   
        Interlocked.Increment(ref maxReaders);
        /* reading is performed */
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Increment(ref read_count);
        if (read_count == 1) // first reader in this row 
        {
            spreadSheetLock.WaitOne();
        }
        lockReadCount.Release();
        using (StreamWriter tw = new StreamWriter(fileName, false))
        {
         
                for (int i = 0; i < this.rows; i++)
                {
                    ((Mutex)writerMutexArr[i]).WaitOne();
                    for (int j = 0; j < this.cols; j++)
                    {
                        //reading from SpreadSheet is performed
                        if(j == this.cols-1)
                            tw.Write(this.spreadaheet[i, j]);
                        else
                            tw.Write(this.spreadaheet[i, j] + "\t");
                    }
                    if(i != this.rows-1)
                        tw.Write("\n");
                    ((Mutex)writerMutexArr[i]).ReleaseMutex();
                }
                tw.Close();
                tw.Dispose();
                     
        }
        lockReadCount.WaitOne(); // lock the read of the read_count var
        Interlocked.Decrement(ref read_count);
        if (read_count == 0) // last reader in this row 
        {
            spreadSheetLock.Release();
        }
        lockReadCount.Release();
        Interlocked.Decrement(ref maxReaders);
        return true;
    }
    public bool load(String fileName)
    {
        // load the spreadsheet from fileName
        // replace the data and size of the current spreadsheet with the loaded data
        spreadSheetLock.WaitOne();
        if (!File.Exists(fileName))
        {
            return false; 
        }
        using (StreamReader file = new StreamReader(fileName))
        {
            string ln;
            //find num of rows and num of cols in the given fileName
            string text = File.ReadAllText(fileName);
            string[] rows = text.Split('\n');
            string[] cols = rows[0].Split('\t');
            int colNum = cols.Length;
            int rowNum = rows.Length;

            string[,] newSpreadSheet = new string[rowNum, colNum];
			int rowIndex=0;
            while ((ln = file.ReadLine()) != null)
            {
                string[] line = ln.Split('\t');
                for (int j = 0; j < colNum; j++)
                {
                    newSpreadSheet[rowIndex, j] = line[j];
                }
                rowIndex++;

            }
            writerMutexArr.Clear();
            for (int i = 0; i < rowNum; i++)
            {
                writerMutexArr.Add(new Mutex(false));
            }
            this.spreadaheet = newSpreadSheet;
            this.rows = rowNum;
            this.cols = colNum;

            file.Close();
            file.Dispose();
        }
        spreadSheetLock.Release();
        return true;
    }

}