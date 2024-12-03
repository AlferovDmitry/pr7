using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Класс для хранения информации о студенте
public class Student
{
    public string FullName; // Полное имя студента
    public int Age; // Возраст студента
    public int BirthYear; // Год рождения студента
    public string Group; // Группа студента
    public string Login; // Логин студента
    public string Password; // Пароль студента
    public List<Grade> Grades; // Список оценок студента

    public Student()
    {
        Grades = new List<Grade>();
    }
}

// Класс для хранения информации об оценке
public struct Grade
{
    public string Subject; // Название предмета
    public int Score; // Оценка
    public DateTime Date; // Дата выставления оценки
}

public class Admin
{
    public string Login { get; set; } // Логин администратора
    public string Password { get; set; } // Пароль администратора
}

public class Program
{
    // Функция сохранения данных студента в бинарный файл
    public static void SaveStudent(Student student, string filename)
    {
        using (BinaryWriter writer = new BinaryWriter(new FileStream(filename, FileMode.Create)))
        {
            writer.Write(student.FullName); // Запись полного имени
            writer.Write(student.Age); // Запись возраста
            writer.Write(student.BirthYear); // Запись года рождения
            writer.Write(student.Group); // Запись группы
            writer.Write(student.Login); // Запись логина
            writer.Write(student.Password); // Запись пароля

            writer.Write(student.Grades.Count); // Запись количества оценок
            foreach (var grade in student.Grades)
            {
                writer.Write(grade.Subject); // Запись названия предмета
                writer.Write(grade.Score); // Запись оценки
                writer.Write(grade.Date.ToBinary()); // Запись даты в бинарном формате
            }
        }
    }

    // Функция загрузки данных студента из бинарного файла
    public static Student LoadStudent(string filename)
    {
        Student student = new Student();
        using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
        {
            student.FullName = reader.ReadString(); // Чтение полного имени
            student.Age = reader.ReadInt32(); // Чтение возраста
            student.BirthYear = reader.ReadInt32(); // Чтение года рождения
            student.Group = reader.ReadString(); // Чтение группы
            student.Login = reader.ReadString(); // Чтение логина
            student.Password = reader.ReadString(); // Чтение пароля

            int gradeCount = reader.ReadInt32(); // Чтение количества оценок
            for (int i = 0; i < gradeCount; i++)
            {
                student.Grades.Add(new Grade
                {
                    Subject = reader.ReadString(), // Чтение названия предмета
                    Score = reader.ReadInt32(), // Чтение оценки
                    Date = DateTime.FromBinary(reader.ReadInt64()) // Чтение даты из бинарного формата
                });
            }
        }
        return student;
    }

    public static void UpdateStudentGrade(string filename, string subject, int newScore)
    {

        Student student = LoadStudent(filename);

        Grade gradeToUpdate = student.Grades.FirstOrDefault(g => g.Subject == subject); // Поиск оценки для изменения
        if (gradeToUpdate != null)
        {
            gradeToUpdate.Score = newScore; // Изменение оценки
        }
        else
        {
            Console.WriteLine($"Оценка по предмету {subject} не найдена."); //Сообщение об ошибке
            return; // Выход из функции
        }


        SaveStudent(student, filename); // Сохранение изменений
    }

    public static void Main(string[] args)
    {
        // Пример использования:
        Student student = new Student
        {
            FullName = "Дмитрий Алферов", // Изменено имя
            Age = 20,
            BirthYear = 2003,
            Group = "БСТ-22",
            Login = "dmitryalferov",
            Password = "password123" 
        };
        student.Grades.Add(new Grade { Subject = "Основы алгоритмизации и программирования", Score = 4, Date = DateTime.Now }); // Изменен предмет и оценка
        student.Grades.Add(new Grade { Subject = "Технические средства информатизации", Score = 4, Date = DateTime.Now.AddDays(-5) }); // Изменен предмет


        SaveStudent(student, "student.dat"); // Сохранение данных в файл
        Student loadedStudent = LoadStudent("student.dat"); // Загрузка данных из файла

        Console.WriteLine($"Загруженное имя: {loadedStudent.FullName}"); // Вывод загруженных данных
        foreach (var grade in loadedStudent.Grades)
        {
            Console.WriteLine($"Предмет: {grade.Subject}, Оценка: {grade.Score}, Дата: {grade.Date}");
        }

        UpdateStudentGrade("student.dat", "Основы алгоритмизации и программирования", 5); // Изменение оценки

    }
}
