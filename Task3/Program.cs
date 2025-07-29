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

            long folderOldSize = GetDirectorySize(_folderPath);
            Console.WriteLine($"Размер папки до чистки: {folderOldSize} байт");

            TimeSpan maxUnusedTime = TimeSpan.FromMinutes(30);
            

            if(!CheckOldFiles(_folderPath, maxUnusedTime))
            {
                Console.WriteLine($"Нет файлов старше {maxUnusedTime.Minutes} минут.");
            }
            else
            {
                CleanOldFiles(_folderPath, maxUnusedTime);
                long folderSize = GetDirectorySize(_folderPath);
                Console.WriteLine($"Размер папки после чистки: {folderSize} байт");
            }
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

        static void CleanOldFiles(string folderPath, TimeSpan maxUnusedTime)
        {
            DateTime now = DateTime.Now;

            DeleteOldFiles(folderPath, maxUnusedTime, now);
            DeleteOldDirectories(folderPath, maxUnusedTime, now);
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
    }
}
