using System.Text;

namespace BL
{
    public class MathExpression
    {
        private string reversePolishNotationText;
        private string answerText;

        public MathExpression(string input)
        {
            reversePolishNotationText = GetReversePolishNotation(input);
            answerText = GetAnswer(reversePolishNotationText).ToString();
        }

        public string GetReversePolishNotationText()
        {
            return reversePolishNotationText;
        }

        public string GetAnswerText()
        {
            return answerText;
        }

        private string GetReversePolishNotation(string input)
        {
            //ValidateExpression(input); TODO
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

        private double GetAnswer(string rpn)
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

        private void ValidateExpression(string expression)
        {
            int bracketBalance = 0;

            for(int i = 0; i < expression.Length; i++) {
                char c = expression[i];

                if(char.IsWhiteSpace(c))
                    continue;

                if(char.IsDigit(c) || c == '.')
                    continue;

                if(IsOperator(c))
                    continue;

                if(c == '(') {
                    bracketBalance++;
                    continue;
                }

                if(c == ')') {
                    bracketBalance--;
                    if(bracketBalance < 0)
                        throw new ArgumentException($"Лишняя закрывающая скобка в позиции {i}");
                    continue;
                }

                // Если дошли сюда - недопустимый символ
                throw new ArgumentException(
                    $"Недопустимый символ '{c}' в позиции {i}. " +
                    $"Разрешены: цифры, ., +, -, *, /, ^, (, ) и пробелы");
            }

            if(bracketBalance != 0)
                throw new ArgumentException("Несбалансированные скобки");
        }

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
