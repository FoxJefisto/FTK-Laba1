using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.IO;

namespace laba1
{
    class Program
    {

        static Dictionary<string, bool> inputToChoice = new Dictionary<string, bool>()
            {
                { "Да", true },
                { "да", true },
                { "y", true },
                { "Y", true }
            };
        static string ConvertToPostfix(string expression)
        {
            Stack<string> st = new Stack<string>();
            List<string> lstPostfix = new List<string>();
            string[] exprInfix = expression.Split(' ');
            Dictionary<string, int> prioritet = new Dictionary<string, int>();
            prioritet["*"] = 3;
            prioritet["/"] = 3;
            prioritet["+"] = 2;
            prioritet["-"] = 2;
            prioritet["("] = 1;

            for (int i = 0; i < exprInfix.Length; i++)
            {
                if (exprInfix[i].AsEnumerable().Any((ch) => char.IsLetter(ch)) || exprInfix[i].AsEnumerable().Any((ch) => char.IsDigit(ch)))
                {
                    lstPostfix.Add(exprInfix[i]);
                }
                else if (exprInfix[i] == "(")
                {
                    st.Push(exprInfix[i]);
                }
                else if (exprInfix[i] == ")")
                {
                    while (st.Peek() != "(")
                    {
                        string cur = st.Pop();
                        if (prioritet.ContainsKey(cur))
                        {
                            lstPostfix.Add(cur);
                        }
                        if (st.Count == 0)
                        {
                            throw new Exception("Ошибка! Неверно поставлен разделитель, либо не согласованы скобки");
                        }
                    }
                    st.Pop();
                }
                else if (prioritet.ContainsKey(exprInfix[i]))
                {
                    while (st.Count != 0 && prioritet[st.Peek()] >= prioritet[exprInfix[i]])
                        lstPostfix.Add(st.Pop());
                    st.Push(exprInfix[i]);
                }
            }
            while (st.Count != 0)
            {
                if (st.Peek() != "(")
                    lstPostfix.Add(st.Pop());
                else
                    throw new Exception("Ошибка! Неверно поставлен разделитель, либо не согласованы скобки");
            }

            string strPostfix = String.Join(" ", lstPostfix);
            return strPostfix;
        }

        static Dictionary<string, double> DefineVariables(string expression)
        {
            Dictionary<string, double> operandToValue = new Dictionary<string, double>();
            string[] lstExpression = expression.Split(' ');
            for(int i = 0; i < lstExpression.Length; i++)
            {
                if (lstExpression[i].AsEnumerable().Any((ch) => char.IsLetter(ch)))
                {
                    if (!operandToValue.ContainsKey(lstExpression[i]))
                    {
                        Console.Write($"{lstExpression[i]} = ");
                        string input = Console.ReadLine();
                        double value = Convert.ToDouble(input);
                        operandToValue[lstExpression[i]] = value;
                    }
                }
                else if (lstExpression[i].AsEnumerable().Any((ch) => char.IsDigit(ch)))
                {
                    double value = Convert.ToDouble(lstExpression[i]);
                    operandToValue[lstExpression[i]] = value;
                }
            }
            return operandToValue;
        }

        static double CalcPostfixExpression(string strPostfix, Dictionary<string, double> operandToValue)
        {
            Stack<double> st = new Stack<double>();
            string[] lstPostfix = strPostfix.Split(' ');
            for(int i = 0; i < lstPostfix.Length; i++)
            {
                if (operandToValue.ContainsKey(lstPostfix[i]))
                {
                    st.Push(operandToValue[lstPostfix[i]]);
                }
                else
                {
                    double operand2 = st.Pop();
                    double result;
                    if (st.Count != 0)
                    {
                        double operand1 = st.Pop();
                        result = RunOperator(lstPostfix[i], operand1, operand2);
                    }
                    else if (lstPostfix[i] == "-")
                        result = -operand2;
                    else if (lstPostfix[i] == "+")
                        result = operand2;
                    else
                        throw new Exception("Ошибка! Не хватает операндов для совершения операции.");
                    st.Push(result);
                }
            }
            return st.Pop();
        }

        static double RunOperator(string oper, double x1, double x2)
        {
            double result;
            if (oper == "*")
                result = x1 * x2;
            else if (oper == "/")
            {
                if (x2 == 0)
                    throw new DivideByZeroException();
                result = x1 / x2;
            }
            else if (oper == "+")
                result = x1 + x2;
            else if (oper == "-")
                result = x1 - x2;
            else
                throw new Exception("Ошибка! Неизвестный оператор");
            return result;
        }

        static string GetExprWithValue(string expression, Dictionary<string, double> operandToValue)
        {
            StringBuilder expr = new StringBuilder(expression);
            foreach(var operand in operandToValue)
            {
                expr.Replace(operand.Key, operand.Value.ToString());
            }
            return expr.ToString();
        }

        static Double Eval(string expression, Dictionary<string, double> operandToValue)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            return Convert.ToDouble(table.Compute(GetExprWithValue(expression, operandToValue), String.Empty));
        }

        static void SolveTask(string exprInfix)
        {
            string exprPostfix = ConvertToPostfix(exprInfix);
            Console.WriteLine($"Выражение в инфиксной форме: {exprInfix}");
            Console.WriteLine($"Выражение в постфиксной форме: {exprPostfix}");
            Console.WriteLine("Хотите ли вы проинициализировать операнды и вычислить значение по постфиксной записи?");
            bool choice = inputToChoice.GetValueOrDefault(Console.ReadLine());
            if (choice)
            {
                Dictionary<string, double> operandToValue = DefineVariables(exprInfix);
                double resultPostfix = CalcPostfixExpression(exprPostfix, operandToValue);
                Console.WriteLine($"Значение выражения по постфиксной записи: {resultPostfix}");
                Console.WriteLine("Проверка...");
                double resultInfix = Eval(exprInfix, operandToValue);
                Console.WriteLine($"Значение выражения по инфиксной записи: {resultInfix}");
                Console.WriteLine(new string('-', 60));
            }
        }

        static void Main(string[] args)
        {
            try
            {
                string exprInfix="";
                Console.WriteLine("Совершить ввод формулы из файла?");
                bool choice = inputToChoice.GetValueOrDefault(Console.ReadLine());
                if (choice)
                {
                    using (StreamReader sr = new StreamReader("input.txt"))
                    {
                        while ((exprInfix = sr.ReadLine()) != null)
                            SolveTask(exprInfix);
                    }
                }
                else
                {
                    Console.WriteLine("Введите выражение");
                    exprInfix = Console.ReadLine();
                    SolveTask(exprInfix);
                }
            }
            catch(Exception ex){
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
