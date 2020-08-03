public class Program
    {

        private static bool IsOpenWorkingAni = false;
        private static int counter;
        private static string DisplayPath = "";

        static void Main(string[] args)
        {

            Console.WriteLine("Reading Settings....\r\n");

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "dirs.txt"))
            {
                Console.WriteLine("Please Set dirs.txt on root !");
                return;
            }

            var froms = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "dirs.txt");
            froms = froms.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            froms = froms.Select(x => (x.LastIndexOf('\\') > -1 ? x.Substring(0, (x.LastIndexOf('\\'))) : x)).ToArray();


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Dirsctories List\r\n=================");
            for (var i = 0; i < froms.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + froms[i]);
            }

            Console.ForegroundColor = ConsoleColor.White;


            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "destination.txt"))
            {
                Console.WriteLine("Please Set destination.txt on root !");
                return;
            }
            var targetStr = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "destination.txt");
            targetStr = (targetStr.LastIndexOf('\\') > -1) ? targetStr.Substring(0, targetStr.LastIndexOf('\\')) : targetStr;



            Directory.CreateDirectory(targetStr);
            var traget = new DirectoryInfo(targetStr);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.Write("Target ==>");
            Console.WriteLine(traget.FullName + "\r\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Start Compressing......\r\n");

            //顯示動畫
            Console.ForegroundColor = ConsoleColor.White;
            IsOpenWorkingAni = true;
            Task.Run(() =>
            {
                while (IsOpenWorkingAni)
                {
                    Turn(DisplayPath);
                    Thread.Sleep(100);
                }
            });


            //載入 7z Lib
            SevenZipCompressor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "x86" + Path.DirectorySeparatorChar + "7z.dll");
            Console.ForegroundColor = ConsoleColor.White;

            var successCount = 0;
            var errorCount = 0;

            var sp = new Stopwatch();
            foreach (var f in froms)
            {
                //Check Dir is Empty 如果裡面沒有資料會被 throw Exception 所以先檢查
                if (!Directory.EnumerateFileSystemEntries(f).Any())
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(f + " is empty so skip.\r\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                sp.Restart();
                try
                {
                    DisplayPath = f + " => " + f.Split('\\').Last() + ".7z";
                    SevenZipCompressor compressor = new SevenZipCompressor();
                    compressor.CompressDirectory(f, targetStr + Path.DirectorySeparatorChar + f.Split('\\').Last() + ".7z");
                    sp.Stop();

                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine();
                    Console.SetCursorPosition(0, Console.CursorTop);


                    ClearCurrentConsoleLine();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Success => " + targetStr + Path.DirectorySeparatorChar + f.Split('\\').Last() + ".7z , Cost Time:" + sp.Elapsed + "\r\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error => " + targetStr + Path.DirectorySeparatorChar + f.Split('\\').Last() + ".7z" + " | " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    errorCount++;
                    continue;
                }

            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Success Tasks : " + successCount + ", ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Fail Tasks : " + errorCount);
            Console.ForegroundColor = ConsoleColor.White;



        }


        /// <summary>
        /// 清除該行文字
        /// https://stackoverflow.com/questions/8946808/can-console-clear-be-used-to-only-clear-a-line-instead-of-whole-console
        /// </summary>
        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <summary>
        /// Working Animation
        /// https://stackoverflow.com/questions/1923323/console-animations
        /// </summary>
        /// <param name="displayPath"></param>
        private static void Turn(string displayPath)
        {
            counter++;
            Console.Write(displayPath + "   ");
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - (displayPath.Length + 4), Console.CursorTop);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);

        }
    }
