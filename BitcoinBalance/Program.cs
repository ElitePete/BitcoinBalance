﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Data.SQLite;

public class bitcoinHolder
{
    public string accountNumber;
    public int pin;
    public string firstName;
    public string lastName;
    public double balance;

    public bitcoinHolder(string accountNumber, string firstName, string lastName, int pin, double balance)
    {
        this.accountNumber = accountNumber;
        this.pin = pin;
        this.firstName = firstName;
        this.lastName = lastName;
        this.balance = balance;
    }

    public static List<bitcoinHolder> GetBitcoinHoldersFromDatabase()
    {
        List<bitcoinHolder> bitcoinHolders = new List<bitcoinHolder>();

        string connectionString = @"Data Source=C:\Users\subpa\Documents\PracticeDBs\cryptodb\bitcoinbalancedb.db;";
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM bitcoinHolders";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string accountNumber = reader.GetString(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);
                        int pin = reader.GetInt32(3);
                        double balance = reader.GetDouble(4);

                        bitcoinHolder holder = new bitcoinHolder(accountNumber, firstName, lastName, pin, balance);
                        bitcoinHolders.Add(holder);
                    }
                }
            }
        }

        return bitcoinHolders;
    }


    //hide pin
    public static int ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo info = Console.ReadKey(true);

        while (info.Key != ConsoleKey.Enter)
        {
            if (info.Key != ConsoleKey.Backspace)
            {
                Console.Write("*");
                password += info.KeyChar;
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    password = password.Substring(0, password.Length - 1);
                    int pos = Console.CursorLeft;
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                }
            }
            info = Console.ReadKey(true);
        }
        Console.WriteLine();

        int pin = 0;
        int.TryParse(password, out pin);

        return pin;
    }






    public String GetNum()
    {
        return accountNumber;
    }

    public int GetPin()
    {
        return pin;
    }


    public String GetFirstName()
    {
        return firstName;
    }

    public String GetLastName()
    {
        return lastName;
    }
    public double GetBalance()
    {
        return balance;
    }

    public void SetNum(String newAccNum)
    {
        accountNumber = newAccNum;
    }

    public void SetPin(int newPin)
    {
        pin = newPin;
    }

    public void SetFirstName(String newFirstName)
    {
        firstName = newFirstName;
    }

    public void SetLastName(String newLastName)
    {
        lastName = newLastName;
    }

    public void SetBalance(double newBalance)
    {
        balance = newBalance;
    }

    public static void UpdateBalanceInDatabase(bitcoinHolder holder)
    {
        string connectionString = @"Data Source=C:\Users\subpa\Documents\PracticeDBs\cryptodb\bitcoinbalancedb.db;"; 
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "UPDATE bitcoinHolders SET balance = @balance WHERE accountNumber = @accountNumber";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@balance", holder.GetBalance());
                command.Parameters.AddWithValue("@accountNumber", holder.GetNum());
                command.ExecuteNonQuery();
            }
        }
    }



    public static void Main(String[] args)
    {
        void printOptions()
        {
            Console.WriteLine("____________________________________________________");
            Console.WriteLine("Please choose from one of the following options. . .");
            Console.WriteLine("1. Deposit");
            Console.WriteLine("2. Withdraw");
            Console.WriteLine("3. Show Balance");
            Console.WriteLine("4. Exit");
            Console.WriteLine("____________________________________________________");
        }

        void deposit(bitcoinHolder currentUser)
        {
            Console.WriteLine("How much would you like to deposit: ");
            double deposit;
            while(!Double.TryParse(Console.ReadLine(), out deposit))
            {
                Console.WriteLine("Invalid format, please try again:");
            }
            currentUser.SetBalance(deposit + currentUser.GetBalance());
            UpdateBalanceInDatabase(currentUser);
            Console.WriteLine("Thank you for your deposit. Your new balance is: " + currentUser.GetBalance());
        }

        void withdraw(bitcoinHolder currentUser)
        {
            Console.WriteLine("How much would you like to withdraw: ");
            double withdraw;
            while(!Double.TryParse(Console.ReadLine(), out withdraw))
            {
                Console.WriteLine("Invalid input format, please try again:");
            }
            //check if we got enough of a balance
            if (currentUser.GetBalance() >= withdraw) 
            {
                currentUser.SetBalance(currentUser.GetBalance() - withdraw);
                UpdateBalanceInDatabase(currentUser);
                Console.WriteLine("You're all set. Your new balance is: " + currentUser.GetBalance());
            } else
            {
                string insufficientFunds = "Insufficient funds >:(";
                foreach(char c in insufficientFunds)
                {
                    Console.Write(c);
                    Thread.Sleep(50);
                }
                Console.WriteLine();
            }
        }

        void balance(bitcoinHolder currentUser)
        {
            Console.WriteLine("Your balance is: " + currentUser.GetBalance());
        }

        List<bitcoinHolder> bitcoinHolders = GetBitcoinHoldersFromDatabase();





        //intro prompt
        string introMessage = "Welcome to BitcoinBalance! \nLets get started with getting you authenticated. \nPlease enter your account number: ";
        
        foreach (char c in introMessage)
        {
            Console.Write(c);
            Thread.Sleep(50);
        }
        Console.WriteLine();

        String accNumber = "";
        bitcoinHolder currentUser;

        while(true)
        {
            try
            {
                accNumber = Console.ReadLine();
                //check list
                currentUser = bitcoinHolders.FirstOrDefault(a => a.accountNumber == accNumber);
                if(currentUser != null)
                {
                    break;
                } else
                {
                    Console.WriteLine("Account number not recognized. Please try again. ");
                }
            }
            catch
            {
                Console.WriteLine("Account number not recognized. Please try again. ");
            }
        }

        Console.WriteLine("Please enter account pin: ");
        int userPin = 0;
        while (true)
        {
            try
            {
                userPin = ReadPassword();
                //check list
                if (currentUser.GetPin() == userPin)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Incorrect pin, try again.");
                }

            }
            catch
            {
                Console.WriteLine("Incorrect pin, try again.");
            }
        }

            Console.WriteLine("Hello, " + currentUser.firstName + ". ☺");
            int option = 0;
            do
            {
                printOptions();
                try
                {
                    option = int.Parse(Console.ReadLine());
                }
                catch
                {
                }
                if (option == 1) { deposit(currentUser); }
                else if(option == 2) { withdraw(currentUser); }
                else if(option == 3) { balance(currentUser); }
                else if(option == 4) { break; }
                else { option = 0; }

            }
            while (option != 4);
            Console.WriteLine("Have a great day!");
        }
    }
