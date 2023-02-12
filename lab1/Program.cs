using System.Reflection;

namespace lab1;

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

    private class MethodOverload {
        public int Count = 1;
        public int MinArgs = int.MaxValue;
        public int MaxArgs = int.MinValue;
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
            Console.WriteLine($"Самое длинное название метода: {MethodTupleToStr(methodWithLongestName)}");
            Console.WriteLine($"Метод с наибольшим числом аргументов: {MethodTupleToStr(methodWithMostArguments)}\n");
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню");
            Console.ReadKey();
            mainMenu.Run();
        });
    }

    private static Menu CreateTypeInfoMenu(TypeContainer typeContainer) {
        return new Menu("Информация по типу", () => {
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
    }

    private static Menu CreateAdditionalTypeInfoMenu(TypeContainer typeContainer) {
        return new Menu("Вывод дополнительной информации по методам", () => {
            var methods = typeContainer.Get().GetMethods();
            var overloads = new Dictionary<string, MethodOverload>();

            foreach (var m in methods) {
                if (overloads.ContainsKey(m.Name)) {
                    overloads[m.Name].Count++;
                } else {
                    overloads.Add(m.Name, new MethodOverload());
                }

                overloads[m.Name].MinArgs = Math.Min(m.GetParameters().Length, overloads[m.Name].MinArgs);
                overloads[m.Name].MaxArgs = Math.Max(m.GetParameters().Length, overloads[m.Name].MaxArgs);
            }

            var result = $"Методы типа {typeContainer.Get().FullName}\n" +
                         $"{"Название",-20} {"Число перегрузок",-20} {"Число параметров",-20}\n";
            return overloads.Aggregate(result,
                (current, pair) =>
                    current +
                    $"{pair.Key,-20} {pair.Value.Count,-20} " +
                    $"{(pair.Value.MinArgs == pair.Value.MaxArgs ? pair.Value.MinArgs : pair.Value.MinArgs + ".." + pair.Value.MaxArgs),-20}\n");
        });
    }

    public static void Main(string[] args) {
        var typeContainer = new TypeContainer();

        var mainMenu = new Menu("Выход в главное меню", () => "Информация по типам:");
        var quitMenu = new Menu("Выход из программы", () => Environment.Exit(1));
        var typeSelectMenu = new Menu("Выбрать тип из списка", () => "Информация по типам\nВыберите тип:\n------");
        var generalInfoMenu = CreateGeneralInfoMenu(mainMenu);
        var typeInfoMenu = CreateTypeInfoMenu(typeContainer);
        var additionalTypeInfoMenu = CreateAdditionalTypeInfoMenu(typeContainer);

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

        additionalTypeInfoMenu.AddMenuItem('0', mainMenu);

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