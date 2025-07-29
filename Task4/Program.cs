using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Task4.Models;

namespace Task4
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = GetBinaryFilePath().Trim('"');
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            List<Student> students = LoadStudentsFromBinaryFile(filePath);

            if (students.Count == 0)
            {
                Console.WriteLine("Файл не содержит информации или произошла ошибка при чтении.");
                return;
            }

            string outputDirectory = CreateStudentsDirectoryOnDesktop();
            SaveStudentsByGroups(students, outputDirectory);

            Console.WriteLine($"Готово! Файлы созданы {outputDirectory}.");
        }

        static string GetBinaryFilePath()
        {
            Console.WriteLine("Введите путь к базе:");
            return Console.ReadLine();
        }

        static List<Student> LoadStudentsFromBinaryFile(string filePath)
        {
            List<Student> students = new List<Student>();

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8))
                {
                    while (fs.Position < fs.Length)
                    {
                        string name = reader.ReadString();
                        string group = reader.ReadString();
                        long ticks = reader.ReadInt64();
                        DateTime dateOfBirth = new DateTime(ticks);
                        decimal averageGrade = reader.ReadDecimal();

                        students.Add(new Student
                        {
                            Name = name,
                            Group = group,
                            DateOfBirth = dateOfBirth,
                            AverageGrade = averageGrade
                        });
                    }
                }
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("Чтение завершено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при чтении файла: " + ex.Message);
            }

            return students;
        }

        static string CreateStudentsDirectoryOnDesktop()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string directoryPath = Path.Combine(desktopPath, "Students");
            Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        static void SaveStudentsByGroups(List<Student> students, string outputDirectory)
        {
            var grouped = students.GroupBy(s => s.Group);

            foreach (var group in grouped)
            {
                string groupFile = Path.Combine(outputDirectory, group.Key + ".txt");

                using (StreamWriter writer = new StreamWriter(groupFile, false, Encoding.UTF8))
                {
                    foreach (var student in group)
                    {
                        writer.WriteLine($"{student.Name}, {student.DateOfBirth:dd.MM.yyyy}, {student.AverageGrade:F2}");
                    }
                }
            }
        }
    }
}
