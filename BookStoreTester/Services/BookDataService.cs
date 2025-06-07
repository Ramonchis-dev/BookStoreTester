using BookStoreTester.Models;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace BookStoreTester.Services
{
    public class BookDataService
    {
        private readonly Random _random;
        private static readonly Dictionary<SupportedLocale, LocaleData> _localeData = new();

        static BookDataService()
        {
            InitializeLocaleData();
        }

        public BookDataService()
        {
            _random = new Random();
        }

        public List<Book> GenerateBooks(int startIndex, int count, SupportedLocale locale, int seed, double avgLikes, double avgReviews)
        {
            var books = new List<Book>();
            var localeData = _localeData[locale];

            for (int i = 0; i < count; i++)
            {
                var bookIndex = startIndex + i;
                var bookSeed = CombineSeeds(seed, bookIndex);
                var bookRandom = new Random(bookSeed);

                var book = new Book
                {
                    Index = bookIndex,
                    ISBN = GenerateISBN(bookRandom),
                    Title = GenerateTitle(bookRandom, localeData),
                    Authors = GenerateAuthors(bookRandom, localeData),
                    Publisher = GeneratePublisher(bookRandom, localeData),
                    CoverImageUrl = GenerateCoverImageUrl(bookIndex)
                };

                // Generate likes and reviews based on averages
                book.Likes = GenerateCount(bookRandom, avgLikes);
                book.Reviews = GenerateReviews(bookRandom, avgReviews, localeData);

                books.Add(book);
            }

            return books;
        }

        public string ExportToCsv(List<Book> books)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Index,ISBN,Title,Authors,Publisher,Likes,Reviews");

            foreach (var book in books)
            {
                var authors = string.Join("; ", book.Authors);
                var reviewCount = book.Reviews.Count;

                csv.AppendLine($"{book.Index},\"{book.ISBN}\",\"{EscapeCsv(book.Title)}\",\"{EscapeCsv(authors)}\",\"{EscapeCsv(book.Publisher)}\",{book.Likes},{reviewCount}");
            }

            return csv.ToString();
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains("\""))
                value = value.Replace("\"", "\"\"");
            return value;
        }

        private static int CombineSeeds(int userSeed, int pageNumber)
        {
            return userSeed * 31 + pageNumber * 17;
        }

        private static string GenerateISBN(Random random)
        {
            var isbn = new StringBuilder("978-");
            isbn.Append(random.Next(0, 10)).Append("-");
            isbn.Append(random.Next(10, 100)).Append("-");
            isbn.Append(random.Next(10000, 100000)).Append("-");
            isbn.Append(random.Next(0, 10));
            return isbn.ToString();
        }

        private static string GenerateTitle(Random random, LocaleData localeData)
        {
            var titleParts = new List<string>();
            var numParts = random.Next(1, 4);

            for (int i = 0; i < numParts; i++)
            {
                titleParts.Add(localeData.TitleWords[random.Next(localeData.TitleWords.Length)]);
            }

            return string.Join(" ", titleParts);
        }

        private static List<string> GenerateAuthors(Random random, LocaleData localeData)
        {
            var authorCount = random.Next(1, 4);
            var authors = new List<string>();

            for (int i = 0; i < authorCount; i++)
            {
                var firstName = localeData.FirstNames[random.Next(localeData.FirstNames.Length)];
                var lastName = localeData.LastNames[random.Next(localeData.LastNames.Length)];

                if (random.Next(10) < 3 && localeData.MiddleInitials.Length > 0) // 30% chance of middle initial
                {
                    var middleInitial = localeData.MiddleInitials[random.Next(localeData.MiddleInitials.Length)];
                    authors.Add($"{firstName} {middleInitial}. {lastName}");
                }
                else
                {
                    authors.Add($"{firstName} {lastName}");
                }
            }

            return authors;
        }

        private static string GeneratePublisher(Random random, LocaleData localeData)
        {
            return localeData.Publishers[random.Next(localeData.Publishers.Length)];
        }

        private static string GenerateCoverImageUrl(int bookIndex)
        {
            var colors = new[] { "FF6B6B", "4ECDC4", "45B7D1", "96CEB4", "FFEAA7", "DDA0DD", "98D8C8", "F7DC6F" };
            var color = colors[bookIndex % colors.Length];
            return $"https://via.placeholder.com/300x400/{color}/FFFFFF?text=Book+{bookIndex}";
        }

        private static int GenerateCount(Random random, double average)
        {
            if (average == 0) return 0;
            if (average >= 1) return (int)Math.Round(random.NextDouble() * average * 2);
            return random.NextDouble() < average ? 1 : 0;
        }

        private static List<Review> GenerateReviews(Random random, double avgReviews, LocaleData localeData)
        {
            var reviewCount = GenerateCount(random, avgReviews);
            var reviews = new List<Review>();

            for (int i = 0; i < reviewCount; i++)
            {
                var review = new Review
                {
                    Text = localeData.ReviewTexts[random.Next(localeData.ReviewTexts.Length)],
                    Author = $"{localeData.FirstNames[random.Next(localeData.FirstNames.Length)]} {localeData.LastNames[random.Next(localeData.LastNames.Length)]}",
                    Rating = Math.Round(random.NextDouble() * 4 + 1, 1) // 1.0 to 5.0
                };
                reviews.Add(review);
            }

            return reviews;
        }

        private static void InitializeLocaleData()
        {
            _localeData[SupportedLocale.EnglishUS] = new LocaleData
            {
                FirstNames = new[] { "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Christopher", "Karen", "Charles", "Nancy", "Daniel", "Lisa", "Matthew", "Betty", "Anthony", "Helen", "Mark", "Sandra" },
                LastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson" },
                MiddleInitials = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "R", "S", "T", "W" },
                TitleWords = new[] { "The", "Last", "First", "Great", "Secret", "Hidden", "Lost", "Final", "Dark", "Silent", "Golden", "Silver", "Ancient", "Modern", "Wild", "Brave", "Noble", "Forgotten", "Sacred", "Mysterious", "Journey", "Adventure", "Quest", "Story", "Tale", "Legend", "Chronicles", "Saga", "Mystery", "Shadow", "Light", "Fire", "Water", "Storm", "Mountain", "Forest", "Ocean", "Desert", "City", "Kingdom", "Empire", "War", "Peace", "Love", "Hope", "Dream", "Nightmare", "Vision", "Memory", "Promise", "Truth", "Lie", "Secret", "Code", "Key", "Door", "Path", "Bridge", "River", "Tree", "Flower", "Star", "Moon", "Sun", "Dawn", "Dusk", "Night", "Day" },
                Publishers = new[] { "Penguin Random House", "HarperCollins", "Macmillan", "Simon & Schuster", "Hachette", "Scholastic", "Wiley", "Pearson", "McGraw Hill", "Oxford University Press", "Cambridge University Press", "Bloomsbury", "Norton", "Vintage Books", "Doubleday", "Little Brown", "Bantam", "Del Rey", "Tor Books", "Ballantine" },
                ReviewTexts = new[] { "An absolutely captivating read that kept me turning pages late into the night.", "The character development is exceptional and the plot twists are genuinely surprising.", "A masterpiece of storytelling that will stay with you long after the final page.", "Beautifully written with rich descriptions and compelling dialogue.", "The author has created a world so vivid you can almost touch it.", "A thrilling adventure from start to finish with memorable characters.", "Thought-provoking themes handled with skill and sensitivity.", "The pacing is perfect and the ending is deeply satisfying.", "A powerful story that explores complex human emotions with grace.", "Expertly crafted with attention to every detail." }
            };

            _localeData[SupportedLocale.German] = new LocaleData
            {
                FirstNames = new[] { "Hans", "Anna", "Klaus", "Maria", "Wolfgang", "Elisabeth", "Jürgen", "Ursula", "Günter", "Ingrid", "Heinz", "Helga", "Gerhard", "Gisela", "Horst", "Brigitte", "Dieter", "Renate", "Manfred", "Christa", "Peter", "Monika", "Karl", "Petra", "Werner", "Angelika", "Helmut", "Gabriele", "Rainer", "Andrea" },
                LastNames = new[] { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann", "Schäfer", "Koch", "Bauer", "Richter", "Klein", "Wolf", "Schröder", "Neumann", "Schwarz", "Zimmermann", "Braun", "Krüger", "Hofmann", "Hartmann", "Lange", "Schmitt", "Werner", "Schmitz", "Krause", "Meier" },
                MiddleInitials = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "R", "S", "T", "W" },
                TitleWords = new[] { "Der", "Die", "Das", "Große", "Kleine", "Letzte", "Erste", "Geheime", "Verborgene", "Verlorene", "Dunkle", "Stille", "Goldene", "Silberne", "Alte", "Neue", "Wilde", "Mutige", "Vergessene", "Heilige", "Geheimnisvolle", "Reise", "Abenteuer", "Suche", "Geschichte", "Märchen", "Legende", "Chroniken", "Saga", "Geheimnis", "Schatten", "Licht", "Feuer", "Wasser", "Sturm", "Berg", "Wald", "Meer", "Wüste", "Stadt", "Königreich", "Reich", "Krieg", "Frieden", "Liebe", "Hoffnung", "Traum", "Alptraum", "Vision", "Erinnerung", "Versprechen", "Wahrheit", "Lüge", "Geheimnis", "Code", "Schlüssel", "Tür", "Pfad", "Brücke", "Fluss", "Baum", "Blume", "Stern", "Mond", "Sonne", "Morgendämmerung", "Abenddämmerung", "Nacht", "Tag" },
                Publishers = new[] { "Suhrkamp Verlag", "Fischer Verlag", "Rowohlt", "dtv", "Klett-Cotta", "Hanser Verlag", "Beck Verlag", "Piper Verlag", "Ullstein", "Heyne Verlag", "Goldmann Verlag", "Bastei Lübbe", "Aufbau Verlag", "Diogenes", "Kiepenheuer & Witsch", "Luchterhand", "Residenz Verlag", "Zsolnay Verlag", "Droemer Knaur", "Beltz Verlag" },
                ReviewTexts = new[] { "Ein absolut fesselndes Buch, das mich bis spät in die Nacht wachgehalten hat.", "Die Charakterentwicklung ist außergewöhnlich und die Wendungen wirklich überraschend.", "Ein Meisterwerk des Erzählens, das lange nach der letzten Seite bei mir bleibt.", "Wunderschön geschrieben mit reichen Beschreibungen und überzeugenden Dialogen.", "Der Autor hat eine Welt erschaffen, die so lebendig ist, dass man sie fast berühren kann.", "Ein spannendes Abenteuer von Anfang bis Ende mit unvergesslichen Charakteren.", "Nachdenkliche Themen, die mit Geschick und Feinfühligkeit behandelt werden.", "Das Tempo ist perfekt und das Ende zutiefst befriedigend.", "Eine kraftvolle Geschichte, die komplexe menschliche Emotionen mit Anmut erforscht.", "Meisterhaft gestaltet mit Aufmerksamkeit für jedes Detail." }
            };

            _localeData[SupportedLocale.Japanese] = new LocaleData
            {
                FirstNames = new[] { "Hiroshi", "Yuki", "Takeshi", "Akiko", "Satoshi", "Keiko", "Masahiro", "Naoko", "Kazuya", "Tomoko", "Shinji", "Mayumi", "Daisuke", "Sachiko", "Ryouta", "Kumiko", "Noboru", "Noriko", "Minoru", "Mariko", "Isamu", "Yoko", "Osamu", "Junko", "Makoto", "Kyoko", "Haruki", "Emiko", "Kenichi", "Michiko" },
                LastNames = new[] { "Sato", "Suzuki", "Takahashi", "Tanaka", "Watanabe", "Ito", "Yamamoto", "Nakamura", "Kobayashi", "Kato", "Yoshida", "Yamada", "Sasaki", "Yamaguchi", "Saito", "Matsumoto", "Inoue", "Kimura", "Hayashi", "Shimizu", "Yamazaki", "Mori", "Abe", "Ikeda", "Hashimoto", "Yamashita", "Ishikawa", "Nakajima", "Ogawa", "Goto" },
                MiddleInitials = new string[0], // Japanese names typically don't use middle initials
                TitleWords = new[] { "夜", "朝", "春", "夏", "秋", "冬", "花", "月", "星", "雲", "風", "雨", "雪", "海", "山", "川", "森", "空", "光", "影", "夢", "希望", "愛", "友情", "平和", "戦争", "旅", "冒険", "物語", "伝説", "秘密", "謎", "真実", "嘘", "記憶", "約束", "最後", "最初", "古い", "新しい", "大きな", "小さな", "美しい", "悲しい", "楽しい", "静か", "騒がしい", "暗い", "明るい", "深い", "浅い", "長い", "短い", "遠い", "近い", "高い", "低い", "強い", "弱い", "速い", "遅い", "若い", "老いた", "新鮮", "古い" },
                Publishers = new[] { "講談社", "集英社", "小学館", "新潮社", "文藝春秋", "岩波書店", "筑摩書房", "河出書房新社", "角川書店", "光文社", "徳間書店", "幻冬舎", "双葉社", "青春出版社", "大和書房", "PHP研究所", "三笠書房", "毎日新聞社", "朝日新聞社", "日本経済新聞社" },
                ReviewTexts = new[] { "深夜まで読み続けてしまう、とても魅力的な本でした。", "キャラクターの成長が素晴らしく、展開も本当に驚きの連続です。", "最後のページを読み終えた後も心に残る、物語の傑作です。", "美しい文章と説得力のある対話で綴られています。", "作者が創り出した世界は、まるで実際に触れられるほど生き生きしています。", "最初から最後まで、忘れられないキャラクターたちとのスリリングな冒険です。", "複雑なテーマを巧みで繊細な手法で扱っています。", "テンポが完璧で、結末は深く満足のいくものです。", "複雑な人間の感情を優雅に探求した力強い物語です。", "細部まで注意深く作られた、見事な作品です。" }
            };

            _localeData[SupportedLocale.French] = new LocaleData
            {
                FirstNames = new[] { "Pierre", "Marie", "Jean", "Anne", "Michel", "Françoise", "Philippe", "Monique", "Alain", "Catherine", "Bernard", "Sylvie", "Christian", "Martine", "Daniel", "Brigitte", "Jacques", "Nicole", "Paul", "Chantal", "André", "Isabelle", "François", "Nathalie", "Claude", "Véronique", "René", "Christine", "Henri", "Dominique" },
                LastNames = new[] { "Martin", "Bernard", "Thomas", "Petit", "Robert", "Richard", "Durand", "Dubois", "Moreau", "Laurent", "Simon", "Michel", "Lefebvre", "Leroy", "Roux", "David", "Bertrand", "Morel", "Fournier", "Girard", "Bonnet", "Dupont", "Lambert", "Fontaine", "Rousseau", "Vincent", "Müller", "Lefevre", "Faure", "Andre" },
                MiddleInitials = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "L", "M", "N", "P", "R", "S", "T" },
                TitleWords = new[] { "Le", "La", "Les", "Grand", "Grande", "Petit", "Petite", "Dernier", "Dernière", "Premier", "Première", "Secret", "Secrète", "Caché", "Cachée", "Perdu", "Perdue", "Sombre", "Silencieux", "Silencieuse", "Doré", "Dorée", "Argenté", "Argentée", "Ancien", "Ancienne", "Moderne", "Sauvage", "Courageux", "Courageuse", "Oublié", "Oubliée", "Sacré", "Sacrée", "Mystérieux", "Mystérieuse", "Voyage", "Aventure", "Quête", "Histoire", "Conte", "Légende", "Chroniques", "Saga", "Mystère", "Ombre", "Lumière", "Feu", "Eau", "Tempête", "Montagne", "Forêt", "Océan", "Désert", "Ville", "Royaume", "Empire", "Guerre", "Paix", "Amour", "Espoir", "Rêve", "Cauchemar", "Vision", "Mémoire", "Promesse", "Vérité", "Mensonge", "Code", "Clé", "Porte", "Chemin", "Pont", "Rivière", "Arbre", "Fleur", "Étoile", "Lune", "Soleil", "Aube", "Crépuscule", "Nuit", "Jour" },
                Publishers = new[] { "Gallimard", "Hachette", "Flammarion", "Seuil", "Albin Michel", "Fayard", "Grasset", "Plon", "Calmann-Lévy", "Robert Laffont", "Le Livre de Poche", "Pocket", "J'ai Lu", "10/18", "Points", "Folio", "Actes Sud", "Minuit", "L'École des Loisirs", "Nathan" },
                ReviewTexts = new[] { "Un livre absolument captivant qui m'a tenu éveillé tard dans la nuit.", "Le développement des personnages est exceptionnel et les rebondissements vraiment surprenants.", "Un chef-d'œuvre de narration qui restera avec vous longtemps après la dernière page.", "Magnifiquement écrit avec des descriptions riches et des dialogues convaincants.", "L'auteur a créé un monde si vivant qu'on peut presque le toucher.", "Une aventure palpitante du début à la fin avec des personnages mémorables.", "Des thèmes profonds traités avec habileté et sensibilité.", "Le rythme est parfait et la fin profondément satisfaisante.", "Une histoire puissante qui explore les émotions humaines complexes avec grâce.", "Magistralement conçu avec une attention à chaque détail." }
            };

            _localeData[SupportedLocale.Spanish] = new LocaleData
            {
                FirstNames = new[] { "Antonio", "María", "José", "Carmen", "Francisco", "Ana", "David", "Isabel", "Juan", "Dolores", "José Antonio", "Pilar", "Manuel", "María Teresa", "Luis", "Rosa", "Miguel", "Francisca", "Ángel", "Antonia", "Carlos", "Mercedes", "Pedro", "Josefa", "Jesús", "Concepción", "Rafael", "Rosario", "Daniel", "Manuela" },
                LastNames = new[] { "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez", "Gómez", "Martín", "Jiménez", "Ruiz", "Hernández", "Díaz", "Moreno", "Muñoz", "Álvarez", "Romero", "Alonso", "Gutiérrez", "Navarro", "Torres", "Domínguez", "Vázquez", "Ramos", "Gil", "Ramírez", "Serrano", "Blanco", "Suárez" },
                MiddleInitials = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "L", "M", "N", "P", "R", "S", "T" },
                TitleWords = new[] { "El", "La", "Los", "Las", "Grande", "Pequeño", "Pequeña", "Último", "Última", "Primero", "Primera", "Secreto", "Secreta", "Oculto", "Oculta", "Perdido", "Perdida", "Oscuro", "Oscura", "Silencioso", "Silenciosa", "Dorado", "Dorada", "Plateado", "Plateada", "Antiguo", "Antigua", "Moderno", "Moderna", "Salvaje", "Valiente", "Noble", "Olvidado", "Olvidada", "Sagrado", "Sagrada", "Misterioso", "Misteriosa", "Viaje", "Aventura", "Búsqueda", "Historia", "Cuento", "Leyenda", "Crónicas", "Saga", "Misterio", "Sombra", "Luz", "Fuego", "Agua", "Tormenta", "Montaña", "Bosque", "Océano", "Desierto", "Ciudad", "Reino", "Imperio", "Guerra", "Paz", "Amor", "Esperanza", "Sueño", "Pesadilla", "Visión", "Memoria", "Promesa", "Verdad", "Mentira", "Código", "Llave", "Puerta", "Camino", "Puente", "Río", "Árbol", "Flor", "Estrella", "Luna", "Sol", "Amanecer", "Atardecer", "Noche", "Día" },
                Publishers = new[] { "Planeta", "Santillana", "Anagrama", "Alfaguara", "Seix Barral", "Tusquets", "Destino", "Lumen", "Random House", "Espasa", "Crítica", "Alianza Editorial", "Cátedra", "Biblioteca Nueva", "Taurus", "Siglo XXI", "Debate", "Ariel", "Paidós", "Galaxia Gutenberg" },
                ReviewTexts = new[] { "Un libro absolutamente cautivador que me mantuvo despierto hasta altas horas.", "El desarrollo de los personajes es excepcional y los giros realmente sorprendentes.", "Una obra maestra de la narrativa que permanecerá contigo mucho después de la última página.", "Bellamente escrito con descripciones ricas y diálogos convincentes.", "El autor ha creado un mundo tan vívido que casi puedes tocarlo.", "Una aventura emocionante de principio a fin con personajes memorables.", "Temas profundos tratados con habilidad y sensibilidad.", "El ritmo es perfecto y el final profundamente satisfactorio.", "Una historia poderosa que explora emociones humanas complejas con gracia.", "Magistralmente elaborado con atención a cada detalle." }
            };
        }

        private class LocaleData
        {
            public string[] FirstNames { get; set; } = Array.Empty<string>();
            public string[] LastNames { get; set; } = Array.Empty<string>();
            public string[] MiddleInitials { get; set; } = Array.Empty<string>();
            public string[] TitleWords { get; set; } = Array.Empty<string>();
            public string[] Publishers { get; set; } = Array.Empty<string>();
            public string[] ReviewTexts { get; set; } = Array.Empty<string>();
        }
    }
}