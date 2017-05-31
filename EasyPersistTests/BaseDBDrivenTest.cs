using EasyPersist.Tests.Helpers;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace EasyPersist.Tests
{
    public class BaseDBDrivenText
    {
        public static string[] FirstNames = new string[] { "Joshua", "Michael", "Anthony", "Christopher", "Jacob", "Daniel", "Matthew", "David", "Alexander", "Nicholas", "Christian", "Ethan", "Jonathan", "Tyler", "Joseph", "William", "Gabriel", "James", "Logan", "Ryan", "Noah", "Andrew", "Dylan", "Jordan", "Brandon", "John", "Kevin", "Jayden", "Elijah", "Nathan", "Sebastian", "Jose", "Samuel", "Zachary", "Justin", "Isaiah", "Aiden", "Robert", "Austin", "Caleb", "Angel", "Benjamin", "Gavin", "Jeremiah", "Luis", "Thomas", "Lucas", "Jason", "Julian", "Bryan", "Adrian", "Evan", "Mason", "Aidan", "Juan", "Landon", "Connor", "Jackson", "Cameron", "Hunter", "Luke", "Sean", "Carlos", "Xavier", "Jack", "Brian", "Alex", "Alejandro", "Diego", "Charles", "Kyle", "Aaron", "Isaac", "Nathaniel", "Antonio", "Eric", "Nicolas", "Jaden", "Tristan", "Hayden", "Chase", "Richard", "Dominic", "Adam", "Ian", "Devin", "Steven", "Wyatt", "Miguel", "Bryce", "Giovanni", "Blake", "Joel", "Patrick", "Victor", "Cole", "Marcus", "Caden", "Jeremy", "Jorge", "Isabella", "Emily", "Madison", "Sophia", "Emma", "Ava", "Olivia", "Mia", "Hannah", "Brianna", "Abigail", "Ashley", "Samantha", "Sarah", "Alyssa", "Elizabeth", "Kayla", "Victoria", "Alexis", "Jasmine", "Chloe", "Hailey", "Natalie", "Taylor", "Angelina", "Gabriella", "Sofia", "Grace", "Savannah", "Nicole", "Destiny", "Alexandra", "Brooke", "Lauren", "Jessica", "Makayla", "Maria", "Alexa", "Ella", "Gabriela", "Melanie", "Kaitlyn", "Julia", "Stephanie", "Jennifer", "Sydney", "Jada", "Anna", "Lily", "Morgan", "Andrea", "Arianna", "Trinity", "Katherine", "Nevaeh", "Amanda", "Ariana", "Addison", "Kaylee", "Rachel", "Riley", "Kylie", "Faith", "Gabrielle", "Maya", "Gianna", "Mackenzie", "Natalia", "Zoe", "Jayla", "Vanessa", "Leah", "Aaliyah", "Katelyn", "Sara", "Adriana", "Lillian", "Haley", "Michelle", "Allison", "Layla", "Amelia", "Daniela", "Jade", "Mariah", "Megan", "Briana", "Valeria", "Jordan", "Kimberly", "Mya", "Camila", "Isabelle", "Isabel", "Jocelyn", "Avery", "Rebecca", "Melissa", "Jenna", "Evelyn", "Paige" };
        public static string[] LastNames = new string[] { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter", "Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed", "Cook", "Morgan", "Bell", "Murphy", "Bailey", "Rivera", "Cooper", "Richardson", "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly", "Sanders", "Price", "Bennett", "Wood", "Barnes", "Ross", "Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long", "Patterson", "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales", "Bryant", "Alexander", "Russell", "Griffin", "Diaz", "Hayes", "Myers", "Ford", "Hamilton", "Graham", "Sullivan", "Wallace", "Woods", "Cole", "West", "Jordan", "Owens", "Reynolds", "Fisher", "Ellis", "Harrison", "Gibson", "Mcdonald", "Cruz", "Marshall", "Ortiz", "Gomez", "Murray", "Freeman", "Wells", "Webb", "Simpson", "Stevens", "Tucker", "Porter", "Hunter", "Hicks", "Crawford", "Henry", "Boyd", "Mason", "Morales", "Kennedy", "Warren", "Dixon", "Ramos", "Reyes", "Burns", "Gordon", "Shaw", "Holmes", "Rice", "Robertson", "Hunt", "Black", "Daniels", "Palmer", "Mills", "Nichols", "Grant", "Knight", "Ferguson", "Rose", "Stone", "Hawkins", "Dunn", "Perkins", "Hudson", "Spencer", "Gardner", "Stephens", "Payne", "Pierce", "Berry", "Matthews", "Arnold", "Wagner", "Willis", "Ray", "Watkins", "Olson", "Carroll", "Duncan", "Snyder", "Hart", "Cunningham", "Bradley", "Lane", "Andrews", "Ruiz", "Harper", "Fox", "Riley", "Armstrong", "Carpenter", "Weaver", "Greene", "Lawrence", "Elliott", "Chavez", "Sims", "Austin", "Peters", "Kelley", "Franklin", "Lawson", "Fields", "Gutierrez", "Ryan", "Schmidt", "Carr", "Vasquez", "Castillo", "Wheeler", "Chapman", "Oliver", "Montgomery", "Richards", "Williamson", "Johnston", "Banks", "Meyer", "Bishop", "Mccoy", "Howell", "Alvarez", "Morrison", "Hansen", "Fernandez", "Garza", "Harvey", "Little", "Burton", "Stanley", "Nguyen", "George", "Jacobs", "Reid", "Kim", "Fuller", "Lynch", "Dean", "Gilbert", "Garrett", "Romero", "Welch", "Larson", "Frazier", "Burke", "Hanson", "Day", "Mendoza", "Moreno", "Bowman", "Medina", "Fowler", "Brewer", "Hoffman", "Carlson", "Silva", "Pearson", "Holland", "Douglas", "Fleming", "Jensen", "Vargas", "Byrd", "Davidson", "Hopkins", "May", "Terry", "Herrera", "Wade", "Soto", "Walters", "Curtis", "Neal", "Caldwell", "Lowe", "Jennings", "Barnett", "Graves", "Jimenez", "Horton", "Shelton", "Barrett", "Obrien", "Castro", "Sutton", "Gregory", "Mckinney", "Lucas", "Miles", "Craig", "Rodriquez", "Chambers", "Holt", "Lambert", "Fletcher", "Watts", "Bates", "Hale", "Rhodes", "Pena", "Beck", "Newman", "Haynes", "Mcdaniel", "Mendez", "Bush", "Vaughn", "Parks", "Dawson", "Santiago", "Norris", "Hardy", "Love", "Steele", "Curry", "Powers", "Schultz", "Barker", "Guzman", "Page", "Munoz", "Ball", "Keller", "Chandler", "Weber", "Leonard", "Walsh", "Lyons", "Ramsey", "Wolfe", "Schneider", "Mullins", "Benson", "Sharp", "Bowen", "Daniel", "Barber", "Cummings", "Hines", "Baldwin", "Griffith", "Valdez", "Hubbard", "Salazar", "Reeves", "Warner", "Stevenson", "Burgess", "Santos", "Tate", "Cross", "Garner", "Mann", "Mack", "Moss", "Thornton", "Dennis", "Mcgee", "Farmer", "Delgado", "Aguilar", "Vega", "Glover", "Manning", "Cohen", "Harmon", "Rodgers", "Robbins", "Newton", "Todd", "Blair", "Higgins", "Ingram", "Reese", "Cannon", "Strickland", "Townsend", "Potter", "Goodwin", "Walton", "Rowe", "Hampton", "Ortega", "Patton", "Swanson", "Joseph", "Francis", "Goodman", "Maldonado", "Yates", "Becker", "Erickson", "Hodges", "Rios", "Conner", "Adkins", "Webster", "Norman", "Malone", "Hammond", "Flowers", "Cobb", "Moody", "Quinn", "Blake", "Maxwell", "Pope", "Floyd", "Osborne", "Paul", "Mccarthy", "Guerrero", "Lindsey", "Estrada", "Sandoval", "Gibbs", "Tyler", "Gross", "Fitzgerald", "Stokes", "Doyle", "Sherman", "Saunders", "Wise", "Colon", "Gill", "Alvarado", "Greer", "Padilla", "Simon", "Waters", "Nunez", "Ballard", "Schwartz", "Mcbride", "Houston", "Christensen", "Klein", "Pratt", "Briggs", "Parsons", "Mclaughlin", "Zimmerman", "French", "Buchanan", "Moran", "Copeland", "Roy", "Pittman", "Brady", "Mccormick", "Holloway", "Brock", "Poole", "Frank", "Logan", "Owen", "Bass", "Marsh", "Drake", "Wong", "Jefferson", "Park", "Morton", "Abbott", "Sparks", "Patrick", "Norton", "Huff", "Clayton", "Massey", "Lloyd", "Figueroa", "Carson", "Bowers", "Roberson", "Barton", "Tran", "Lamb", "Harrington", "Casey", "Boone", "Cortez", "Clarke", "Mathis", "Singleton", "Wilkins", "Cain", "Bryan", "Underwood", "Hogan", "Mckenzie", "Collier", "Luna", "Phelps", "Mcguire", "Allison", "Bridges", "Wilkerson", "Nash", "Summers", "Atkins", "Wilcox", "Pitts", "Conley", "Marquez", "Burnett", "Richard", "Cochran", "Chase", "Davenport", "Hood", "Gates", "Clay", "Ayala", "Sawyer", "Roman", "Vazquez", "Dickerson", "Hodge", "Acosta", "Flynn", "Espinoza", "Nicholson", "Monroe", "Wolf", "Morrow", "Kirk", "Randall", "Anthony", "Whitaker", "Oconnor", "Skinner", "Ware", "Molina", "Kirby", "Huffman", "Bradford", "Charles", "Gilmore", "Dominguez", "Oneal", "Bruce", "Lang", "Combs", "Kramer", "Heath", "Hancock", "Gallagher", "Gaines", "Shaffer", "Short", "Wiggins", "Mathews", "Mcclain", "Fischer", "Wall", "Small", "Melton", "Hensley", "Bond", "Dyer", "Cameron", "Grimes", "Contreras", "Christian", "Wyatt", "Baxter", "Snow", "Mosley", "Shepherd", "Larsen", "Hoover", "Beasley", "Glenn", "Petersen", "Whitehead", "Meyers", "Keith", "Garrison", "Vincent", "Shields", "Horn", "Savage", "Olsen", "Schroeder", "Hartman", "Woodard", "Mueller", "Kemp", "Deleon", "Booth", "Patel", "Calhoun", "Wiley", "Eaton", "Cline", "Navarro", "Harrell", "Lester", "Humphrey", "Parrish", "Duran", "Hutchinson", "Hess", "Dorsey", "Bullock", "Robles", "Beard", "Dalton", "Avila", "Vance", "Rich", "Blackwell", "York", "Johns", "Blankenship", "Trevino", "Salinas", "Campos", "Pruitt", "Moses", "Callahan", "Golden", "Montoya", "Hardin", "Guerra", "Mcdowell", "Carey", "Stafford", "Gallegos", "Henson", "Wilkinson", "Booker", "Merritt", "Miranda", "Atkinson", "Orr", "Decker", "Hobbs", "Preston", "Tanner", "Knox", "Pacheco", "Stephenson", "Glass", "Rojas", "Serrano", "Marks", "Hickman", "English", "Sweeney", "Strong", "Prince", "Mcclure", "Conway", "Walter", "Roth", "Maynard", "Farrell", "Lowery", "Hurst", "Nixon", "Weiss", "Trujillo", "Ellison", "Sloan", "Juarez", "Winters", "Mclean", "Randolph", "Leon", "Boyer", "Villarreal", "Mccall", "Gentry", "Carrillo", "Kent", "Ayers", "Lara", "Shannon", "Sexton", "Pace", "Hull", "Leblanc" };

        protected DatabaseParam DBParam
            = new DatabaseParam() { 
                ServerName = "localhost\\SQL2016", 
                MasterPassword = "z", 
                MasterLogin = "sa",
                DatabaseName = "EasyPersistTestsUT"
            };
        protected string ConnectionSting { get; set; }


        public BaseDBDrivenText()
        {
            ConnectionSting = "Data Source=" + DBParam.ServerName
                              + ";Initial Catalog=" + DBParam.DatabaseName
                              + ";Persist Security Info=True;User ID=" + DBParam.MasterLogin
                              + ";Password=" + DBParam.MasterPassword;
            LoggingConfiguration config = new LoggingConfiguration();
            // Step 2. Create targets and add them to the configuration 
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            DebugTarget debugTarget = new DebugTarget();
            config.AddTarget("debug", debugTarget);
            // Step 3. Set target properties
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            debugTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            // Step 4. Define rules 
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);
            LoggingRule rule2 = new LoggingRule("*", LogLevel.Trace, debugTarget);
            config.LoggingRules.Add(rule2);
            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }

        public static string GetRandomFirstName(int i)
        {

            //int rndIndex = random.Next(FirstNames.Length);
            int rndIndex = (i / LastNames.Length) % FirstNames.Length;
            return FirstNames[rndIndex];
        }

        public static string GetRandomLastName(int i)
        {
            //int rndIndex = random.Next(LastNames.Length);
            int rndIndex = i % LastNames.Length;
            return LastNames[rndIndex];
        }

        public static string GetRandomEmail(string fullname, string seed, int i)
        {
            string email = fullname.ToLower().Replace(' ', '.');
            email = email + i + seed + "@mailinator.com";
            return email;
        }
    }
}
