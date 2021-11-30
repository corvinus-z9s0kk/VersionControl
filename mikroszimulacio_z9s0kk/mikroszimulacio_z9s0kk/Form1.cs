using mikroszimulacio_z9s0kk.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mikroszimulacio_z9s0kk
{
    public partial class Form1 : Form
    {
        List<Person> Population = new List<Person>();
        List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
        List<DeathProbability> DeathProbabilities = new List<DeathProbability>();
        Random rng = new Random(726);

        public Form1()
        {
            InitializeComponent();

            Population = GetPopulation(@"C:/Temp/nép.csv");
            BirthProbabilities = GetBirthProbabilities(@"C:/Temp/születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:/Temp/halál.csv");
            dataGridView1.DataSource = DeathProbabilities;
            for (int year = 2005;  year <= 2024; year++)
            {
                for (int i = 0; i < Population.Count; i++)
                {
                    SimStep(year, Population[i]);
                }

                int NumberOfMales = (from x in Population
                                     where x.Gender == Gender.Male && x.IsAlive
                                     select x).Count();
                int NumberOFemales = (from x in Population
                                      where x.Gender == Gender.Female && x.IsAlive
                                      select x).Count();
                Console.WriteLine(
                        string.Format("Év:{0} Fiúk:{1} Lányok:{2}", year, NumberOfMales, NumberOFemales));
            }
        }

        public void SimStep(int year, Person actualPerson)
        {
            if (!actualPerson.IsAlive) return;

            int age = (int)(year - actualPerson.BirthYear);

            var deathprob = (from x in DeathProbabilities
                             where x.Gender == actualPerson.Gender && x.Age == age
                             select x.P).FirstOrDefault();

            if (rng.NextDouble() <= deathprob) actualPerson.IsAlive = false;

            if (actualPerson.IsAlive && actualPerson.Gender == Gender.Female)
            {
                var birthprob = (from x in BirthProbabilities
                                 where x.Age == age
                                 select x.P).FirstOrDefault();

                if (rng.NextDouble() <= birthprob)
                {
                    Person újszülött = new Person();
                    újszülött.BirthYear = year;
                    újszülött.Gender = (Gender)rng.Next(1,3);
                    újszülött.NumberOfChildren = 0;
                    újszülött.IsAlive = true;
                    Population.Add(újszülött);
                }
            }
        }

        public List<Person> GetPopulation(string csvpath)
        {
            List<Person> population = new List<Person>();
            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NumberOfChildren = int.Parse(line[2])
                    });
                }
            };
            return population;
        }

        public List<BirthProbability> GetBirthProbabilities(string csvpath)
        {
            List<BirthProbability> birthProbabilities = new List<BirthProbability>();
            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    birthProbabilities.Add(new BirthProbability()
                    {
                        Age = int.Parse(line[0]),
                        NumberOfChildren = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    });
                }
            };
            return birthProbabilities;
        }

        public List<DeathProbability> GetDeathProbabilities(string csvpath)
        {
            List<DeathProbability> deathProbabilities = new List<DeathProbability>();
            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    deathProbabilities.Add(new DeathProbability()
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        Age = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    });
                }
            };
            return deathProbabilities;
        }
    }
}
