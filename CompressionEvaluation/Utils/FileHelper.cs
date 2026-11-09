using System.Text;

namespace CompressionEvaluation.Utils
{
    public static class FileHelper
    {
        public static void CreateSampleTestFiles(string directory, bool forceOverwrite = false)
        {
            Directory.CreateDirectory(directory);

            Console.WriteLine("\n Creating sample test files...");

            string textFile = Path.Combine(directory, "sample_text.txt");
            if (forceOverwrite || !File.Exists(textFile))
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 10000; i++)
                {
                    sb.AppendLine("This is a test line with repetitive content. " +
                                 "AAAA BBBB CCCC DDDD EEEE FFFF GGGG HHHH IIII JJJJ");
                }
                File.WriteAllText(textFile, sb.ToString());
                Console.WriteLine($"    Created: {Path.GetFileName(textFile)}");
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(textFile)} (already exists)");
            }

            string binaryFile = Path.Combine(directory, "sample_random.bin");
            if (forceOverwrite || !File.Exists(binaryFile))
            {
                var random = new Random();
                byte[] data = new byte[1024 * 1024];
                random.NextBytes(data);
                File.WriteAllBytes(binaryFile, data);
                Console.WriteLine($"Created: {Path.GetFileName(binaryFile)}");
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(binaryFile)} (already exists)");
            }

            string repetitiveFile = Path.Combine(directory, "sample_repetitive.txt");
            if (forceOverwrite || !File.Exists(repetitiveFile))
            {
                var sb = new StringBuilder();

                sb.AppendLine(new string('A', 50000));
                sb.AppendLine(new string('B', 50000));
                sb.AppendLine(new string('C', 50000));
                sb.AppendLine(new string('D', 50000));
                sb.AppendLine(new string('E', 50000));
                sb.AppendLine(new string('F', 50000));
                sb.AppendLine(new string('G', 50000));
                sb.AppendLine(new string('H', 50000));
                sb.AppendLine(new string('I', 50000));
                sb.AppendLine(new string('J', 50000));

                for (int i = 0; i < 100; i++)
                {
                    sb.AppendLine(new string('X', 10000));
                    sb.AppendLine(new string('Y', 10000));
                    sb.AppendLine(new string('Z', 10000));
                }

                File.WriteAllText(repetitiveFile, sb.ToString());
                Console.WriteLine($"    Created: {Path.GetFileName(repetitiveFile)} (RLE will be BEST for this file!)");
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(repetitiveFile)} (already exists)");
            }

            string jsonFile = Path.Combine(directory, "sample_data.json");
            if (forceOverwrite || !File.Exists(jsonFile))
            {
                var sb = new StringBuilder();
                sb.AppendLine("{");
                sb.AppendLine("  \"products\": [");
                for (int i = 0; i < 1000; i++)
                {
                    sb.AppendLine("    {");
                    sb.AppendLine($"      \"id\": {i},");
                    sb.AppendLine($"      \"name\": \"Product {i}\",");
                    sb.AppendLine($"      \"description\": \"Description for product {i}\",");
                    sb.AppendLine($"      \"price\": {i * 10.99},");
                    sb.AppendLine($"      \"category\": \"Category {i % 10}\"");
                    sb.AppendLine($"    }},");
                }
                sb.AppendLine("  ]");
                sb.AppendLine("}");
                File.WriteAllText(jsonFile, sb.ToString());
                Console.WriteLine($"    Created: {Path.GetFileName(jsonFile)}");
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(jsonFile)} (already exists)");
            }

            string csvFile = Path.Combine(directory, "sample_data.csv");
            if (forceOverwrite || !File.Exists(csvFile))
            {
                CreateCsvFile(directory, forceOverwrite);
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(csvFile)} (already exists)");
            }

            string sqlFile = Path.Combine(directory, "sample_database.sql");
            if (forceOverwrite || !File.Exists(sqlFile))
            {
                CreateSqlDump(directory, forceOverwrite);
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(sqlFile)} (already exists)");
            }

            string xmlFile = Path.Combine(directory, "sample_data.xml");
            if (forceOverwrite || !File.Exists(xmlFile))
            {
                CreateXmlFile(directory, forceOverwrite);
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(xmlFile)} (already exists)");
            }

            string lz4SensorFile = Path.Combine(directory, "lz4_sensor_realtime.bin");
            if (forceOverwrite || !File.Exists(lz4SensorFile))
            {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                var random = new Random(42);

                for (int packet = 0; packet < 100000; packet++)
                {
                    writer.Write(packet);
                    writer.Write(random.Next(0, 100));
                    writer.Write(random.Next(0, 100));
                    writer.Write(random.Next(0, 100));
                }
                File.WriteAllBytes(lz4SensorFile, ms.ToArray());
                Console.WriteLine($"    Created: {Path.GetFileName(lz4SensorFile)} (LZ4 will be FASTEST - real-time!)");
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(lz4SensorFile)} (already exists)");
            }

            string lz4SpeedFile = Path.Combine(directory, "lz4_speed_test.bin");
            if (forceOverwrite || !File.Exists(lz4SpeedFile))
            {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                var random = new Random(42);

                for (int i = 0; i < 200000; i++)
                {
                    writer.Write(random.Next(0, 65535));
                }
                File.WriteAllBytes(lz4SpeedFile, ms.ToArray());
                Console.WriteLine($"    Created: {Path.GetFileName(lz4SpeedFile)} (LZ4 will be FASTEST)");
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(lz4SpeedFile)} (already exists)");
            }

            string zstdBalanceFile = Path.Combine(directory, "zstd_balance_test.bin");
            if (forceOverwrite || !File.Exists(zstdBalanceFile))
            {
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                var random = new Random(42);

                for (int i = 0; i < 50000; i++)
                {
                    for (int j = 0; j < 20; j++)
                        writer.Write((byte)65);

                    writer.Write(random.Next(0, 255));
                    writer.Write(random.Next(0, 255));
                }
                File.WriteAllBytes(zstdBalanceFile, ms.ToArray());
                Console.WriteLine($"   Created: {Path.GetFileName(zstdBalanceFile)} (Zstd will be BEST BALANCED)");
            }
            else
            {
                Console.WriteLine($"   Skipped: {Path.GetFileName(zstdBalanceFile)} (already exists)");
            }

            string streamingFile = Path.Combine(directory, "sample_stream_data.txt");
            if (forceOverwrite || !File.Exists(streamingFile))
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 20000; i++)
                {
                    sb.AppendLine($"[{DateTime.Now.AddSeconds(-i):HH:mm:ss}] INFO Processing chunk {i}");
                    sb.AppendLine($"Data: {Guid.NewGuid():N}");
                    sb.AppendLine($"Checksum: {i * 12345:X8}");
                }
                File.WriteAllText(streamingFile, sb.ToString());
                Console.WriteLine($"   Created: {Path.GetFileName(streamingFile)} (GZip will be BEST FOR COMPATIBILITY!)");
            }
            else
            {
                Console.WriteLine($"  Skipped: {Path.GetFileName(streamingFile)} (already exists)");
            }

            string largeTextFile = Path.Combine(directory, "large_text_25mb.txt");
            if (forceOverwrite || !File.Exists(largeTextFile))
            {
                CreateLargeTextFile(largeTextFile);
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(largeTextFile)} (already exists)");
            }

            string largeBinaryFile = Path.Combine(directory, "large_binary_20mb.bin");
            if (forceOverwrite || !File.Exists(largeBinaryFile))
            {
                CreateLargeBinaryFile(largeBinaryFile);
            }
            else
            {
                Console.WriteLine($"    Skipped: {Path.GetFileName(largeBinaryFile)} (already exists)");
            }
        }

        private static void CreateCsvFile(string directory, bool forceOverwrite)
        {
            string csvFile = Path.Combine(directory, "sample_data.csv");
            var sb = new StringBuilder();

            sb.AppendLine("TransactionID,Date,ProductName,Category,Quantity,Price,Total,Country,Status");

            string[] products = { "Laptop", "Mouse", "Keyboard", "Monitor", "USB Cable", "Headphones", "Webcam", "Speaker" };
            string[] categories = { "Electronics", "Accessories", "Peripherals", "Audio" };
            string[] countries = { "USA", "Canada", "UK", "Germany", "France", "Japan", "Brazil", "Australia" };
            string[] statuses = { "Completed", "Pending", "Shipped", "Delivered", "Cancelled" };

            var random = new Random(42);

            for (int i = 1; i <= 10000; i++)
            {
                string product = products[random.Next(products.Length)];
                string category = categories[random.Next(categories.Length)];
                string country = countries[random.Next(countries.Length)];
                string status = statuses[random.Next(statuses.Length)];

                int quantity = random.Next(1, 10);
                decimal price = Math.Round((decimal)(random.Next(10, 1000) + random.NextDouble()), 2);
                decimal total = Math.Round(quantity * price, 2);

                string date = DateTime.Now.AddDays(-random.Next(0, 365)).ToString("yyyy-MM-dd");

                sb.AppendLine($"{i},{date},{product},{category},{quantity},{price:F2},{total:F2},{country},{status}");
            }

            File.WriteAllText(csvFile, sb.ToString());
            Console.WriteLine($"Created: {Path.GetFileName(csvFile)} ({FormatBytes(new FileInfo(csvFile).Length)})");
        }

        private static void CreateSqlDump(string directory, bool forceOverwrite)
        {
            string sqlFile = Path.Combine(directory, "sample_database.sql");
            var sb = new StringBuilder();

            sb.AppendLine("-- =============================================");
            sb.AppendLine("-- SAMPLE DATABASE DUMP FOR COMPRESSION TESTING");
            sb.AppendLine("-- Generated: " + DateTime.Now);
            sb.AppendLine("-- =============================================\n");

            sb.AppendLine("DROP TABLE IF EXISTS users;");
            sb.AppendLine("CREATE TABLE users (");
            sb.AppendLine("    id INT PRIMARY KEY,");
            sb.AppendLine("    username VARCHAR(50) NOT NULL,");
            sb.AppendLine("    email VARCHAR(100) NOT NULL,");
            sb.AppendLine("    country VARCHAR(50),");
            sb.AppendLine("    age INT,");
            sb.AppendLine("    registration_date DATE,");
            sb.AppendLine("    is_active BOOLEAN");
            sb.AppendLine(");\n");

            sb.AppendLine("DROP TABLE IF EXISTS orders;");
            sb.AppendLine("CREATE TABLE orders (");
            sb.AppendLine("    order_id INT PRIMARY KEY,");
            sb.AppendLine("    user_id INT,");
            sb.AppendLine("    product_name VARCHAR(100),");
            sb.AppendLine("    quantity INT,");
            sb.AppendLine("    price DECIMAL(10,2),");
            sb.AppendLine("    order_date DATE,");
            sb.AppendLine("    status VARCHAR(20)");
            sb.AppendLine(");\n");

            sb.AppendLine("-- Inserting users...");
            string[] countries = { "USA", "Canada", "UK", "Germany", "France", "Japan", "Brazil", "Australia", "India", "Spain" };
            string[] firstNames = { "John", "Jane", "Michael", "Sarah", "David", "Emma", "James", "Lisa", "Robert", "Maria" };
            string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };

            var random = new Random(42);

            for (int i = 1; i <= 50000; i++)
            {
                string firstName = firstNames[random.Next(firstNames.Length)];
                string lastName = lastNames[random.Next(lastNames.Length)];
                string username = $"{firstName.ToLower()}.{lastName.ToLower()}{i % 1000}";
                string email = $"{username}@example.com";
                string country = countries[random.Next(countries.Length)];
                int age = 18 + random.Next(50);
                string regDate = DateTime.Now.AddDays(-random.Next(0, 1095)).ToString("yyyy-MM-dd");
                bool isActive = random.Next(2) == 1;

                sb.AppendLine($"INSERT INTO users VALUES ({i}, '{username}', '{email}', '{country}', {age}, '{regDate}', {isActive.ToString().ToLower()});");

                if (i % 5000 == 0)
                {
                    sb.AppendLine($"-- {i} users inserted...");
                }
            }

            sb.AppendLine("\n-- Inserting orders...");

            string[] products = { "Laptop Pro", "Wireless Mouse", "Mechanical Keyboard", "USB-C Hub", "Monitor 27\"", "External SSD", "Webcam HD", "Noise Cancelling Headphones", "Tablet", "Smart Watch" };
            string[] statuses = { "Pending", "Processing", "Shipped", "Delivered", "Cancelled", "Refunded" };

            for (int i = 1; i <= 100000; i++)
            {
                int userId = random.Next(1, 50001);
                string product = products[random.Next(products.Length)];
                int quantity = random.Next(1, 5);
                decimal price = Math.Round((decimal)(random.Next(20, 2000) + random.NextDouble()), 2);
                string orderDate = DateTime.Now.AddDays(-random.Next(0, 365)).ToString("yyyy-MM-dd");
                string status = statuses[random.Next(statuses.Length)];

                sb.AppendLine($"INSERT INTO orders VALUES ({i}, {userId}, '{product}', {quantity}, {price}, '{orderDate}', '{status}');");

                if (i % 10000 == 0)
                {
                    sb.AppendLine($"-- {i} orders inserted...");
                }
            }

            sb.AppendLine("\n-- Creating indexes for better performance");
            sb.AppendLine("CREATE INDEX idx_users_email ON users(email);");
            sb.AppendLine("CREATE INDEX idx_orders_user_id ON orders(user_id);");
            sb.AppendLine("CREATE INDEX idx_orders_order_date ON orders(order_date);");
            sb.AppendLine("\n-- =============================================");
            sb.AppendLine("-- DUMP COMPLETED SUCCESSFULLY");
            sb.AppendLine("-- =============================================");

            File.WriteAllText(sqlFile, sb.ToString());
            Console.WriteLine($"Created: {Path.GetFileName(sqlFile)} ({FormatBytes(new FileInfo(sqlFile).Length)})");
        }

        private static void CreateXmlFile(string directory, bool forceOverwrite)
        {
            string xmlFile = Path.Combine(directory, "sample_data.xml");
            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<?xml-stylesheet type=\"text/xsl\" href=\"style.xsl\"?>");
            sb.AppendLine("<!-- Sample XML File for Compression Testing -->");
            sb.AppendLine("<!DOCTYPE catalog [");
            sb.AppendLine("    <!ELEMENT catalog (product*)>");
            sb.AppendLine("    <!ELEMENT product (id, name, description, price, category, specifications, reviews, metadata)>");
            sb.AppendLine("    <!ATTLIST product status (active|inactive) \"active\">");
            sb.AppendLine("]>");
            sb.AppendLine("<catalog xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            sb.AppendLine("    <metadata>");
            sb.AppendLine("        <generated_date>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</generated_date>");
            sb.AppendLine("        <total_products>10000</total_products>");
            sb.AppendLine("        <version>2.0</version>");
            sb.AppendLine("        <company>Compression Test Labs</company>");
            sb.AppendLine("    </metadata>");

            string[] categories = { "Electronics", "Clothing", "Books", "Home & Garden", "Sports", "Toys", "Automotive", "Health", "Beauty", "Food" };
            string[] adjectives = { "Premium", "Deluxe", "Basic", "Pro", "Ultra", "Standard", "Advanced", "Essential", "Professional", "Compact" };
            string[] materials = { "Plastic", "Metal", "Wood", "Glass", "Fabric", "Leather", "Rubber", "Ceramic", "Carbon Fiber", "Stainless Steel" };

            var random = new Random(42);

            for (int i = 1; i <= 10000; i++)
            {
                string status = random.Next(10) < 2 ? "inactive" : "active";
                string category = categories[random.Next(categories.Length)];
                string adjective = adjectives[random.Next(adjectives.Length)];
                string material = materials[random.Next(materials.Length)];
                string productName = $"{adjective} Product {i}";
                decimal price = Math.Round((decimal)(random.Next(10, 1000) + random.NextDouble()), 2);
                int stock = random.Next(0, 1000);
                double rating = Math.Round(3 + random.NextDouble() * 2, 1);
                int reviewCount = random.Next(0, 500);

                sb.AppendLine($"    <product status=\"{status}\">");
                sb.AppendLine($"        <id>{i}</id>");
                sb.AppendLine($"        <name>{productName}</name>");
                sb.AppendLine($"        <description>This is a {adjective.ToLower()} product made of {material}. Perfect for {category.ToLower()} enthusiasts. High quality and durable.</description>");
                sb.AppendLine($"        <price currency=\"USD\">{price:F2}</price>");
                sb.AppendLine($"        <category>{category}</category>");
                sb.AppendLine($"        <specifications>");
                sb.AppendLine($"            <material>{material}</material>");
                sb.AppendLine($"            <weight>{random.Next(100, 5000) / 1000.0:F1}kg</weight>");
                sb.AppendLine($"            <dimensions>{random.Next(10, 100)}x{random.Next(10, 100)}x{random.Next(5, 50)}cm</dimensions>");
                sb.AppendLine($"            <color>Color{random.Next(1, 20)}</color>");
                sb.AppendLine($"            <warranty>{random.Next(1, 5)} years</warranty>");
                sb.AppendLine($"        </specifications>");
                sb.AppendLine($"        <reviews>");
                sb.AppendLine($"            <average_rating>{rating:F1}</average_rating>");
                sb.AppendLine($"            <total_reviews>{reviewCount}</total_reviews>");
                sb.AppendLine($"            <positive_feedback>{random.Next(50, 100)}%</positive_feedback>");
                sb.AppendLine($"        </reviews>");
                sb.AppendLine($"        <metadata>");
                sb.AppendLine($"            <created_date>{DateTime.Now.AddDays(-random.Next(0, 730)):yyyy-MM-dd}</created_date>");
                sb.AppendLine($"            <last_updated>{DateTime.Now.AddDays(-random.Next(0, 30)):yyyy-MM-dd}</last_updated>");
                sb.AppendLine($"            <stock_quantity>{stock}</stock_quantity>");
                sb.AppendLine($"            <shipping_weight>{random.Next(100, 5000) / 1000.0:F1}kg</shipping_weight>");
                sb.AppendLine($"            <barcode>{random.NextInt64(1000000000000, 9999999999999)}</barcode>");
                sb.AppendLine($"        </metadata>");
                sb.AppendLine($"    </product>");

                if (i % 1000 == 0)
                {
                    sb.AppendLine($"    <!-- {i} products generated -->");
                }
            }

            sb.AppendLine("</catalog>");

            File.WriteAllText(xmlFile, sb.ToString());
            Console.WriteLine($" Created: {Path.GetFileName(xmlFile)} ({FormatBytes(new FileInfo(xmlFile).Length)})");
        }

        private static void CreateLargeTextFile(string path)
        {
            Console.WriteLine("    Creating large text file (~25 MB, this may take a moment)...");

            var paragraph = new StringBuilder();
            paragraph.AppendLine("The quick brown fox jumps over the lazy dog while the sun sets slowly behind");
            paragraph.AppendLine("the distant mountains. In the quiet valley below, a small river winds its way");
            paragraph.AppendLine("through ancient forests where deer graze peacefully in the fading light. Every");
            paragraph.AppendLine("evening the same scene unfolds: birds return to their nests, farmers finish");
            paragraph.AppendLine("their work in the fields, and the first stars begin to appear in the darkening");
            paragraph.AppendLine("sky. This is the rhythm of rural life, unchanged for generations, where each");
            paragraph.AppendLine("season brings its own familiar patterns and the passage of time is measured");
            paragraph.AppendLine("not by clocks but by the turning of the earth and the cycles of nature.");
            paragraph.AppendLine();

            string paragraphText = paragraph.ToString();

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20);
            using var writer = new StreamWriter(stream);

            const long targetSizeBytes = 25L * 1024 * 1024;
            long written = 0;
            int section = 0;

            while (written < targetSizeBytes)
            {
                string header = $"=== Section {section} ===\n";
                writer.Write(header);
                written += header.Length;

                writer.Write(paragraphText);
                written += paragraphText.Length;

                string uniqueLine = $"Record: {section:D6} | ID: {Guid.NewGuid():N} | Value: {section * 37 % 10000}\n";
                writer.Write(uniqueLine);
                written += uniqueLine.Length;

                section++;
            }

            writer.Flush();
            Console.WriteLine($"    Created: {Path.GetFileName(path)} ({FormatBytes(new FileInfo(path).Length)}) — parallel will shine here!");
        }

        private static void CreateLargeBinaryFile(string path)
        {
            Console.WriteLine("    Creating large binary file (~20 MB)...");

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20);
            using var writer = new BinaryWriter(stream);

            var random = new Random(12345);

            const long targetSizeBytes = 20L * 1024 * 1024;
            int recordCount = (int)(targetSizeBytes / 20);

            for (int i = 0; i < recordCount; i++)
            {
                writer.Write(i);
                writer.Write((byte)(i % 16));
                writer.Write((byte)random.Next(0, 4));
                writer.Write((short)random.Next(0, 1000));
                writer.Write((short)(i % 200));

                for (int p = 0; p < 8; p++)
                    writer.Write(p == 4 ? (byte)random.Next(0, 256) : (byte)0);
            }

            writer.Flush();
            Console.WriteLine($"    Created: {Path.GetFileName(path)} ({FormatBytes(new FileInfo(path).Length)}) — mixed compressibility, shows parallel balance!");
        }

        public static string[] GetTestFiles(string directory)
        {
            if (!Directory.Exists(directory))
                return new string[0];

            return Directory.GetFiles(directory, "*.*")
                .Where(f => {
                    string ext = Path.GetExtension(f).ToLower();
                    return !f.EndsWith(".compressed") &&
                           !f.EndsWith(".rle") &&
                           !f.EndsWith(".br") &&
                           !f.EndsWith(".gz") &&
                           !f.EndsWith(".gzip") &&
                           !f.EndsWith(".lz4") &&
                           !f.EndsWith(".zst");
                })
                .ToArray();
        }

        public static void DisplayInputFiles(string directory)
        {
            var files = GetTestFiles(directory);

            if (files.Length == 0)
            {
                Console.WriteLine("No files found in input directory.");
                return;
            }

            Console.WriteLine($"\nFound {files.Length} file(s) to test:");
            Console.WriteLine(new string('-', 70));

            var grouped = files.GroupBy(f => Path.GetExtension(f).ToLower());

            foreach (var group in grouped.OrderBy(g => g.Key))
            {
                string extension = string.IsNullOrEmpty(group.Key) ? "(no extension)" : group.Key;
                Console.WriteLine($"\n   {extension.ToUpper()} files ({group.Count()}):");
                foreach (var file in group)
                {
                    var info = new FileInfo(file);
                    Console.WriteLine($" {Path.GetFileName(file),-35} {FormatBytes(info.Length),10}");
                }
            }
            Console.WriteLine();
        }

        public static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:F2} {sizes[order]}";
        }

        public static void DisplayFileInfo(string filePath)
        {
            var info = new FileInfo(filePath);
            Console.WriteLine($"File: {Path.GetFileName(filePath)}");
            Console.WriteLine($"Size: {FormatBytes(info.Length)}");
            Console.WriteLine($"Extension: {Path.GetExtension(filePath)}");
        }
    }
}