using System;
using HSEkatkat.DI;
using HSEkatkat.Models;
using HSEkatkat.Services;

namespace HSEkatkat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Инициализация DI-контейнера
            var container = new DiContainer();
            container.Register<IServiceCenter, ServiceCenter>();
            container.Register<IItemFactory, ItemFactory>();
            container.Register<Manager, Manager>();
            
            // Получение готовых зависимостей через контейнер
            var manager = container.Resolve<Manager>();
            var factory = container.Resolve<IItemFactory>();
            
            bool running = true;
            while (running)
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }
                Console.WriteLine("Система учёта парка «HSEКат»\n");
                Console.WriteLine("1. Добавить технику");
                Console.WriteLine("2. Отчёт по числу единиц техники");
                Console.WriteLine("3. Отчёт по инвентарю");
                Console.WriteLine("4. Отчёт по энергии");
                Console.WriteLine("5. Техника для новичков");
                Console.WriteLine("0. Выйти\n");
                
                int choice = ReadInt("Ваш выбор: ", 0, 5);
                Console.WriteLine();
                
                switch (choice)
                {
                    case 1:
                        AddItemMenu(manager, factory);
                        break;
                    case 2:
                        ShowCountReport(manager);
                        break;
                    case 3:
                        ShowInventoryReport(manager);
                        break;
                    case 4:
                        ShowEnergyReport(manager);
                        break;
                    case 5:
                        ShowBeginnerReport(manager);
                        break;
                    case 0:
                        running = false;
                        Console.WriteLine("Завершение работы. До свидания!");
                        break;
                }
                if (running)
                {
                    Console.WriteLine("Нажмите Enter для продолжения...");
                    Console.ReadLine();
                }
            }
        }

        // Вспомогательные методы

        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int result) && result >= min && result <= max)
                {
                    return result;
                }

                Console.WriteLine($"Ошибка! Введите число от {min} до {max}.\n");
            }
        }
        private static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, out double result))
                {
                    return result;
                }
                Console.WriteLine("Ошибка! Введите корректное число.\n");
            }
        }
        
        private static void AddItemMenu(Manager manager, IItemFactory factory)
        {
            Console.WriteLine("Добавление техники\n");
            var types = factory.GetAvailableTypes();
            for (int i = 0; i < types.Length; i++) 
            {
                Console.WriteLine($"{i + 1}. {types[i]}");
            }
            Console.WriteLine("0. Отмена\n");

            int type = ReadInt("Выберите тип: ", 0, types.Length);
            if (type == 0) return;

            Console.Write("Инвентарный номер: ");
            string number = Console.ReadLine() ?? "";

            Console.Write("Название: ");
            string name = Console.ReadLine() ?? "";

            double energy = 0;
            int beginnerScore = 0;

            // Спрашиваем энергию только если тип её потребляет
            if (factory.HasEnergy(type)) 
            {
                energy = ReadDouble("Потребление энергии (кВт·ч): ");
            }

            // Спрашиваем оценку только если тип её имеет
            if (factory.HasBeginnerScore(type)) 
            {
                beginnerScore = ReadInt("Оценка для новичков (1-10): ", 1, 10);
            }
            var item = factory.CreateItem(type, number, name, energy, beginnerScore);
            if (manager.AddItem(item)) 
            {
                Console.WriteLine($"{item.Name} (id: {item.InventoryNumber}) успешно добавлен!");
            }
            else 
            {
                Console.WriteLine($"Не удалось добавить {item.Name}. Проверьте номер (возможно, он пуст или уже существует).");
            }
        }

        private static void ShowCountReport(Manager manager) 
        {
            Console.WriteLine("Отчёт по числу единиц техники");
            Console.WriteLine($"Всего единиц техники в парке: {manager.GetAllItems().Count}");
        }

        private static void ShowInventoryReport(Manager manager)
        {
            Console.WriteLine("Отчёт по инвентарю");
            var items = manager.GetAllItems();
            if (items.Count == 0)
            {
                Console.WriteLine("Парк пуст.\n");
                return;
            }
            foreach (var item in items)
            {
                Console.WriteLine($"id: {item.InventoryNumber} - {item.Name} ");
            }
            Console.WriteLine();
        }

        private static void ShowEnergyReport(Manager manager)
        {
            Console.WriteLine("Отчёт по энергии");
            Console.WriteLine($"Суммарное суточное потребление: {manager.GetTotalDailyEnergy()} кВт·ч");
        }

        private static void ShowBeginnerReport(Manager manager)
        {
            Console.WriteLine("Техника для новичков:");
            var items = manager.GetBeginnerFriendlyItems();
            if (items.Count == 0)
            {
                Console.WriteLine("Нет подходящей техники.");
                return;
            }
            foreach (var item in items)
            {
                Console.WriteLine($"id: {item.InventoryNumber} - {item.Name} ");
            }
            Console.WriteLine();
        }
    }
}