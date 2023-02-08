class Program
{
    private class Menu
    {
        private readonly Dictionary<char, Menu> menuItems = new Dictionary<char, Menu>();
        private readonly String? message;
        private readonly String description;
        private readonly Action<char>? func;

        public Menu(String description, String message)
        {
            this.message = message;
            this.description = description;
        }

        public Menu(String description, Action<char> func)
        {
            this.description = description;
            this.func = func;
        }

        public void Run()
        {
            System.Console.Clear();
            if (func == null)
            {
                PrintMenuInfo();
                while (true)
                {
                    char input = Console.ReadLine()[0];

                    Menu? nextMenu;
                    if (menuItems.TryGetValue(input, out nextMenu))
                    {
                        nextMenu.Run();
                        break;
                    }
                    else
                    {
                        System.Console.Clear();
                        PrintMenuInfo();
                        System.Console.Write("Неправильный ввод, попробуйте еще: ");
                    }
                }
            }
            else
            {
                func('a');
            }
        }

        public void AddMenuItem(char id, Menu menu)
        {
            menuItems.Add(id, menu);
        }

        private void PrintMenuInfo()
        {
            System.Console.WriteLine(message);
            foreach (var pair in menuItems)
            {
                System.Console.WriteLine(pair.Key + " - " + pair.Value.description);
            }
        }
    }

    public static void Main(String[] args)
    {
        Menu mainMenu = new Menu("", "Информация по типам:");
        Menu quitMenu = new Menu("Выход из программы", c => System.Environment.Exit(1));

        mainMenu.AddMenuItem('0', quitMenu);

        mainMenu.Run();
    }
}