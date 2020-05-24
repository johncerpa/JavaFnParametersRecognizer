using System;
using System.Text.RegularExpressions;

namespace Variable
{
    class Program
    {
        static void Main(string[] args)
        {
            int sw = 1;
            string variable;

            while (sw == 1)
            {

                Console.Write("Ingrese variable a verificar: ");
                variable = Console.ReadLine().Trim();

                int espacio = variable.IndexOf(" ");

                // Si no contiene espacios
                if (espacio == -1)
                {
                    string exprVariable = @"^[a-zA-Z_$][a-zA-Z_$0-9]*$";
                    Regex regexVariable = new Regex(exprVariable);

                    if (regexVariable.IsMatch(variable))
                    {
                        Console.WriteLine("Variable escrita correctamente");
                    }
                    else
                    {
                        Console.WriteLine("Error en la variable");
                    }
                }
                else
                {
                    Console.WriteLine("La variable no puede conformarse por palabras separadas por espacio.");
                }

                Console.Write("¿Desea continuar verificando variables? (1: si, 0: no): ");
                string opc = Console.ReadLine();

                if (opc != "1")
                {
                    sw = 0;
                }
            }
        }
    }
}
