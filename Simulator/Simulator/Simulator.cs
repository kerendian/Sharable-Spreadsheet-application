using System;
using System.Threading;

class Simulator
{
    static void Main(string[] args)
    {
        int rows = Int32.Parse(args[0]);
        int cols = Int32.Parse(args[1]);
        int nThreads = Int32.Parse(args[2]);
        int nOperations = Int32.Parse(args[3]);
        String[,] matrix = new String[rows, cols];
        SharableSpreadSheet spreadSheet = new SharableSpreadSheet(rows, cols);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = "testcell" + i + j;
            }
        }
        spreadSheet.setMat(matrix);

        ThreadPool.SetMinThreads(nThreads, nThreads);
        ThreadPool.SetMaxThreads(nThreads, nThreads);

        Object[] arguments = new object[3];
        arguments[0] = spreadSheet;
        arguments[1] = nOperations;
        arguments[2] = nThreads;


        for (int i = 0; i < nThreads; i++)
        {
            ThreadPool.QueueUserWorkItem(operate, arguments);
            Thread.Sleep(2000);
        }
        //saving the updated sheet
        spreadSheet.save("spreadSheet.dat");

    }


    public static void operate(Object arguments)
    {

        Thread thread = Thread.CurrentThread;
        int threadID = thread.ManagedThreadId;
        Object[] argumentsArr = (Object[])arguments;
        SharableSpreadSheet sheet = (SharableSpreadSheet)(argumentsArr[0]);
        int numOperations = (int)(argumentsArr[1]);
        int nUsers = (int)(argumentsArr[2]);

        int rows = 0, cols = 0;
        sheet.getSize(ref rows, ref cols);
        var rand = new Random();
        var randCol = new Random();
        var randRaw = new Random();
        var randX = new Random();
        var randY = new Random();



        for (int i = 0; i < numOperations; i++)
        {
            int opNumber = rand.Next(0, 13);
            //Console.WriteLine(opNumber);
            switch (opNumber)
            {
                case 0: //getCell

                    int row0 = randRaw.Next(0, rows);
                    int col0 = randRaw.Next(0, cols);
                    String strResult = sheet.getCell(row0, col0);
                    Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: the string we found in the given cell: " + strResult);
                    break;

                case 1: //setCell
                    int row1 = randRaw.Next(0, rows);
                    int col1 = randRaw.Next(0, cols);
                    int x1 = randX.Next(0, rows);
                    int y1 = randY.Next(0, cols);
                    String str1 = "testcell" + x1 + y1;
                    bool res1 = sheet.setCell(row1, col1, str1);

                    if (res1 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: cell " + '[' + (row1 + 1) + ',' + (col1 + 1) + "]" + "was set with: " + '"' + str1 + '"');
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: cell " + '[' + (row1 + 1) + ',' + (col1 + 1) + "]" + "was NOT set with: " + '"' + str1 + '"');

                    break;

                case 2: //searchString
                    int row2 = 0;
                    int col2 = 0;
                    int x2 = randX.Next(0, rows);
                    int y2 = randY.Next(0, cols);
                    String str2 = "testcell" + (x2 + 1) + (y2 + 1);
                    bool res2 = sheet.searchString(str2, ref row2, ref col2);

                    if (res2 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string " + '"' + str2 + '"' + " found in cell [" + (row2 + 1) + "," + (col2 + 1) + "].");
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string " + '"' + str2 + '"' + " NOT found in cell [" + (row2 + 1) + "," + (col2 + 1) + "].");

                    break;

                case 3: //exchangeRows
                    int fRow3 = randRaw.Next(0, rows / 2);
                    int sRow3 = randRaw.Next(rows / 2, rows);
                    bool res3 = sheet.exchangeRows(fRow3, sRow3);
                    if (res3 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: rows [" + (fRow3 + 1) + "]" + " and [" + (sRow3 + 1) + "] exchanged successfully.");
                    else
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: rows[" + (fRow3 + 1) + "]" + " and[" + (sRow3 + 1) + "] exchanged Un successfully.");

                    break;

                case 4: //exchangeCols
                    int fCol4 = randRaw.Next(0, cols / 2);
                    int sCol4 = randRaw.Next(cols / 2, cols);
                    bool res4 = sheet.exchangeCols(fCol4, sCol4);

                    if (res4 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: cols [" + (fCol4 + 1) + "]" + " and [" + (sCol4 + 1) + "] exchanged successfully.");
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: cols [" + (fCol4 + 1) + "]" + " and [" + (sCol4 + 1) + "] exchanged UNsuccessfully.");

                    break;

                case 5: //searchInRow
                    int row5 = randRaw.Next(0, rows); ;
                    int col5 = 0;
                    int x5 = randX.Next(0, rows);
                    int y5 = randY.Next(0, cols);
                    String str5 = "testcell" + x5 + y5;
                    bool res5 = sheet.searchInRow(row5, str5, ref col5);

                    if (res5 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string '" + str5 + "' was found in column " + (col5 + 1) + ".");
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string '" + str5 + "' was NOT found in column " + (col5 + 1) + ".");

                    break;

                case 6: //searchInCol
                    int row6 = 0;
                    int col6 = randRaw.Next(0, cols);
                    int x6 = randX.Next(0, rows);
                    int y6 = randY.Next(0, cols);
                    String str6 = "testcell" + x6 + y6;
                    bool res6 = sheet.searchInCol(col6, str6, ref row6);

                    if (res6 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string '" + str6 + "' was found in column " + (row6 + 1) + ".");
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string '" + str6 + "' was NOT found in column " + (row6 + 1) + ".");

                    break;

                case 7: //searchInRange
                    int row7 = 0;
                    int col7 = 0;
                    int fromCol = randCol.Next(0, cols / 2);
                    int toCol = randCol.Next(cols / 2, cols);
                    int fromRow = randRaw.Next(0, rows / 2);
                    int toRow = randRaw.Next(rows / 2, rows);
                    int x7 = randX.Next(0, rows);
                    int y7 = randY.Next(0, cols);
                    String str7 = "testcell" + x7 + y7;
                    bool res7 = sheet.searchInRange(fromCol, toCol, fromRow, toRow, str7, ref row7, ref col7); //same here - random input

                    if (res7 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string '" + str7 + "' was found in the given range at [" + (row7 + 1) + "," + (col7 + 1) + "].");
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: string '" + str7 + "' was NOT found in the given range at [" + (row7 + 1) + "," + (col7 + 1) + "].");

                    break;

                case 8: //addRow
                    int row8 = randRaw.Next(0, rows);
                    bool res8 = sheet.addRow(row8);
                    if (res8 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: a new row was added after row " + (row8 + 1));
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: a new row was NOT added after row " + (row8 + 1));

                    //Thread.Sleep(3000);
                    break;

                case 9: //addCol
                    int col9 = randCol.Next(0, cols);
                    bool res9 = sheet.addCol(col9);
                    if (res9 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: a new column was added after column " + (col9 + 1));
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: a new column was NOT added after column " + (col9 + 1));

                    break;

                case 10: //getSize
                    int row10 = 0;
                    int col10 = 0;
                    sheet.getSize(ref row10, ref col10);
                    Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: the size of the sheet is:  " + row10 + "," + col10 + ".");
                    break;

                case 11: //setConcurrentSearchLimit
                    int randomNumber = randCol.Next(1, nUsers);
                    bool res11 = sheet.setConcurrentSearchLimit(randomNumber); //random number
                    if (res11 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: set  current limit to: " + randomNumber);
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: couldn't set the limit to: " + randomNumber);
                    break;

                case 12: //save
                    bool res12 = sheet.save("SpreadSheet.dat");

                    if (res12 == true)
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]: saving and exiting.....");
                    else
                        Console.WriteLine("User [" + Thread.CurrentThread.ManagedThreadId + "]:  NOT saving and exiting.....");
                    break;
            }
            Thread.Sleep(100);

        }


    }
}

