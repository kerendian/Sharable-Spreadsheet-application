# Multithreaded-Sharable-Spreadsheet-application

***Implementation of Sharable Spreadsheet***
first, we decided that 5 functions are changing the shape of the whole
Spreadsheet table and they need to lock all the table when executes.
The functions are: addRow, addCol, exchangeRows, exchangeCols, load, we call
them **writer functions**.

We used one **Mutex** called lockReadCount and, one **Semaphore** called
spreadSheetLock, to lock the whole table in these 5 functions, we used the
**reader writer problem** to do it. In this way, many reader functions like searching can be performed
simultaneously on the Spreadsheet but if one of the 5 function is performing it
will be the only one in the spreadsheet at the time until the function finishes.

Second, all the other functions that we call **reader functions** lock the row they
are operating in. We used Mutex array in size of the number of rows in the
Spreadsheet to lock each row that used in any reader function.

We are locking the row because it more efficient in memory than locking each
cell and more efficient in run time than locking all the spread sheet.

setCell is also writer function but doesn’t need to lock the whole spread sheet
so we lock the row in which it operates.

In the setConcurrentSearchLimit function we used **SemaphoreSlim** to increase
or decrease the number of users can be simultaneously in all the searching
functions at the same time.

**Types of lock We use are**: Mutex, Semaphore and SemaphoreSlim
**Number of locks we use in this program**: 1 Mutex, 1 Semaphore, 1 SemaphoreSlim, 
Mutex array in size of the number of rows in the table.
