using System;
using System.IO;

namespace Task2
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

            if (!ValidateFolderPath(_folderPath))
                return;

            long folderSize = GetDirectorySize(_folderPath);
            Console.WriteLine($"Размер папки: {folderSize} байт");

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

        static long GetDirectorySize(string path)
        {
            long size = 0;

            try
            {
                // Суммируем размеры всех файлов в текущей папке
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    size += fileInfo.Length;
                }

                // Рекурсивно добавляем размеры вложенных папок
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    size += GetDirectorySize(directory);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Нет доступа к файлу или папке: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подсчёте размера: {ex.Message}");
            }

            return size;
        }
    }
}
