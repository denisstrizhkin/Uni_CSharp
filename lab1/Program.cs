﻿namespace lab1; 

internal abstract class Program {
    private class TypeContainer {
        private Type _type = typeof(int);

        public void Set(Type type) {
            _type = type;
        }

        public Type Get() {
            return _type;
        }
    }

    private class Menu {
        private readonly Dictionary<char, Menu> _menuItems = new Dictionary<char, Menu>();
        private readonly Func<string>? _message;
        private readonly string _description;
        private readonly Action? _func;

        public Menu(string description, Func<string>? message) {
            _message = message;
            _description = description;
        }

        public Menu(string description, Action func) {
            _description = description;
            _func = func;
        }

        public void Run() {
            Console.Clear();
            if (_func == null) {
                PrintMenuInfo();
                while (true) {
                    var input = char.ToUpper(Console.ReadKey().KeyChar);

                    if (_menuItems.TryGetValue(input, out var nextMenu)) {
                        nextMenu.Run();
                        break;
                    }

                    Console.Clear();
                    PrintMenuInfo();
                    Console.Write("Неправильный ввод, попробуйте еще: ");
                }
            }
            else {
                _func();
            }
        }

        public void AddMenuItem(char id, Menu menu) {
            _menuItems.Add(char.ToUpper(id), menu);
        }

        private void PrintMenuInfo() {
            if (_message != null) {
                Console.WriteLine(_message());
            }
            foreach (var pair in _menuItems) {
                Console.WriteLine(pair.Key + " - " + pair.Value._description);
            }
        }
    }

    public static void Main(string[] args) {
        var typeContainer = new TypeContainer();

        var mainMenu = new Menu("Выход в главное меню", () => "Информация по типам:");
        var quitMenu = new Menu("Выход из программы", () => Environment.Exit(1));
        var typeSelectMenu = new Menu("Выбрать тип из списка", () => "Информация по типам\nВыберите тип:\n------");

        var generalInfoMenu = new Menu("Общая информация по типам", () => {
            Console.WriteLine("Общая информация по типам");
            Console.WriteLine("Подключенные сборки: 17");
            Console.WriteLine("Всего типов по всем подключенным сборкам: 26103");
            Console.WriteLine("Ссылочные типы (только классы): 20601");
            Console.WriteLine("Значимые типы: 4377");
            Console.WriteLine("Информация в соответствии с вариантом №0");
            Console.WriteLine("...");
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню");
            Console.ReadKey();
            mainMenu.Run();
        });

        var typeInfoMenu = new Menu("Информация по типу", () => {
            var type = typeContainer.Get();
            return $"Информация по типу: {type.FullName}\n" +
                   $"    Значимый тип: {(type.IsValueType ? '+' : '-')}\n" +
                   $"    Пространство имен: {type.Namespace}\n" +
                   $"    Сборка: {type.Assembly.GetName().Name}\n" +
                   $"    Общее число элементов: {type.GetFields().Length + type.GetMethods().Length + type.GetProperties().Length}\n" +
                   $"    Число методов: {type.GetMethods().Length}\n" +
                   $"    Число свойств: {type.GetProperties().Length}\n" +
                   $"    Число полей: {type.GetFields().Length}\n" +
                   $"    Список полей: {string.Join(", ", type.GetFields().ToList().ConvertAll(f => f.Name))}\n" +
                   $"    Список свойств: {string.Join(", ", type.GetProperties().ToList().ConvertAll(f => f.Name))}\n";
        });

        var additionalTypeInfoMenu = new Menu("Вывод дополнительной информации по методам", () => {
            var methods = typeContainer.Get().GetMethods();
            var overloads = new Dictionary<string, int>();

            foreach (var m in methods) {
                if (overloads.ContainsKey(m.Name)) {
                    overloads[m.Name]++;
                }
                else {
                    overloads.Add(m.Name, 1);
                }
            }

            return "test";
        });

        var selectTypeUintMenu = new Menu("uint", () => {
            typeContainer.Set(typeof(uint));
            typeInfoMenu.Run();
        });
        var selectTypeIntMenu = new Menu("int", () => {
            typeContainer.Set(typeof(int));
            typeInfoMenu.Run();
        });
        var selectTypeLongMenu = new Menu("long", () => {
            typeContainer.Set(typeof(long));
            typeInfoMenu.Run();
        });
        var selectTypeFloatMenu = new Menu("float", () => {
            typeContainer.Set(typeof(float));
            typeInfoMenu.Run();
        });
        var selectTypeDoubleMenu = new Menu("double", () => {
            typeContainer.Set(typeof(double));
            typeInfoMenu.Run();
        });
        var selectTypeCharMenu = new Menu("char", () => {
            typeContainer.Set(typeof(char));
            typeInfoMenu.Run();
        });
        var selectTypeStringMenu = new Menu("string", () => {
            typeContainer.Set(typeof(string));
            typeInfoMenu.Run();
        });
        var selectTypeVectorMenu = new Menu("Vector", () => {
            typeContainer.Set(typeof(float));
            typeInfoMenu.Run();
        });
        var selectTypeMatrixMenu = new Menu("Matrix", () => {
            typeContainer.Set(typeof(float));
            typeInfoMenu.Run();
        });

        mainMenu.AddMenuItem('0', quitMenu);
        mainMenu.AddMenuItem('1', generalInfoMenu);
        mainMenu.AddMenuItem('2', typeSelectMenu);

        typeInfoMenu.AddMenuItem('0', mainMenu);
        typeInfoMenu.AddMenuItem('M', additionalTypeInfoMenu);

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