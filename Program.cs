using System;
using System.Collections.Generic;
using System.IO;

namespace Diary
{
    internal class Program
    {
        static void Main()
        {
            Diary diary = new Diary();

            while (true)
            {
                diary.PrintInstructions();

                Console.WriteLine("Zadej příkaz:");
                string command = Console.ReadLine();
                Console.Clear();

                switch (command)
                {

                    case "predchozi":
                        diary.ShowPreviousEntry();
                        break;
                    case "dalsi":
                        diary.ShowNextEntry();
                        break;
                    case "novy":
                        diary.AddNewEntry();
                        break;
                    case "uloz":
                        diary.SaveEntry();
                        break;
                    case "smaz":
                        diary.DeleteEntry();
                        break;
                    case "zavri":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Neplatný příkaz. Zkus to znovu.");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }

    class DiaryEntry
    {
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }

    class Diary
    {
        private LinkedList<DiaryEntry> entries = new LinkedList<DiaryEntry>();
        private LinkedListNode<DiaryEntry> currentEntry;
        public int Count { get { return entries.Count; } }

        private const string FileName = "diary.txt";

        public Diary()
        {
            LoadEntriesFromFile();
        }

        private void LoadEntriesFromFile()
        {
            if (File.Exists(FileName))
            {
                try
                {
                    string[] lines = File.ReadAllLines(FileName);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 2)
                        {
                            DateTime date;
                            if (DateTime.TryParseExact(parts[0], "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out date))
                            {
                                DiaryEntry entry = new DiaryEntry { Date = date, Text = parts[1] };
                                entries.AddLast(entry);
                            }
                        }
                    }
                    currentEntry = entries.Last;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Chyba při načítání záznamů: {e.Message}");
                    Console.ResetColor();
                }
            }
        }

        private void SaveEntriesToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FileName))
                {
                    foreach (DiaryEntry entry in entries)
                        writer.WriteLine($"{entry.Date.ToString("dd.MM.yyyy")},{entry.Text}");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání: {e.Message}");
                Console.ResetColor();
            }
        }

        public void ShowPreviousEntry()
        {
            if (currentEntry != null && currentEntry.Previous != null)
            {
                currentEntry = currentEntry.Previous;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Neexistuje předchozí záznam.");
                Console.ResetColor();
            }
        }

        public void ShowNextEntry()
        {
            if (currentEntry != null && currentEntry.Next != null)
            {
                currentEntry = currentEntry.Next;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Neexistuje následující záznam.");
                Console.ResetColor();
            }
        }

        public void AddNewEntry()
        {
            Console.WriteLine("Zadej datum (DD.MM.RRRR):");
            DateTime date;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out date))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nesprávný formát, znovu (DD.MM.RRRR):");
                Console.ResetColor();
            }

            Console.WriteLine("Zadej text záznamu (pro ukončení napiš 'konec-diare'):");

            string text = "";
            string line;
            while ((line = Console.ReadLine()) != "konec-diare")
                text += line + Environment.NewLine;

            DiaryEntry newEntry = new DiaryEntry { Date = date, Text = text };
            entries.AddLast(newEntry);
            currentEntry = entries.Last;

            SaveEntriesToFile();
        }

        public void SaveEntry()
        {
            if (currentEntry != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Záznam uložen.");
                Console.ResetColor();
                SaveEntriesToFile();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Neexistuje žádný záznam k uložení.");
                Console.ResetColor();
            }
        }

        public void DeleteEntry()
        {
            if (currentEntry != null)
            {
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("Pro odstranění tohoto záznamu stiskni 'a', pro zrušení jiný znak.");

                if (Console.ReadLine().ToLower() == "a")
                {
                    LinkedListNode<DiaryEntry> nextEntry = currentEntry.Next;
                    entries.Remove(currentEntry);
                    currentEntry = nextEntry;
                    if (currentEntry == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Záznam odstraněn.");
                        Console.ResetColor();
                    }

                    SaveEntriesToFile();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Neexistuje žádný aktuální záznam k odstranění.");
                Console.ResetColor();
            }
        }

        public void PrintInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nDeník se ovládá následujícími příkazy:");
            Console.WriteLine("- predchozi: Přesunutí na předchozí záznam");
            Console.WriteLine("- dalsi: Přesunutí na další záznam");
            Console.WriteLine("- novy: Vytvoření nového záznamu");
            Console.WriteLine("- uloz: Uložení vytvořeného záznamu");
            Console.WriteLine("- smaz: Odstranění záznamu");
            Console.WriteLine("- zavri: Zavření deníku");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Počet záznamů: {Count}");

            if (currentEntry != null)
            {
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine($"Aktuální záznam:");
                Console.WriteLine($"Datum: {currentEntry.Value.Date.ToString("dd.MM.yyyy")}");
                Console.WriteLine(currentEntry.Value.Text);
            }
            Console.ResetColor();
        }
    }
}
