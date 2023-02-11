class Program
{
    private class TypeContainer
    {
        private Type type = typeof(int);
        public void set(Type type)
        {
            this.type = type;
        }

        public Type get()
        {
            return type;
        }
    }

    private class Menu
    {
        private readonly Dictionary<char, Menu> menuItems = new Dictionary<char, Menu>();
        private readonly Func<string>? message;
        private readonly String description;
        private readonly Action<char>? func;

        public Menu(String description, Func<string>? message)
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
                    char input = Console.ReadKey().KeyChar;

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
            System.Console.WriteLine(message());
            foreach (var pair in menuItems)
            {
                System.Console.WriteLine(pair.Key + " - " + pair.Value.description);
            }
        }
    }

    public static void Main(String[] args)
    {
        TypeContainer typeContainer = new TypeContainer();

        Menu mainMenu = new Menu("Выход в главное меню", () => "Информация по типам:");
        Menu quitMenu = new Menu("Выход из программы", c => System.Environment.Exit(1));
        Menu typeSelectMenu = new Menu("Выбрать тип из списка", () => "Информация по типам\nВыберите тип:\n------");

        Menu generalInfoMenu = new Menu("Общая информация по типам", c => {
            System.Console.WriteLine("Общая информация по типам");
            System.Console.WriteLine("Подключенные сборки: 17");
            System.Console.WriteLine("Всего типов по всем подключенным сборкам: 26103");
            System.Console.WriteLine("Ссылочные типы (только классы): 20601");
            System.Console.WriteLine("Значимые типы: 4377");
            System.Console.WriteLine("Информация в соответствии с вариантом №0");
            System.Console.WriteLine("...");
            System.Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню");
            Console.ReadKey();
            mainMenu.Run();
        });

        Menu typeInfoMenu = new Menu("Информация по типу", () =>
        {
            Type type = typeContainer.get();
            return $"Информация по типу: {type.FullName}\n" +
                $"    Значимый тип: {(type.IsValueType ? '+' : '-')}\n" +
                $"    Пространство имен: {type.Namespace}\n" +
                $"    Сборка: {type.Assembly.GetName().Name}\n" +
                $"    Общее число элементов: {type.GetFields().Length + type.GetMethods().Length + type.GetProperties().Length}\n" +
                $"    Число методов: {type.GetMethods().Length}\n" +
                $"    Число свойств: {type.GetProperties().Length}\n" +
                $"    Число пoлей: {type.GetFields().Length}\n" +
                $"    Список полей: {String.Join(", ", type.GetFields().ToList().ConvertAll(f => f.Name))}\n" +
                $"    Список свойств: {String.Join(", ", type.GetProperties().ToList().ConvertAll(f => f.Name))}\n";
        });

        Menu additionalTypeInfoMenu = new Menu("Вывод дополнительной информации по методам", () => {
            return "test";
        });

        Menu selectTypeUintMenu = new Menu("uint", c =>
        {
            typeContainer.set(typeof(uint));
            typeInfoMenu.Run();
        });
        Menu selectTypeIntMenu = new Menu("int", c =>
        {
            typeContainer.set(typeof(int));
            typeInfoMenu.Run();
        });
        Menu selectTypeLongMenu = new Menu("long", c =>
        {
            typeContainer.set(typeof(long));
            typeInfoMenu.Run();
        });
        Menu selectTypeFloatMenu = new Menu("float", c =>
        {
            typeContainer.set(typeof(float));
            typeInfoMenu.Run();
        });
        Menu selectTypeDoubleMenu = new Menu("double", c =>
        {
            typeContainer.set(typeof(double));
            typeInfoMenu.Run();
        });
        Menu selectTypeCharMenu = new Menu("char", c =>
        {
            typeContainer.set(typeof(char));
            typeInfoMenu.Run();
        });
        Menu selectTypeStringMenu = new Menu("string", c =>
        {
            typeContainer.set(typeof(string));
            typeInfoMenu.Run();
        });
        Menu selectTypeVectorMenu = new Menu("Vector", c =>
        {
            typeContainer.set(typeof(float));
            typeInfoMenu.Run();
        });
        Menu selectTypeMatrixMenu = new Menu("Matrix", c =>
        {
            typeContainer.set(typeof(float));
            typeInfoMenu.Run();
        });

        mainMenu.AddMenuItem('0', quitMenu);
        mainMenu.AddMenuItem('1', generalInfoMenu);
        mainMenu.AddMenuItem('2', typeSelectMenu);
        typeInfoMenu.AddMenuItem('0', mainMenu);

        typeSelectMenu.AddMenuItem('0', mainMenu);
        typeSelectMenu.AddMenuItem('1', selectTypeUintMenu);
        typeSelectMenu.AddMenuItem('2', selectTypeIntMenu);
        typeSelectMenu.AddMenuItem('3', selectTypeLongMenu);
        typeSelectMenu.AddMenuItem('4', selectTypeFloatMenu);
        typeSelectMenu.AddMenuItem('5', selectTypeDoubleMenu);
        typeSelectMenu.AddMenuItem('6', selectTypeCharMenu);
        typeSelectMenu.AddMenuItem('7', selectTypeStringMenu);
        typeSelectMenu.AddMenuItem('8', selectTypeVectorMenu);
        typeSelectMenu.AddMenuItem('9', selectTypeMatrixMenu);

        mainMenu.Run();
    }
}