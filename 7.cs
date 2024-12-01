using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EducationSystem
{
    public class User
    {
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public User(string fullName, string login, string password)
        {
            FullName = fullName;
            Login = login;
            Password = password;
        }
    }

    public class Student : User
    {
        public string Group { get; set; }
        public List<Grade> Grades { get; set; } = new();

        public Student(string fullName, string login, string password, string group)
            : base(fullName, login, password) => Group = group;

        public void ViewGrades()
        {
            Console.Clear();
            Console.WriteLine($"Оценки студента {FullName}:\n");
            Grades.ForEach(g => Console.WriteLine($"{g.Subject}: {g.Value} (Дата: {g.Date:dd.MM.yyyy})"));
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }

    public class Teacher : User
    {
        public List<string> Subjects { get; set; }
        public List<string> Groups { get; set; }

        public Teacher(string fullName, string login, string password, List<string> subjects, List<string> groups)
            : base(fullName, login, password)
        {
            Subjects = subjects;
            Groups = groups;
        }

        public void AddGrade(Student student, string subject, int gradeValue)
        {
            if (Subjects.Contains(subject) && Groups.Contains(student.Group))
            {
                student.Grades.Add(new Grade(subject, gradeValue));
                Console.WriteLine("Оценка успешно добавлена.");
            }
            else
            {
                Console.WriteLine("Недостаточно прав для выставления оценки.");
            }
        }
    }

    public class Admin : User
    {
        public Admin(string login, string password) : base("Администратор", login, password) { }

        public void ManageUsers<T>(List<T> users, Func<T> createUser) where T : User
        {
            Console.Clear();
            Console.WriteLine("1. Добавить\n2. Удалить\n3. Назад");
            var choice = Console.ReadKey(true).Key;
            if (choice == ConsoleKey.D1)
            {
                users.Add(createUser());
                Console.WriteLine("Пользователь добавлен.");
            }
            else if (choice == ConsoleKey.D2)
            {
                Console.Write("Введите логин для удаления: ");
                var user = users.FirstOrDefault(u => u.Login == Console.ReadLine());
                if (user != null)
                {
                    users.Remove(user);
                    Console.WriteLine("Пользователь удалён.");
                }
                else
                {
                    Console.WriteLine("Пользователь не найден.");
                }
            }
            Console.ReadKey();
        }
    }

    public class Grade
    {
        public string Subject { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public Grade(string subject, int value) => (Subject, Value) = (subject, value);
    }

    class Program
    {
        static List<Student> students = new();
        static List<Teacher> teachers = new();
        static Admin admin = new("admin", "admin123");

        static void Main() => LoadData();
    }
}
