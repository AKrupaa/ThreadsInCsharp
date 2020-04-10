using System;
using System.Collections.Generic;
using System.Threading;

namespace zadanie3
{
    class Program
    {
        public static void CallWhenChanged(int actualRowLenght, int actualColumnLenght)
        {
            Console.WriteLine("Size extended!");
            Console.WriteLine("Actual size of array: [" + actualRowLenght + ", " + actualColumnLenght + "]");
        }

        static void Main(string[] args)
        {
            // tu utworz obiekt klasy implementujacej tablicę dwuwymiarową do przechowywania liczb typu int
            // w argumencie konstruktora podaj rozmiar (czy moze byc to macierz ??)
            Array array = new Array(2, 2);

            // kolekcja wątków inne niż główny
            List<Thread> threads = new List<Thread>();

            // utworzenie wątku
            Thread mainThread;

            // Get the reference of main Thread 
            mainThread = Thread.CurrentThread;

            // Set the name of main thread 
            mainThread.Name = "Main Thread";

            // subscribe
            array.delegateWithParams += new DelegateWithParams(CallWhenChanged);

            // zapisz w zakresie
            //array[1, 1] = 11;
            array.Add(1, 1, 11);

            // odczytaj
            Console.WriteLine(array[1, 1]);

            // odczytaj spoza zakresu - zlap wyjatek
            try
            {
                Console.WriteLine(array[2, 2]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // zapisz poza zakresem
            //array[7, 19] = 719;

            array.Add(7, 20, 720);

            // wypisz dana liczbe (jeszcze kiedys poza zakresem)
            Console.WriteLine(array[7, 19]);
            Console.WriteLine(array[7, 20]);

            //brawo.

            // Uworzyć program z kilkoma wątkami, 
            // z których każdy będzie wpisywać swoje dane (np. identyfikator wątku) 
            // do tablicy przy pomocy wybranych metod

            for(int i = 0; i < 3; i++)
            {
                // utworzenie wątków
                Thread newThreadToBlockingOne, newThreadToNoBlockingOne;

                // https://stackoverflow.com/questions/3627840/meaning-of-operator-in-c-if-it-exists
                // przypisanie odpowiedniemu wątkowi delegata do metody array.Add
                newThreadToBlockingOne = new Thread(() => array.Add(i, i, i * 10 + i));
                // przypisanie nazwy do danego wątku aby w przyszłości ją odróźnić
                newThreadToBlockingOne.Name = "Thread Blocking " + i;

                // same
                newThreadToNoBlockingOne = new Thread(() => array.AddNoBlock(i, i, i * 10 + i));
                newThreadToNoBlockingOne.Name = "Thread No Blocking " + i;

                // dodanie do kolekcji
                threads.Add(newThreadToBlockingOne);
                threads.Add(newThreadToNoBlockingOne);

                // wystartowanie wątków
                newThreadToBlockingOne.Start();
                newThreadToNoBlockingOne.Start();
            }

            Thread anotherThread;
            anotherThread = new Thread(() => array.Add(21, 21, 2121));
            anotherThread.Name = "Thred No Blocking which Invoke Delegate";

            // dodanie do kolekcji
            threads.Add(anotherThread);

            anotherThread.Start();

            // Zapisać otrzymaną tablicę do pliku tekstowego

            //poczekaj na skończenie wątków
            foreach (Thread thread in threads)
            {
                if (thread != null)
                {
                    thread.Join();
                }
            }

            //Console.WriteLine("Dziala");

            // zapisz tablicę do pliku tekstowego
            array.WriteToFileTheWholeContent("tablica");
            //brawo.
        }
    }
}
