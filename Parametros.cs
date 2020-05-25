using System;
using System.Text.RegularExpressions;

namespace Parametros2
{
    class Program
    {
        static String reserv = @"^(byte|short|int|long|float|double|bool|char|String|Array|Byte|Short|Integer|Long|Float|Double|Boolean|Character|[a-zA-Z_$][a-zA-Z_$0-9]*)";

        static void Main(string[] args)
        {
            int sw = 1;
            while (sw == 1)
            {
                Console.WriteLine("Escriba los parametros que desea validar separados por comas");
                Console.Write("Parametros >> ");
                String ParamValid = Console.ReadLine().Trim();

                String pattern = @"\s*,\s*";
                String[] elems = System.Text.RegularExpressions.Regex.Split(ParamValid, pattern);

                bool noHayError = true, hayIdsRepetidos = false, idUsaPalabraReserv = false;
                int elemsAVerificar = 0;

                String[] id = new String[elems.Length]; // Vector de identificadores

                for (int i = 0; i < elems.Length; i++)
                {
                    elems[i] = elems[i].Trim();
                    int corchAbre = elems[i].IndexOf("[");
                    int corchCierr = elems[i].IndexOf("]");

                    if (corchAbre == -1 && corchCierr == -1)
                    {
                        int spaceIdx = elems[i].IndexOf(" ");

                        if (spaceIdx > -1)
                        {
                            // Ejemplo: "Hola a" => ["Hola", "a"]
                            String pt = @"\s+";
                            String[] valor = System.Text.RegularExpressions.Regex.Split(elems[i].Trim(), pt);

                            if (valor.Length == 2)
                            {
                                id[elemsAVerificar] = valor[1];
                                ++elemsAVerificar;
                            }
                        }
                    }
                    else // Entra aqui si el parametro es un vector o una matriz
                    {
                        int corchUltimoIdx = elems[i].LastIndexOf("]");

                        if (corchUltimoIdx > -1)
                        {
                            if (corchUltimoIdx == elems[i].Length - 1)
                            {
                                // Ejemplo: Clase a[][], int b [] []
                                int primerCorch = elems[i].IndexOf("[");

                                if (primerCorch > -1)
                                {
                                    String toSplit = elems[i].Substring(0, primerCorch).Trim();
                                    String patron = @"\s+";
                                    String[] valor = System.Text.RegularExpressions.Regex.Split(toSplit, patron);

                                    if (valor.Length == 2)
                                    {
                                        id[elemsAVerificar] = valor[1];
                                        ++elemsAVerificar;
                                    }
                                }
                            }
                            else if (corchUltimoIdx < elems[i].Length - 1)
                            {
                                id[elemsAVerificar] = elems[i].Substring(corchUltimoIdx + 1);
                                ++elemsAVerificar;
                            }
                        }
                    }
                }

                Regex v = new Regex(reserv);

                for (int y = 0; y < elemsAVerificar; y++)
                {
                    if (v.IsMatch(id[y].Trim())) // Esta usando una palabra reservada
                    {
                        idUsaPalabraReserv = true;
                    }
                }

                // Verificar que identificadores no sean iguales
                for (int y = 0; y < elemsAVerificar - 1; y++)
                {
                    for (int j = y + 1; j < elems.Length; j++)
                    {
                        if (id[y] == id[j])
                        {
                            hayIdsRepetidos = true;
                        }
                    }
                }

                for (int i = 0; i < elems.Length; i++) // Ej.: final int a
                {
                    if (elems[i].Length > 4)
                    {
                        String substr = elems[i].Substring(0, 5);
                        if (substr == "final")
                        {
                            elems[i] = elems[i].Substring(5).Trim();
                        }
                    }

                    if (!verificarParam(elems[i]))
                    {
                        Console.WriteLine("Error en el parametro " + (i + 1));
                        noHayError = false;
                    }
                }

                if (hayIdsRepetidos)
                {
                    Console.WriteLine("Error, hay identificadores repetidos");
                }

                if (idUsaPalabraReserv)
                {
                    Console.WriteLine("Error, hay identificadores usando palabras reservadas");
                }

                if (noHayError && !hayIdsRepetidos && !idUsaPalabraReserv)
                {
                    Console.WriteLine("Los parametros estan bien escritos");
                }

                Console.Write("¿Desea continuar? (1: si/ 0: no): ");
                String opc = Console.ReadLine();

                if (opc != "1")
                {
                    sw = 0;
                }
            }

        }

        static bool verificarParam(String elem)
        {
            // int[] a, int[]a, int[   ]a, int [] [] [] a
            String exp2 = reserv + @"\s*(\[\s*\]\s*)+\s*[a-zA-Z_$][a-zA-Z_$0-9]*$";
            Regex r2 = new Regex(exp2);

            if (r2.IsMatch(elem))
            {
                return true;
            }

            // int a[] [] [], int b[][][]
            String exp3 = reserv + @"\s+[a-zA-Z_$][a-zA-Z_$0-9]*\s*(\[\s*\]\s*)*$";
            Regex r3 = new Regex(exp3);

            if (r3.IsMatch(elem))
            {
                return true;
            }

            return false;
        }
    }
}
