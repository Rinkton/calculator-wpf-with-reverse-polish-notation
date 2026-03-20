using System.Text;

namespace BL
{
    public class MathExpression
    {
        public MathExpression(string input)
        {

        }

        /// <summary>
        /// Преобразует инфиксное выражение в обратную польскую запись (ОПЗ)
        /// </summary>
        /// <param name="input">Инфиксное выражение (например: "3 + 4 * 2")</param>
        /// <returns>Строка в ОПЗ (например: "3 4 2 * +")</returns>
        public string GetReversePolishNotationText(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var output = new List<string>();
            var operators = new Stack<char>();

            for(int i = 0; i < input.Length; i++) {
                char c = input[i];

                // Пропускаем пробелы
                if(char.IsWhiteSpace(c))
                    continue;

                // Если число или десятичная точка
                if(char.IsDigit(c) || c == '.') {
                    string number = ParseNumber(input, ref i);
                    output.Add(number);
                    continue;
                }

                // Если оператор
                if(IsOperator(c)) {
                    while(operators.Count > 0 &&
                           operators.Peek() != '(' &&
                           GetPriority(operators.Peek()) >= GetPriority(c)) {
                        output.Add(operators.Pop().ToString());
                    }
                    operators.Push(c);
                }
                // Открывающая скобка
                else if(c == '(') {
                    operators.Push(c);
                }
                // Закрывающая скобка
                else if(c == ')') {
                    while(operators.Count > 0 && operators.Peek() != '(') {
                        output.Add(operators.Pop().ToString());
                    }
                    if(operators.Count > 0 && operators.Peek() == '(')
                        operators.Pop();
                }
            }

            // Выгружаем оставшиеся операторы
            while(operators.Count > 0) {
                output.Add(operators.Pop().ToString());
            }

            return string.Join(" ", output);
        }

        /// <summary>
        /// Вычисляет результат выражения в обратной польской записи
        /// </summary>
        /// <param name="rpn">Выражение в ОПЗ (например: "3 4 2 * +")</param>
        /// <returns>Результат вычисления</returns>
        public double GetReverseAnswer(string rpn)
        {
            if(string.IsNullOrWhiteSpace(rpn))
                return 0;

            var stack = new Stack<double>();
            string[] tokens = rpn.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach(string token in tokens) {
                if(IsNumber(token)) {
                    stack.Push(double.Parse(token));
                }
                else if(IsOperator(token[0]) && token.Length == 1) {
                    if(stack.Count < 2)
                        throw new InvalidOperationException("Недостаточно операндов");

                    double b = stack.Pop();
                    double a = stack.Pop();
                    double result = Calculate(a, b, token[0]);
                    stack.Push(result);
                }
                else {
                    throw new InvalidOperationException($"Неизвестный токен: {token}");
                }
            }

            if(stack.Count != 1)
                throw new InvalidOperationException("Некорректное выражение");

            return stack.Pop();
        }

        // Приватные вспомогательные методы

        private string ParseNumber(string input, ref int index)
        {
            var sb = new StringBuilder();

            while(index < input.Length && (char.IsDigit(input[index]) || input[index] == '.')) {
                sb.Append(input[index]);
                index++;
            }
            index--; // Возвращаем индекс назад, т.к. цикл for увеличит его

            return sb.ToString();
        }

        private bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
        }

        private bool IsNumber(string token)
        {
            return double.TryParse(token, out _);
        }

        private int GetPriority(char op)
        {
            switch(op) {
                case '^': return 3; // Наивысший приоритет
                case '*':
                case '/': return 2;
                case '+':
                case '-': return 1;
                default: return 0;
            }
        }

        private double Calculate(double a, double b, char op)
        {
            switch(op) {
                case '+': return a + b;
                case '-': return a - b;
                case '*': return a * b;
                case '/':
                    if(b == 0)
                        throw new DivideByZeroException("Деление на ноль");
                    return a / b;
                case '^': return Math.Pow(a, b);
                default: throw new InvalidOperationException($"Неизвестный оператор: {op}");
            }
        }
    }
}
