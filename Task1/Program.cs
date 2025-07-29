using System.IO;
using System;
using System.ComponentModel.Design;

namespace Task1
{
    internal class Program
    {
        static string _folderPath { get; set; }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Введите путь к папке");
                _folderPath = Console.ReadLine();
            }
            else
            {
                _folderPath = args[0];
            }

            if (string.IsNullOrEmpty(_folderPath))
            {
                Console.WriteLine("Ошибка: не указан путь до папки.");
                Console.ReadLine();
            }

            if (!ValidateFolderPath(_folderPath))
                return;

            TimeSpan maxUnusedTime = TimeSpan.FromMinutes(30);
            CheckOldFiles(_folderPath, maxUnusedTime);

            CleanOldFiles(_folderPath, maxUnusedTime);

            Console.WriteLine("Очистка завершена.");
            Console.ReadLine();
        }

        static bool ValidateFolderPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Ошибка: путь не должен быть пустым.");
                return false;
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Ошибка: папка \"{path}\" не существует.");
                return false;
            }

            return true;
        }

        static void CleanOldFiles(string folderPath, TimeSpan maxUnusedTime)
        {
            DateTime now = DateTime.Now;

            DeleteOldFiles(folderPath, maxUnusedTime, now);
            DeleteOldDirectories(folderPath, maxUnusedTime, now);
        }

        static bool CheckOldFiles(string folderPath, TimeSpan maxUnusedTime)
        {
            var now = DateTime.Now;
            bool isExist = false;
            try
            {
                foreach (string file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                
                    DateTime lastAccess = File.GetLastAccessTime(file);
                    if ((now - lastAccess) > maxUnusedTime)
                    {
                        Console.WriteLine($"Будет удалено: {file}");
                        isExist = true;
                    }
               
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет доступа к {folderPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении : {ex.Message}");
            }
            return isExist;
        }
        static void DeleteOldFiles(string folderPath, TimeSpan maxUnusedTime, DateTime now)
        {
            try
            {
                foreach (string file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                
                    DateTime lastAccess = File.GetLastAccessTime(file);
                    if ((now - lastAccess) > maxUnusedTime)
                    {
                        File.Delete(file);
                        Console.WriteLine($"Удален файл: {file}");
                    }
                
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Нет доступа : {folderPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
            }
        }

        static void DeleteOldDirectories(string folderPath, TimeSpan maxUnusedTime, DateTime now)
        {
            string[] directories = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
            Array.Sort(directories, (a, b) => b.Length.CompareTo(a.Length));

            foreach (string dir in directories)
            {
                try
                {
                    DateTime lastAccess = Directory.GetLastAccessTime(dir);
                    if ((now - lastAccess) > maxUnusedTime)
                    {
                        Directory.Delete(dir, true);
                        Console.WriteLine($"Удалена папка: {dir}");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Нет доступа к папке: {dir}");
                }
                catch (IOException)
                {
                    Console.WriteLine($"Папка не пуста или занята: {dir}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении папки {dir}: {ex.Message}");
                }
            }
        }
    }
}