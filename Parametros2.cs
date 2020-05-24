using System;
using System.Text.RegularExpressions;

namespace Parametros2
{
    class Program
    {
        static void Main(string[] args)
        {
            int sw = 1;

            while (sw == 1)
            {
                Console.Write("Escriba el parámetro que desea validar: ");

                // Se lee la entrada y se quitan los espacios del lado izquierdo y derecho
                string ParamValid = Console.ReadLine().Trim();

                // Expresion regular para separar por comas
                // Ejemplo: "Hola a , int b" => Resulta en ["Hola a", "int b"]
                string pattern = @"\s*,\s*";

                // Hace la separación de arriba con el patrón dado
                string[] elems = System.Text.RegularExpressions.Regex.Split(ParamValid, pattern);

                bool noHayError = true;
                bool hayIdsRepetidos = false;
                int elemsAVerificar = 0;

                String[] id = new String[elems.Length]; // Vector de identificadores

                // Guardar identificadores
                for (int i = 0; i < elems.Length; i++)
                {
                    int corchAbre = elems[i].IndexOf("["); // indice del primer corchete abre
                    int corchCierr = elems[i].IndexOf("]"); // indice del primer corchete cierra
                    if (corchAbre == -1 && corchCierr == -1)
                    {
                        // Se busca el primer indice de un espacio
                        int spaceIdx = elems[i].IndexOf(" ");

                        if (spaceIdx > -1)
                        {
                            // Expresion regular para separar pos un espacio o más
                            // Ejemplo: "Hola a" => ["Hola", "a"], ["int", "b"]
                            string pt = @"\s+";
                            string[] valor = System.Text.RegularExpressions.Regex.Split(elems[i], pt);

                            if (valor.Length == 2)
                            {
                                // Si se encuentran el tipo y el identificador sin error, se guardan
                                id[elemsAVerificar] = valor[1]; // Identificadores para comparar que no se repitan
                                ++elemsAVerificar;
                            }
                        }
                    }
                    else
                    {
                        // Entra aqui si el parametro es un vector o una matriz
                        // Ej.: int[] a, int[][]b, int c[][]

                        // Busca el ultimo indice de "]"
                        int corchUltimoIdx = elems[i].LastIndexOf("]");

                        // Si no se encuentra el indice de "]" se marca el error
                        if (corchUltimoIdx > -1)
                        {
                            // En caso de no encontrar error, se guardan
                            if (corchUltimoIdx == elems[i].Length - 1)
                            {
                                // Ejemplo: Clase a[][], int b [] []
                                int primerCorch = elems[i].IndexOf("[");

                                if (primerCorch > -1)
                                {
                                    string toSplit = elems[i].Substring(0, primerCorch).Trim();

                                    string patron = @"\s+";
                                    string[] valor = System.Text.RegularExpressions.Regex.Split(toSplit, patron);

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

                    // Verificar que identificadores no sean iguales
                    for (int y = 0; y < elemsAVerificar - 1; y++)
                    {
                        for (int j = y + 1; j < elems.Length; j++)
                        {
                            if (id[y] == id[j])
                            {
                                Console.WriteLine("Error, hay identificadores repetidos");
                                hayIdsRepetidos = true;
                            }
                        }
                    }
                }

                for (int i = 0; i < elems.Length; i++)
                {
                    String substr = elems[i].Substring(0, 5);
                    if (substr == "final")
                    {
                        elems[i] = elems[i].Substring(5).Trim();
                    }

                    if (!verificarParam(elems[i]))
                    {
                        Console.WriteLine("Error en el parametro " + (i + 1));
                        noHayError = false;
                    }
                }

                if (noHayError && !hayIdsRepetidos)
                {
                    Console.WriteLine("Los parametros estan bien escritos");
                }

                Console.Write("¿Desea continuar? (1: si/ 0: no): ");
                string opc = Console.ReadLine();

                if (opc != "1")
                {
                    sw = 0;
                }
            }

        }

        static bool verificarParam(String elem)
        {

            // Palabras reservadas para tipos
            String reserv = @"^(byte|short|int|long|float|double|bool|char|String|Array|Byte|Short|Integer|Long|Float|Double|Boolean|Character)";

            // int a, String b
            String exp1 = reserv + @"\s+[a-zA-Z_$][a-zA-Z_$0-9]*$";
            Regex r1 = new Regex(exp1);

            if (r1.IsMatch(elem))
            {
                return true;
            }

            // int[] a, int[]a, int[   ]a, int [] [] [] a
            String exp2 = reserv + @"\s*(\[\s*\]\s*)*\s*[a-zA-Z_$][a-zA-Z_$0-9]*$";
            Regex r2 = new Regex(exp2);

            if (r2.IsMatch(elem))
            {
                return true;
            }

            // int a[] [] [], int b[][][]
            String exp3 = reserv + @"\s*[a-zA-Z_$][a-zA-Z_$0-9]*\s*(\[\s*\]\s*)*$";
            Regex r3 = new Regex(exp3);

            if (r3.IsMatch(elem))
            {
                return true;
            }

            return false;
        }
    }
}
