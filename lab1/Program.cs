using System.Numerics;
using System.Reflection;

namespace lab1;

internal abstract class Program {
    private class TypeContainer {
        public Type Type = typeof(int);
    }

    private class ColorContainer {
        public ConsoleColor Color = ConsoleColor.Black;
    }

    private class MethodOverload {
        public int Count = 1;
        public int MinArgs = int.MaxValue;
        public int MaxArgs = int.MinValue;
    }

    private class Menu {
        private readonly Dictionary<char, Menu> _menuItems = new();
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
            } else {
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

    private static Menu CreateGeneralInfoMenu(Menu mainMenu) {
        var refAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = refAssemblies.SelectMany(asm => asm.GetTypes()).ToList();
        var methods = types.SelectMany(type => type.GetMethods()
            .Select(method => (typeName: type.FullName, method))).ToList();

        var nRefTypes = types.Count(type => type.IsClass);
        var nValTypes = types.Count(type => type.IsValueType);

        string MethodTupleToStr((string?, MethodInfo) t) => t.Item1 + "::" + t.Item2.Name;

        var methodWithLongestName = methods.MaxBy(tuple => tuple.method.Name.Length);
        var methodWithMostArguments = methods.MaxBy(tuple => tuple.method.GetParameters().Length);

        return new Menu("Общая информация по типам", () => {
            Console.WriteLine("Общая информация по типам");
            Console.WriteLine($"Подключенные сборки: {refAssemblies.Length}");
            Console.WriteLine($"Всего типов по всем подключенным сборкам: {nRefTypes + nValTypes}");
            Console.WriteLine($"Ссылочные типы (только классы): {nRefTypes}");
            Console.WriteLine($"Значимые типы: {nValTypes}");
            Console.WriteLine("Информация в соответствии с вариантом №0");
            Console.WriteLine($"Самое длинное название метода: {MethodTupleToStr(methodWithLongestName) + $" ({methodWithLongestName.method.Name.Length})"}");
            Console.WriteLine($"Метод с наибольшим числом аргументов: {MethodTupleToStr(methodWithMostArguments)}");
            Console.WriteLine($" - [ {String.Join(", ", methodWithMostArguments.method.GetParameters().Select(param => param.Name))} ]\n");
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню");
            Console.ReadKey();
            mainMenu.Run();
        });
    }

    private static Menu CreateTypeInfoMenu(TypeContainer typeContainer) {
        return new Menu("Информация по типу", () => {
            var type = typeContainer.Type;
            var nMethods = type.GetMethods().Length;
            var nProperties = type.GetProperties().Length;
            var nFields = type.GetFields().Length;
            var fieldsStr = string.Join(", ", type.GetFields().Select(f => f.Name));
            var propertiesStr = string.Join(", ", type.GetProperties().Select(f => f.Name));

            return $"Информация по типу: {type.FullName}\n" +
                   $"    Значимый тип: {(type.IsValueType ? '+' : '-')}\n" +
                   $"    Пространство имен: {type.Namespace}\n" +
                   $"    Сборка: {type.Assembly.GetName().Name}\n" +
                   $"    Общее число элементов: {nFields + nMethods + nProperties}\n" +
                   $"    Число методов: {nMethods}\n" +
                   $"    Число свойств: {nProperties}\n" +
                   $"    Число полей: {nFields}\n" +
                   $"    Список полей: {fieldsStr}\n" +
                   $"    Список свойств: {propertiesStr}\n";
        });
    }

    private static Menu CreateAdditionalTypeInfoMenu(TypeContainer typeContainer, Dictionary<string, MethodOverload> overloads) {
        return new Menu("Вывод дополнительной информации по методам", () => {
            PopulateOverloads(typeContainer.Type.GetMethods(), overloads);
            return GetOverloadsString(overloads, typeContainer);
        });
    }

    private static void PopulateOverloads(MethodInfo[] methods, Dictionary<string, MethodOverload> overloads) {
        foreach (var m in methods) {
            if (overloads.ContainsKey(m.Name)) {
                overloads[m.Name].Count++;
            } else {
                overloads.Add(m.Name, new MethodOverload());
            }

            overloads[m.Name].MinArgs = Math.Min(m.GetParameters().Length, overloads[m.Name].MinArgs);
            overloads[m.Name].MaxArgs = Math.Max(m.GetParameters().Length, overloads[m.Name].MaxArgs);
        }
    }

    private static string GetOverloadsString(Dictionary<string, MethodOverload> overloads, TypeContainer typeContainer) {
        if (overloads.Count == 0) {
            return $"Методы типа {typeContainer.Type.FullName}\n Название   Число перегрузок   Число параметров\n";
        }

        var firstColSize = overloads.Select(pair => pair.Key).MaxBy(name => name.Length)!.Length;

        string GetLine(string name, string count, string minmax) =>
                name.PadRight(firstColSize) + "   " + count.PadRight(20) + minmax.PadRight(20) + '\n';

        var result = $"Методы типа {typeContainer.Type.FullName}\n" +
                         GetLine("Название", "Число перегрузок", "Число параметров");
        return overloads.Aggregate(result,
                (current, pair) =>
                    current + GetLine(pair.Key, Convert.ToString(pair.Value.Count),
                        pair.Value.MinArgs == pair.Value.MaxArgs
                            ? Convert.ToString(pair.Value.MinArgs)
                            : pair.Value.MinArgs + ".." + pair.Value.MaxArgs));
    }

    private static void selectNumber(ref int i) {
        Console.Write("Введите число: ");
        while (true) {
            try {
                i = Convert.ToInt32(Console.ReadLine());
                break;
            } catch (FormatException) {
                Console.Clear();
                Console.Write("Неправильный ввод, попробуйте еще: ");
            }
        }
    }

    public static void Main(string[] args) {
        var typeContainer = new TypeContainer();
        var colorContainer = new ColorContainer();
        var overloads = new Dictionary<string, MethodOverload>();
        int num = 0;

        var mainMenu = new Menu("Выход в главное меню", () => "Информация по типам:");
        var quitMenu = new Menu("Выход из программы", () => {
            Console.ResetColor();
            Environment.Exit(1);
        });
        var typeSelectMenu = new Menu("Выбрать тип из списка", () => "Информация по типам\nВыберите тип:\n------");
        var generalInfoMenu = CreateGeneralInfoMenu(mainMenu);
        var typeInfoMenu = CreateTypeInfoMenu(typeContainer);
        var additionalTypeInfoMenu = CreateAdditionalTypeInfoMenu(typeContainer, overloads);
        var consoleColorMenu = new Menu("Параметры консоли", () => "Выберите опцию:");
        var colorSelectMenu = new Menu("Выбор цвета", () => "Выберите цвет:");
        var setForeGroundColor = new Menu("Установить цвет текста", () => {
            colorSelectMenu.Run();
            Console.ForegroundColor = colorContainer.Color;
            mainMenu.Run();
        });
        var setBackGroundColor = new Menu("Установить цвет фона", () => {
            colorSelectMenu.Run();
            Console.BackgroundColor = colorContainer.Color;
            mainMenu.Run();
        });
        var selectByNumOverloads = new Menu("Отобрать по номеру перегрузок", () => {
            selectNumber(ref num);
            Console.WriteLine(GetOverloadsString(overloads.Where(o => o.Value.Count == num).ToDictionary(o => o.Key, o => o.Value), typeContainer));
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню");
            Console.ReadKey();
            mainMenu.Run();
        });
        var selectByNumParameters = new Menu("Отобрать по номеру параметров", () => {
            selectNumber(ref num);
            var methods = typeContainer.Type.GetMethods().Where(m => m.GetParameters().Length == num).ToArray();
            overloads.Clear();
            Console.WriteLine(overloads.Count);
            PopulateOverloads(methods, overloads);
            Console.WriteLine(GetOverloadsString(overloads, typeContainer));
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню");
            Console.ReadKey();
            mainMenu.Run();
        });

        void SetType(Type t) {
            typeContainer.Type = t;
            typeInfoMenu.Run();
        }

        var selectTypeUintMenu = new Menu("uint", () => { SetType(typeof(uint)); });
        var selectTypeIntMenu = new Menu("int", () => { SetType(typeof(int)); });
        var selectTypeLongMenu = new Menu("long", () => { SetType(typeof(long)); });
        var selectTypeFloatMenu = new Menu("float", () => { SetType(typeof(float)); });
        var selectTypeDoubleMenu = new Menu("double", () => { SetType(typeof(double)); });
        var selectTypeCharMenu = new Menu("char", () => { SetType(typeof(char)); });
        var selectTypeStringMenu = new Menu("string", () => { SetType(typeof(string)); });
        var selectTypeVectorMenu = new Menu("Vector", () => { SetType(typeof(Vector)); });
        var selectTypeMatrixMenu = new Menu("Matrix", () => { SetType(typeof(Matrix4x4)); });

        var selectBlack = new Menu("Черный", () => colorContainer.Color = ConsoleColor.Black);
        var selectDarkBlue = new Menu("Темно-синий", () => colorContainer.Color = ConsoleColor.DarkBlue);
        var selectDarkGreen = new Menu("Темно-зеленый", () => colorContainer.Color = ConsoleColor.DarkGreen);
        var selectDarkCyan = new Menu("Темно-голубой", () => colorContainer.Color = ConsoleColor.DarkCyan);
        var selectDarkRed = new Menu("Темно-красный", () => colorContainer.Color = ConsoleColor.DarkRed);
        var selectDarkMagenta = new Menu("Темно-пурпурный", () => colorContainer.Color = ConsoleColor.DarkMagenta);
        var selectDarkYellow = new Menu("Темно-желтый", () => colorContainer.Color = ConsoleColor.DarkYellow);
        var selectGray = new Menu("Серый", () => colorContainer.Color = ConsoleColor.Gray);
        var selectDarkGray = new Menu("Темно-серый", () => colorContainer.Color = ConsoleColor.DarkGray);
        var selectBlue = new Menu("Синий", () => colorContainer.Color = ConsoleColor.Blue);
        var selectGreen = new Menu("Зеленый", () => colorContainer.Color = ConsoleColor.Green);
        var selectCyan = new Menu("Голубой", () => colorContainer.Color = ConsoleColor.Cyan);
        var selectRed = new Menu("Красный", () => colorContainer.Color = ConsoleColor.Red);
        var selectMagenta = new Menu("Пурпурный", () => colorContainer.Color = ConsoleColor.Magenta);
        var selectYellow = new Menu("Желтый", () => colorContainer.Color = ConsoleColor.Yellow);
        var selectWhite = new Menu("Белый", () => colorContainer.Color = ConsoleColor.White);

        mainMenu.AddMenuItem('0', quitMenu);
        mainMenu.AddMenuItem('1', generalInfoMenu);
        mainMenu.AddMenuItem('2', typeSelectMenu);
        mainMenu.AddMenuItem('3', consoleColorMenu);

        consoleColorMenu.AddMenuItem('0', mainMenu);
        consoleColorMenu.AddMenuItem('1', setForeGroundColor);
        consoleColorMenu.AddMenuItem('2', setBackGroundColor);

        typeInfoMenu.AddMenuItem('0', mainMenu);
        typeInfoMenu.AddMenuItem('M', additionalTypeInfoMenu);

        additionalTypeInfoMenu.AddMenuItem('0', mainMenu);
        additionalTypeInfoMenu.AddMenuItem('1', selectByNumOverloads);
        additionalTypeInfoMenu.AddMenuItem('2', selectByNumParameters);

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

        colorSelectMenu.AddMenuItem('0', mainMenu);
        colorSelectMenu.AddMenuItem('1', selectBlack);
        colorSelectMenu.AddMenuItem('2', selectDarkBlue);
        colorSelectMenu.AddMenuItem('3', selectDarkGreen);
        colorSelectMenu.AddMenuItem('4', selectDarkCyan);
        colorSelectMenu.AddMenuItem('5', selectDarkRed);
        colorSelectMenu.AddMenuItem('6', selectDarkMagenta);
        colorSelectMenu.AddMenuItem('7', selectDarkYellow);
        colorSelectMenu.AddMenuItem('8', selectGray);
        colorSelectMenu.AddMenuItem('9', selectDarkGray);
        colorSelectMenu.AddMenuItem('A', selectBlue);
        colorSelectMenu.AddMenuItem('B', selectGreen);
        colorSelectMenu.AddMenuItem('C', selectCyan);
        colorSelectMenu.AddMenuItem('D', selectRed);
        colorSelectMenu.AddMenuItem('E', selectMagenta);
        colorSelectMenu.AddMenuItem('F', selectYellow);
        colorSelectMenu.AddMenuItem('G', selectWhite);

        mainMenu.Run();
    }
}