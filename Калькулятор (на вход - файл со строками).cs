using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace ConsoleApp4
{
    class Program
    {
        /// <summary>
        /// переводит из инфиксной записи в постфиксную
        /// </summary>
        /// <param name="FromInFixToPostfix"></param>
        /// <returns></returns>
        static string FromInFixToPostfix (string expression, Dictionary<string,int> variables)
        {
            var ms = Regex.Matches(expression, @"\w+|\d+|[()+-/*]");
            Stack<char> st = new Stack<char>();
            List<string> res = new List<string>();
            int Flag = 0;
            foreach (Match m in ms)
            {
                if (variables.ContainsKey(m.Value))
                {
                    res.Add(variables[m.Value].ToString() + " ");
                }
                if (int.TryParse(m.Value,out int a))
                {
                    res.Add(a.ToString() + " ");
                }
                else if (m.Value == "(")
                {
                    st.Push('(');
                    Flag = 1;
                }
                else if (m.Value == ")")
                {
                    Flag = 0;
                    char prioritized = st.Pop();
                    while (prioritized!='(')
                    {
                        res.Add(prioritized.ToString());
                        if (prioritized != '(')
                        {
                            prioritized = st.Pop();
                        }
                    }
                }
                else if (((m.Value == "-") || (m.Value == "+") || (m.Value == "*") || (m.Value == "/") ) && (Flag == 0))
                {
                    switch (m.Value)
                    {
                        case "+":
                            {
                                var lst = new List<char>();
                                while (st.Count != 0)
                                {
                                    char oper = st.Pop();
                                    if ((oper == '+') || (oper == '-') || (oper == '*') || (oper == '/'))
                                    {
                                        res.Add(oper.ToString());
                                    }
                                    else
                                    {
                                        lst.Add(oper);
                                    }
                                }
                                lst.Reverse();
                                foreach (var x in lst)
                                {
                                    st.Push(x);
                                }
                                st.Push('+');
                            }
                            break;
                        case "-":
                            {
                                var lst = new List<char>();
                                while (st.Count != 0)
                                {
                                    char oper = st.Pop();
                                    if ((oper == '+') || (oper == '-') || (oper == '*') || (oper == '/'))
                                    {
                                        res.Add(oper.ToString());
                                    }
                                    else
                                    {
                                        lst.Add(oper);
                                    }
                                }
                                lst.Reverse();
                                foreach (var x in lst)
                                {
                                    st.Push(x);
                                }
                                st.Push('-');
                            }
                            break;
                        case "*":
                            {
                                var lst = new List<char>();
                                while (st.Count != 0)
                                {
                                    char oper = st.Pop();
                                    if ((oper == '*') || (oper == '/'))
                                    {
                                        res.Add(oper.ToString());
                                    }
                                    else
                                    {
                                        lst.Add(oper);
                                    }
                                }
                                lst.Reverse();
                                foreach (var x in lst)
                                {
                                    st.Push(x);
                                }
                                st.Push('*');
                            }
                            break;
                        case "/":
                            {
                                var lst = new List<char>();
                                while (st.Count != 0)
                                {
                                    char oper = st.Pop();
                                    if ((oper == '*') || (oper == '/'))
                                    {
                                        res.Add(oper.ToString());
                                    }
                                    else
                                    {
                                        lst.Add(oper);
                                    }
                                }
                                lst.Reverse();
                                foreach (var x in lst)
                                {
                                    st.Push(x);
                                }
                                st.Push('/');
                            }
                            break;     
                    }
                }
                else if (Flag == 1)
                {
                    st.Push(m.Value[0]);
                }
            }
            while (st.Count!=0)
            {
                res.Add(st.Pop().ToString());
            }
            string stringres = "";
            foreach (var x in res)
                stringres += x;
            return stringres;
        }

        /// <summary>
        /// Вычисляет значение выражения, записанного в польской инверсной записи
        /// </summary>
        /// <param name="CountExpressionInPOLIZ"></param>
        /// <returns></returns>
        static void CountExpressionInPOLIZ(string expression)
        {
            var st = new Stack<int>();
            int flag = 0;
            flag = 0;
            var ss = Regex.Matches(expression, @"\d+|[+*/-]");
            if (ss.Count == 0)// Есть хоть одно совпадение
            {
                flag = 2;
                Console.WriteLine("Вы ввели строку, не содержащую допустимых символов");
            }
            else
            {
                foreach (Match m in ss)
                {
                    if (int.TryParse(m.Value, out int a))
                    {
                        st.Push(a);
                    }
                    else if ((m.Value == "+") && (st.Count >= 2))
                    {
                        st.Push(st.Pop() + st.Pop());
                    }
                    else if ((m.Value == "*") && (st.Count >= 2))
                    {
                        st.Push(st.Pop() * st.Pop());
                    }
                    else if ((m.Value == "-")&&(st.Count >= 2))
                    {
                        var k = st.Pop();
                        var z = st.Pop();
                        st.Push(z - k);
                    }
                    else if ((m.Value == "/") && (st.Count >= 2))
                    {
                        var k = st.Pop();
                        var z = st.Pop();
                        st.Push((int)(z/k));
                    }
                    else
                    {
                        flag = 1;
                    }
                }
            }
            if ((st.Count > 1) || (flag == 1))
            {
                flag = 200;
                Console.WriteLine("В выражении неверное кол-во операторов");
            }
            if (flag == 0)
            {
                Console.WriteLine($"Ответ:{st.Pop()}");
            }
        }

        /// <summary>
        /// вычисляет значения выражения из файла (возможно использование переменных
        /// </summary>
        /// <param name="CountExpressionFromFile"></param>
        static void CountExpressionFromFile(string path = "test.txt")
        {
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                var dic = new Dictionary<string, int>();
                string text = sr.ReadToEnd();
                var variables = Regex.Matches(text, @"(\w+)=(\d+)",RegexOptions.IgnoreCase);
                foreach (Match m in variables)
                {
                    dic.Add(m.Groups[1].Value, int.Parse(m.Groups[2].Value));
                }
                var textsequence = File.ReadLines(path);
                textsequence = textsequence.Where(x => x.Contains("EVAL"));
                foreach (var x in textsequence)
                {
                    Console.WriteLine(x);
                    CountExpressionInPOLIZ(FromInFixToPostfix(x,dic));
                }
            }
        }
        static void Main(string[] args)
        {
            CountExpressionFromFile();
            Console.ReadLine();
        }
    }
}
