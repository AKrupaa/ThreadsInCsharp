﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace zadanie3
{
    //Napisać klasę implementującą tablicę dwuwymiarową do przechowywania liczb typu int.
    class Array : ITwoDimensionalArray
    {
        // --- Zapewnić bezpieczeństwo wszystkich metod i właściwości dla operacji wielowątkowych

        private static Mutex mutexNo1 = new Mutex();
        private static Mutex mutexNo2 = new Mutex();

        // Tablica posiada właściwość tylko do odczytu zwracającą jej rozmiar.
        private int rowLenght;
        // Tablica posiada właściwość tylko do odczytu zwracającą jej rozmiar.
        private int columnLenght;
        private int[,] array;
        private const int defaultCapacity = 2;

        // string showing actual size of array
        private string typeCastingSizeOfArray = "";


        // https://www.youtube.com/watch?v=TdiN18PR4zk
        // https://stackoverflow.com/questions/803242/understanding-events-and-event-handlers-in-c-sharp
        // This delegate can be used to point to methods
        // which return void and take no param.
        public delegate void Delegate();

        // This event can cause any method which conforms
        // to Delegate to be called.
        public event Delegate MyDelegate;

        //Tablica posiada zdarzenie (event), które jest wywoływane w momencie jej rozszerzenia.
        //Argumentem jest aktualny rozmiar.
        public void HandleSomethingHappened()
        {
            typeCastingSizeOfArray = "[" + rowLenght + "," + columnLenght + "]";
            Console.WriteLine("Actual size: " + typeCastingSizeOfArray);
        }

        //W momencie utworzenia podawany jest rozmiar.
        //dodałem standardowy rozmiar, a co.
        public Array(int row = defaultCapacity, int column = defaultCapacity)
        {
            rowLenght = row;
            columnLenght = column;
            array = new int[rowLenght, columnLenght];
        }

        //Tablica posiada indeksator do zapisu i odczytu.
        public int this[int row, int column]
        {
            //- Przekroczenie rozmiaru podczas odczytu powoduje rzucenie wyjątku.
            get
            {
                if (row >= rowLenght || column >= columnLenght)
                {
                    //rozmiar przekroczony -> rzuc wyjatek
                    throw new Exception("Index was outside the bounds of the array");
                }

                // rozmiar nie jest przekroczony
                return array[row, column];
            }
            //set
            //{
            //    // Przekroczenie rozmiaru podczas zapisu powoduje rozszerzenie tablicy do żądanego rozmiaru.
            //    Add(row, column, value);
            //}
        }

        // Przypisuje wartosc w zakresie tablicy lub rozszerza rozmiar tablicy i przypisuje wartosc
        public void Add(int row, int column, int value)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // zablokuj dostęp do metody na czas trwania całej metody dla tylko i wyłącznie jednego wątku
            // entire action that is here is run exclusively
            // Wait until it is safe to enter, and do not enter if the request times out.
            Console.WriteLine("{0} is requesting the mutexNo1", Thread.CurrentThread.Name);
            mutexNo1.WaitOne();

            stopwatch.Stop();
            Console.WriteLine("{0} has entered the protected area", Thread.CurrentThread.Name);
            Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);

            // jezeli rozmiar przekroczony
            // rowLenght = 10; columnLenght = 10; 
            // to można jedynie odczytac array[9, 9]
            if (row >= rowLenght || column >= columnLenght)
            {
                // zastap nieaktualne długości aktualnymi wartościami
                // row i column posiada docelowe miejsce wpisania wartosci
                int rowExtended = rowLenght;
                int columnExtended = columnLenght;
                if (row >= rowLenght)
                    rowExtended = row + 1;
                if (column >= columnLenght)
                    columnExtended = column + 1;

                // utworz tymczasowa najwieksza tablice;
                int[,] newArray = new int[rowExtended, columnExtended];

                // skopiuj wszystkie wartosci ze starej (oryginalnej, malej) do (wiekszej) wszystkie jej wartosci
                for (int cRow = 0; cRow < rowLenght; cRow++)
                {
                    for (int cColumn = 0; cColumn < columnLenght; cColumn++)
                    {
                        // skopiuj wartosci z Array (oryginal) do newArray (kopia najwieksza tablica)
                        newArray[cRow, cColumn] = array[cRow, cColumn];
                    }
                }

                // przypisz do array (oryginal) rozszerzona tablice newArray (kopie)
                array = newArray;
                // zaaktualizuj rozmiary
                rowLenght = rowExtended;
                columnLenght = columnExtended;

                // In this case we INVOKE the Delegate
                // it mean that he RISE a signal to others, that something happened
                //if (MyDelegate != null)
                //    MyDelegate();

                //krotszy zapis \/
                MyDelegate?.Invoke();

                // do odpowiedniej komorki array (rozszerzonej, teraz: oryginalnej) wpisz wartosc pobraną
                // instrukcja dla -> nie jest przekroczony :)
            }

            // jezeli rozmiar nie jest przekroczony
            // to do konkretnej komorki wpisz pobrana wartosc
            array[row, column] = value;

            // zwolnienie dostępu do tej metody dla innych wątków
            // Release the Mutex. 
            Console.WriteLine("{0} is leaving the protected area", Thread.CurrentThread.Name);
            mutexNo1.ReleaseMutex();
            Console.WriteLine("{0} has released the mutexNo1", Thread.CurrentThread.Name);
        }


        public void AddNoBlock(int row, int column, int value)
        {

            Console.WriteLine("{0} is requesting the mutexNo2", Thread.CurrentThread.Name);
            if (mutexNo2.WaitOne(1000))
            {
                Console.WriteLine("{0} has entered the protected area", Thread.CurrentThread.Name);
                // jezeli rozmiar przekroczony
                // rowLenght = 10; columnLenght = 10; 
                // to można jedynie odczytac array[9, 9]
                if (row >= rowLenght || column >= columnLenght)
                {
                    // zastap nieaktualne długości aktualnymi wartościami
                    // row i column posiada docelowe miejsce wpisania wartosci
                    int rowExtended = rowLenght;
                    int columnExtended = columnLenght;
                    if (row >= rowLenght)
                        rowExtended = row + 1;
                    if (column >= columnLenght)
                        columnExtended = column + 1;

                    // utworz tymczasowa najwieksza tablice;
                    int[,] newArray = new int[rowExtended, columnExtended];

                    // skopiuj wszystkie wartosci ze starej (oryginalnej, malej) do (wiekszej) wszystkie jej wartosci
                    for (int cRow = 0; cRow < rowLenght; cRow++)
                    {
                        for (int cColumn = 0; cColumn < columnLenght; cColumn++)
                        {
                            // skopiuj wartosci z Array (oryginal) do newArray (kopia najwieksza tablica)
                            newArray[cRow, cColumn] = array[cRow, cColumn];
                        }
                    }

                    // przypisz do array (oryginal) rozszerzona tablice newArray (kopie)
                    array = newArray;
                    // zaaktualizuj rozmiary
                    rowLenght = rowExtended;
                    columnLenght = columnExtended;

                    // In this case we INVOKE the Delegate
                    // it mean that he RISE a signal to others, that something happened
                    //if (MyDelegate != null)
                    //    MyDelegate();

                    //krotszy zapis \/
                    MyDelegate?.Invoke();

                    // do odpowiedniej komorki array (rozszerzonej, teraz: oryginalnej) wpisz wartosc pobraną
                    // instrukcja dla -> nie jest przekroczony :)
                }

                // jezeli rozmiar nie jest przekroczony
                // to do konkretnej komorki wpisz pobrana wartosc
                array[row, column] = value;

                // jezeli chcesz zobaczyc else odkomentuj ponizsza linijke!
                //Thread.Sleep(5000);
                Console.WriteLine("{0} is leaving the protected area", Thread.CurrentThread.Name);
                mutexNo2.ReleaseMutex();
                Console.WriteLine("{0} has released the mutexNo2", Thread.CurrentThread.Name);
            }
            else
            {
                Console.WriteLine("{0} will not acquire the mutexNo2", Thread.CurrentThread.Name);
            }
        }

        public void WriteToFileTheWholeContent(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path = path + "\\" + filename + ".txt";

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    //Do something
                    Console.WriteLine(ex.ToString());
                }
            }

            using (StreamWriter file =
                        new StreamWriter(path, true))
            {
                for(int row = 0; row < rowLenght; row++)
                {
                    for(int column = 0; column < columnLenght; column ++)
                    {
                        file.Write(array[row,column]+" ");
                    }
                    file.Write("\n");
                }
                
            }
            Console.WriteLine("Check your Desktop :-)");
        }
    }
}