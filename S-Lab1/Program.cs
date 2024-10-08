using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class Program
{
    public class Meeting
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public List<string> ParticipantFiles { get; set; }
    }

    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
    }

    public interface IDataLoader
    {
        Meeting LoadMeeting(string filePath);
        User LoadUser(string filePath);
    }

    public class JsonDataLoader : IDataLoader
    {
        public Meeting LoadMeeting(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Meeting meeting = JsonSerializer.Deserialize<Meeting>(json);
            return meeting;
        }

        public User LoadUser(string filePath)
        {
            string json = File.ReadAllText(filePath);
            User user = JsonSerializer.Deserialize<User>(json);
            return user;
        }
    }

    public class XmlDataLoader : IDataLoader
    {
        public Meeting LoadMeeting(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Meeting));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                return (Meeting)serializer.Deserialize(fileStream);
            }
        }

        public User LoadUser(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(User));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                return (User)serializer.Deserialize(fileStream);
            }
        }
    }

    public abstract class DataLoaderFactory
    {
        public abstract IDataLoader CreateDataLoader();
    }

    public class JsonDataLoaderFactory : DataLoaderFactory
    {
        public override IDataLoader CreateDataLoader()
        {
            return new JsonDataLoader();
        }
    }

    public class XmlDataLoaderFactory : DataLoaderFactory
    {
        public override IDataLoader CreateDataLoader()
        {
            return new XmlDataLoader();
        }
    }

    public static void Main()
    {

        string fileFormat = "json";
        string meetingFilePath = @"C:\Users\ivale\source\repos\S-Lab1\S-Lab1\files\meeting.json";

        DataLoaderFactory factory;

        if (fileFormat == "json")
        {
            factory = new JsonDataLoaderFactory();
        }
        else if (fileFormat == "xml")
        {
            factory = new XmlDataLoaderFactory();
        }
        else
        {
            Console.WriteLine("Непідтримуваний формат файлу. Використовуйте json або xml.");
            return;
        }

        IDataLoader dataLoader = factory.CreateDataLoader();

        var meeting = dataLoader.LoadMeeting(meetingFilePath);

        Console.WriteLine($"Meeting on {meeting.Date}, URL: {meeting.URL}");
        Console.WriteLine($"Description: {meeting.Description}");

        List<User> participants = new List<User>();


        foreach (var participantFile in meeting.ParticipantFiles)
        {
            var user = dataLoader.LoadUser(participantFile);
            participants.Add(user);
            Console.WriteLine($"User {user.Name} (ID: {user.ID})");
        }

        Console.WriteLine("Всі учасники завантажені.");
    }
}
